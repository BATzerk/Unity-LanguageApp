using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupAppOptions : MonoBehaviour {


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    void Start() {
        // Start closed.
        ClosePopup();

        //// Add event listeners
        //GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent += OpenPopup;
    }
    //private void OnDestroy() {
    //    // Remove event listeners
    //    GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent -= OpenPopup;
    //}

    // ----------------------------------------------------------------
    //  Open / Close
    // ----------------------------------------------------------------
    public void ClosePopup() {
        this.gameObject.SetActive(false);
    }
    public void OpenPopup() {
        this.gameObject.SetActive(true);
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClick_CopyAllSetsToClipboard() {
        GameManagers.Instance.DataManager.CopyAllSetsToClipboard();
    }
    public void OnClick_ForceRefillSourdoughSet() {
        GameManagers.Instance.DataManager.RefillSourdoughSet();
        SceneHelper.ReloadScene();
    }

}
