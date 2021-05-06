using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TermEditableTile : MonoBehaviour
{
    // Components
    [SerializeField] private Button b_options;
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_deleteConfirmation;
    [SerializeField] private RectTransform rt_options;
    [SerializeField] private TextMeshProUGUI t_myNumber;
    [SerializeField] private TextMeshProUGUI t_debugInfo;
    [SerializeField] private TextMeshProUGUI t_setName;
    [SerializeField] private TMP_InputField if_native;
    [SerializeField] private TMP_InputField if_foreign;
    [SerializeField] private TMP_InputField if_phonetic;
    // References
    private Term myTerm;
    private PanelEditSet myPanel;


    // ================================================================
    //  Start / Destroy
    // ================================================================
    private void Start() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent += RefreshSetNameText;
    }
    private void OnDestroy() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent -= RefreshSetNameText;
    }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(PanelEditSet myPanelEditSet, RectTransform tf_parent) {
        this.myPanel = myPanelEditSet;
        GameUtils.ParentAndReset(gameObject, tf_parent);
        // No panelEditSet? Oh! Then I'm a search editable! Change my visuals slightly.
        if (myPanelEditSet == null) {
            //b_preDelete.gameObject.SetActive(false);
            t_myNumber.gameObject.SetActive(false);
            t_debugInfo.gameObject.SetActive(false);

            t_setName.gameObject.SetActive(true);
        }
        else {
            t_setName.gameObject.SetActive(false);
        }
    }


    // ----------------------------------------------------------------
    //  Update Visuals
    // ----------------------------------------------------------------
    public void UpdateContent(int myIndex, Term myTerm) {
        this.myTerm = myTerm;

        // Update visuals
        t_myNumber.text = (myIndex + 1).ToString();
        t_debugInfo.text = "Y:" + myTerm.totalYeses;// + "\nN:" + myTerm.totalNos;
        if_native.text = myTerm.english;
        if_foreign.text = myTerm.danish;
        if_phonetic.text = myTerm.phonetic;
        t_setName.text = myTerm.mySet.name;
        HideOptions();
        HideDeleteConfirmation();
    }
    private void RefreshSetNameText() {
        t_setName.text = myTerm.mySet.name;
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnFinishedEditingAnyField() {
        // Update my term by my texts.
        myTerm.english = if_native.text;
        myTerm.danish = if_foreign.text;
        myTerm.phonetic = if_phonetic.text;
        // The moment we're done editing a field, save the ENTIRE library again!
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
    }

    public void OnClickOptions() {
        rt_options.gameObject.SetActive(true);
    }
    public void OnClickOpenRecordPopup() {
        GameManagers.Instance.EventManager.OpenRecordPopup(myTerm);
    }
    public void OnClickMoveTerm() {
        GameManagers.Instance.EventManager.ShowMoveTermPopup(myTerm);
        //HideOptions();
    }
    public void OnClickPreDelete() {
        rt_deleteConfirmation.gameObject.SetActive(true);
    }
    public void OnClickDeleteConfirmed() {
        myTerm.mySet.RemoveTerm(myTerm);
        GameManagers.Instance.EventManager.OnAnySetContentsChanged();
    }
    public void HideOptions() {
        rt_options.gameObject.SetActive(false);
        rt_deleteConfirmation.gameObject.SetActive(false); // hide this one too, just in case.
    }
    public void HideDeleteConfirmation() {
        rt_deleteConfirmation.gameObject.SetActive(false);
    }
}
