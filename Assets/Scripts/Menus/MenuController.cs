using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // References
    [SerializeField] private BasePanel panel_edit;
    [SerializeField] private BasePanel panel_studyChooseSet;
    [SerializeField] private PanelStudyFlashcards panel_studyFlashcards;


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
        panel_edit.SetVisibility(false);
        panel_studyChooseSet.SetVisibility(false);
        panel_studyFlashcards.SetVisibility(false);

        _panel.SetVisibility(true);
    }

    // ================================================================
    //  Events
    // ================================================================
    public void OpenPanel_Edit() { ShowPanel(panel_edit); }
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
        if (Input.GetKeyDown(KeyCode.Delete)) {
            GameManagers.Instance.DataManager.ClearAllSaveData();
        }
    }



}
