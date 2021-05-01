using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // References
    [SerializeField] private BasePanel panel_edit=null;
    [SerializeField] private BasePanel panel_studyChooseSet =null;


    // ================================================================
    //  Start
    // ================================================================
    void Start()
    {
        ShowPanel(panel_studyChooseSet );
    }


    // ================================================================
    //  Doers
    // ================================================================
    public void ShowPanel(BasePanel _panel)
    {
        panel_edit.SetVisibility(false);
        panel_studyChooseSet.SetVisibility(false);

        _panel.SetVisibility(true);
    }

    // ================================================================
    //  Events
    // ================================================================
    public void OpenPanel_IBs() { ShowPanel(panel_edit); }
    public void OpenPanel_Today() { ShowPanel(panel_studyChooseSet); }



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
