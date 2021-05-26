using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupTermOptions : MonoBehaviour {
    // Components
    [SerializeField] GameObject go_deleteConfirmation;
    [SerializeField] TextMeshProUGUI t_foreignHeader;
    [SerializeField] TextMeshProUGUI t_debugStats;
    [SerializeField] TMP_InputField if_native;
    [SerializeField] TMP_InputField if_foreign;
    [SerializeField] TMP_InputField if_phonetic;
    // References
    [SerializeField] MoveTermPopup moveTermPopup;
    [SerializeField] SubPopupRecordAudioClip recordAudioClipSP;
    private Term currTerm;


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    void Start() {
        // Start closed.
        ClosePopup();

        // Add event listeners
        GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent += OpenPopup;
        GameManagers.Instance.EventManager.CloseTermOptionsPopupEvent += ClosePopup;
    }
    private void OnDestroy() {
        // Remove event listeners
        GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent -= OpenPopup;
        GameManagers.Instance.EventManager.CloseTermOptionsPopupEvent -= ClosePopup;
    }

    // ----------------------------------------------------------------
    //  Open / Close
    // ----------------------------------------------------------------
    public void ClosePopup() {
        recordAudioClipSP.OnOwnerClose();
        this.gameObject.SetActive(false);
    }
    public void OpenPopup(Term currTerm) {
        this.currTerm = currTerm;
        this.gameObject.SetActive(true);
        recordAudioClipSP.OnOwnerOpen(currTerm);

        UpdateTextFields();
        HideDeleteConfirmation();
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void UpdateTextFields() {
        t_foreignHeader.text = currTerm.foreign;//"\"" +  + "\""
        if_native.text = currTerm.native;
        if_foreign.text = currTerm.foreign;
        if_phonetic.text = currTerm.phonetic;
        string debugStr = "";
        debugStr += "mySet: " + currTerm.mySet.name + "\n";
        debugStr += "nos: " + currTerm.totalNos + ", yeses: " + currTerm.totalYeses + "\n";
        debugStr += "nSDStays: " + currTerm.nSDStays + ", nSDLeaves: " + currTerm.nSDLeaves + "\n";
        debugStr += "myGuid:     " + currTerm.myGuid + "\n";
        debugStr += "audio0Guid: " + currTerm.audio0Guid;
        t_debugStats.text = debugStr;
    }

    public void OnClick_TopBanner() {
        GameManagers.Instance.EventManager.SpeakTTSForeign(currTerm);
    }
    public void OnClick_MoveTermButton() {
        moveTermPopup.Show(currTerm);
    }
    public void OnEndEditAnyTextField() {
        // Update my actual term now!
        currTerm.native = if_native.text;
        currTerm.foreign = if_foreign.text;
        currTerm.phonetic = if_phonetic.text;
        UpdateTextFields();
        // Save 'em, Joe. :)
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        // Refresh all my texts now.
        GameManagers.Instance.EventManager.OnAnySetContentsChanged();
    }

    public void OnClickPreDelete() {
        go_deleteConfirmation.SetActive(true);
    }
    public void HideDeleteConfirmation() {
        go_deleteConfirmation.SetActive(false);
    }
    public void OnClickDeleteConfirmed() {
        GameManagers.Instance.DataManager.library.RemoveTerm(currTerm);
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        GameManagers.Instance.EventManager.OnAnySetContentsChanged();
        ClosePopup();
    }


}
