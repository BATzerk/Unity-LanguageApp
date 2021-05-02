using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // References
    [SerializeField] private PanelEditSet panel_editSet;
    [SerializeField] private BasePanel panel_studyChooseSet;
    [SerializeField] private PanelStudyFlashcards panel_studyFlashcards;

    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ================================================================
    //  Start
    // ================================================================
    void Start()
    {
        ShowPanel(panel_studyChooseSet);
    }


    // ================================================================
    //  Doers
    // ================================================================
    public void ShowPanel(BasePanel _panel)
    {
        panel_editSet.SetVisibility(false);
        panel_studyChooseSet.SetVisibility(false);
        panel_studyFlashcards.SetVisibility(false);

        _panel.SetVisibility(true);
    }

    // ================================================================
    //  Events
    // ================================================================
    public void OpenPanel_EditSet(StudySet set) { ShowPanel(panel_editSet); panel_editSet.OpenSet(set); }
    public void OpenPanel_StudyChooseSet() { ShowPanel(panel_studyChooseSet); }
    public void OpenPanel_StudyFlashcards(StudySet set) { ShowPanel(panel_studyFlashcards); panel_studyFlashcards.OpenSet(set); }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        if (Application.isPlaying)
            SceneHelper.ReloadScene();
    }



    // ================================================================
    //  Update
    // ================================================================
    void Update()
    {
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
