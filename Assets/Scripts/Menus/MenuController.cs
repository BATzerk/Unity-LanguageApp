using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // References
    [SerializeField] private PanelEditSet panel_editSet;
    [SerializeField] private BasePanel panel_searchTerms;
    [SerializeField] private BasePanel panel_studyChooseSet;
    [SerializeField] private PanelStudyFlashcards panel_studyFlashcards;

    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ================================================================
    //  Start
    // ================================================================
    void Start() {
        // TODO: Save which panel was open last. Open that one!
        //ShowPanel(panel_studyChooseSet);
        OpenPanel_EditSet(dm.library.sets[0]);
    }


    // ================================================================
    //  Doers
    // ================================================================
    public void ShowPanel(BasePanel _panel)
    {
        panel_editSet.SetVisibility(false);
        panel_searchTerms.SetVisibility(false);
        panel_studyChooseSet.SetVisibility(false);
        panel_studyFlashcards.SetVisibility(false);

        _panel.SetVisibility(true);
    }

    // ================================================================
    //  Events
    // ================================================================
    public void OpenPanel_EditSet(StudySet set) { ShowPanel(panel_editSet); panel_editSet.OpenSet(set); }
    public void OpenPanel_SearchTerms() { ShowPanel(panel_searchTerms); }
    public void OpenPanel_StudyChooseSet() { ShowPanel(panel_studyChooseSet); }
    public void OpenPanel_StudyFlashcards(StudySet set) { ShowPanel(panel_studyFlashcards); panel_studyFlashcards.OpenSet(set); }

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

        //Debug.Log("0'th's set: " + dm.library.sets[0].allTerms[0].mySet);


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
