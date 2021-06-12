using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//public struct ForeignLanguage {
//    public 
//}

public class PopupSetCurrForeignLanguage : BasePopup
{
    // Components


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    private void Start() {
        Hide(); // Start off hidden, of course.

        //GameManagers.Instance.EventManager.ShowPopup_MoveTermEvent += Show;
    }
    //private void OnDestroy() {
    //    GameManagers.Instance.EventManager.ShowPopup_MoveTermEvent -= Show;
    //}

    // ----------------------------------------------------------------
    //  Hide / Show
    // ----------------------------------------------------------------
    public void Hide() {
        this.gameObject.SetActive(false);
    }
    public void Show() {
        this.gameObject.SetActive(true);
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClick_SetCurrForeignLanguage(string languageCode) {
        // Update the code!
        SettingsManager.Instance.SetCurrForeignCode(languageCode);
        GameManagers.Instance.DataManager.ReloadStudySetLibrary();
        // Hide me and open up the ChooseSetPanel
        Hide();
        GameManagers.Instance.EventManager.OpenPanelChooseSet();
    }



}
