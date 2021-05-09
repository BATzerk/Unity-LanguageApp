using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TermEditableTile : MonoBehaviour
{
    // Components
    [SerializeField] private Button b_options;
    [SerializeField] private Button b_playClip;
    [SerializeField] private Button b_editMySet;
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_deleteConfirmation;
    [SerializeField] private RectTransform rt_options;
    [SerializeField] private TextMeshProUGUI t_myNumber;
    [SerializeField] private TextMeshProUGUI t_debugInfo;
    [SerializeField] private TextMeshProUGUI t_setName;
    //[SerializeField] private TextMeshProUGUI t_native;
    //[SerializeField] private TextMeshProUGUI t_foreign;
    //[SerializeField] private TextMeshProUGUI t_phonetic;
    [SerializeField] private TMP_InputField if_native;
    [SerializeField] private TMP_InputField if_foreign;
    [SerializeField] private TMP_InputField if_phonetic;
    [SerializeField] private VerticalLayoutGroup vertLayoutGroup;
    // References
    private Term myTerm;


    // ================================================================
    //  Start / Destroy
    // ================================================================
    private void Start() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent += RefreshVisuals;
    }
    private void OnDestroy() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent -= RefreshVisuals;
    }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(bool isPanelEditSet, RectTransform tf_parent) {
        GameUtils.ParentAndReset(gameObject, tf_parent);
        // In EditSet panel?
        if (isPanelEditSet) {
            b_editMySet.gameObject.SetActive(false);
        }
        // Ah! I'm a search editable! Change my visuals slightly.
        else {
            t_myNumber.gameObject.SetActive(false);
            t_debugInfo.gameObject.SetActive(false);

            b_editMySet.gameObject.SetActive(true);
        }
    }

    public void OpenKeyboardForNativeField() {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(if_native.gameObject, null);
        //if_native.OnPointerClick(null);
    }


    // ----------------------------------------------------------------
    //  Update Visuals
    // ----------------------------------------------------------------
    public void SetMyTerm(int myIndex, Term myTerm) {
        this.myTerm = myTerm;
        t_myNumber.text = (myIndex + 1).ToString();
        t_setName.text = myTerm.mySet.name;

        RefreshVisuals();
    }
    private void RefreshVisuals() {
        // Update visuals
        t_debugInfo.text = "Y:" + myTerm.totalYeses;// + "\nN:" + myTerm.totalNos;
        if_native.text = myTerm.native;
        if_foreign.text = myTerm.foreign;
        if_phonetic.text = myTerm.phonetic;
        //Hack_RefreshSize();
        //Invoke("Hack_RefreshSize", 1.1f);
        b_playClip.gameObject.SetActive(myTerm.HasAudio0());
        HideOptions();
        HideDeleteConfirmation();
        StartCoroutine(RefreshLayoutCoroutine());
    }
    private IEnumerator RefreshLayoutCoroutine() {
        yield return null; // skip a frame. Wait until canvases are done updating.
        //Canvas.ForceUpdateCanvases();// HACK Temp here.
        vertLayoutGroup.CalculateLayoutInputVertical();
        vertLayoutGroup.SetLayoutVertical();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(myRectTransform);
        float groupHeight = vertLayoutGroup.preferredHeight;
        //if (groupHeight == 0) { // Don't have a preferred height yet? No worries; invoke this again until we do!
        //    Invoke("Hack_RefreshSize", 0.1f);
        //}
        //else { // We've got a proper height. Apply it!
            myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, groupHeight + 20);// Mathf.Abs(rt_if_phonetic.anchoredPosition.y)+rt_if_phonetic.rect.size.y);
        //}
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnFinishedEditingAnyField() {
        // Update my term by my texts.
        myTerm.native = if_native.text;
        myTerm.foreign = if_foreign.text;
        myTerm.phonetic = if_phonetic.text;
        // The moment we're done editing a field, save the ENTIRE library again!
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        // Refresh our visuals now!
        RefreshVisuals();
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
    public void OnClickEditMySet() {
        GameManagers.Instance.EventManager.OpenPanelEditSet(myTerm.mySet);
    }
    public void OnClickPlayClip() {
        GameManagers.Instance.EventManager.PlayTermClip(myTerm);
    }
    public void OnClickPreDelete() {
        rt_deleteConfirmation.gameObject.SetActive(true);
    }
    public void OnClickDeleteConfirmed() {
        myTerm.mySet.RemoveTerm(myTerm);
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        GameManagers.Instance.EventManager.OnAnySetContentsChanged();
    }
    public void HideOptions() {
        rt_options.gameObject.SetActive(false);
        rt_deleteConfirmation.gameObject.SetActive(false); // hide this one too, just in case.
        //RefreshVisuals();
    }
    public void HideDeleteConfirmation() {
        rt_deleteConfirmation.gameObject.SetActive(false);
    }
}
