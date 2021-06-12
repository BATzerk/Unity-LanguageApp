using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
    // References
    [SerializeField] private BottomTabBar bottomTabBar;
    [SerializeField] private PanelAppSettings panel_appSettings;
    [SerializeField] private PanelEditSet panel_editSet;
    [SerializeField] private BasePanel panel_searchTerms;
    [SerializeField] private BasePanel panel_chooseSet;
    [SerializeField] private PanelStudyFlashcards panel_studyFlashcards;

    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ================================================================
    //  Start / Destroy
    // ================================================================
    void Start() {
        // Open the last panel that was open!
        PanelTypes lastPanelOpenType = (PanelTypes) SaveStorage.GetInt(SaveKeys.LastPanelOpen);
        string lastSetName = SaveStorage.GetString(SaveKeys.LastStudySetOpenName);
        StudySet lastSet = dm.library.GetSetByName(lastSetName);
        if (lastSet != null && lastPanelOpenType == PanelTypes.EditSet) {
            OpenPanel_EditSet(lastSet);
        }
        else if (lastSet != null && lastPanelOpenType == PanelTypes.StudyFlashcards) {
            OpenPanel_StudyFlashcards(lastSet);
        }
        else {
            OpenPanel_ChooseSet();
        }

        // Add event listeners.
        GameManagers.Instance.EventManager.OpenPanelEditSetEvent += OpenPanel_EditSet;
        GameManagers.Instance.EventManager.OpenPanelChooseSetEvent += OpenPanel_ChooseSet;
        GameManagers.Instance.EventManager.OpenPanelStudyFlashcardsEvent += OpenPanel_StudyFlashcards;
    }
    private void OnDestroy() {
        // Remove event listeners.
        GameManagers.Instance.EventManager.OpenPanelEditSetEvent -= OpenPanel_EditSet;
        GameManagers.Instance.EventManager.OpenPanelChooseSetEvent -= OpenPanel_ChooseSet;
        GameManagers.Instance.EventManager.OpenPanelStudyFlashcardsEvent -= OpenPanel_StudyFlashcards;
    }


    // ================================================================
    //  Doers
    // ================================================================
    public void ShowPanel(BasePanel _panel) {
        panel_appSettings.SetVisibility(false);
        panel_editSet.SetVisibility(false);
        panel_searchTerms.SetVisibility(false);
        panel_chooseSet.SetVisibility(false);
        panel_studyFlashcards.SetVisibility(false);

        _panel.SetVisibility(true);
        bottomTabBar.UpdateTabHighlighted(_panel.MyPanelType);

        // Save this was the last panel opened!
        SaveStorage.SetInt(SaveKeys.LastPanelOpen, _panel.MyPanelType.GetHashCode());
    }


    // ================================================================
    //  Events
    // ================================================================
    public void OpenPanel_AppSettings() { ShowPanel(panel_appSettings); }
    public void OpenPanel_EditSet() { ShowPanel(panel_editSet); }
    public void OpenPanel_SearchTerms() { ShowPanel(panel_searchTerms); }
    public void OpenPanel_ChooseSet() { ShowPanel(panel_chooseSet); }
    public void OpenPanel_StudyFlashcards() { ShowPanel(panel_studyFlashcards); }
    public void OpenPanel_EditSet(StudySet set) {
        dm.SetCurrSet(set);
        OpenPanel_EditSet();
    }
    public void OpenPanel_StudyFlashcards(StudySet set) {
        dm.SetCurrSet(set);
        OpenPanel_StudyFlashcards();
    }



#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        if (Application.isPlaying)
            SceneHelper.ReloadScene();
    }
#endif



    // ================================================================
    //  Update
    // ================================================================
    void Update() {
        // Key Inputs
        if (Input.GetKeyDown(KeyCode.Delete)) dm.ClearAllSaveData();
        // CTRL + ___
        if (Input.GetKey(KeyCode.LeftControl)) {
            if (Input.GetKeyDown(KeyCode.R)) {
                dm.ReplaceAllStudySetsWithPremadeHardcodedOnes();
                SceneHelper.ReloadScene();
            }
        }
    }



}
