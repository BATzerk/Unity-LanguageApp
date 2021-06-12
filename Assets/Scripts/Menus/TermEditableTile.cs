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
    [SerializeField] private TextMeshProUGUI t_myNumber;
    [SerializeField] private TextMeshProUGUI t_setName;
    [SerializeField] private TextMeshProUGUI t_foreignAbbr;
    [SerializeField] private TMP_InputField if_native;
    [SerializeField] private TMP_InputField if_foreign;
    [SerializeField] private TMP_InputField if_phonetic;
    [SerializeField] private VerticalLayoutGroup vertLayoutGroup;
    // Properties
    private bool amIFocused;
    // References
    private Term myTerm;


    // ================================================================
    //  Start / Destroy
    // ================================================================
    private void Start() {
        GameManagers.Instance.EventManager.GiveTermTileFocusEvent += OnGiveTermTileFocus;
    }
    private void OnDestroy() {
        GameManagers.Instance.EventManager.GiveTermTileFocusEvent -= OnGiveTermTileFocus;
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
        t_setName.text = myTerm.MySetName();
        t_foreignAbbr.text = SettingsManager.Instance.CurrForeignNameAbbr.ToUpper();

        RefreshVisuals();
    }
    private void RefreshVisuals() {
        // Update visuals
        if_native.text = myTerm.native;
        if_foreign.text = myTerm.foreign;
        if_phonetic.text = myTerm.phonetic;
        UpdatePhoneticFieldEnabled(false); // update phonetic field but DON'T request refreshing the layout (I'm about to do that in 2 lines).
        b_playClip.gameObject.SetActive(myTerm.HasAudio0());
        StartCoroutine(RefreshLayoutCoroutine());
    }
    private IEnumerator RefreshLayoutCoroutine() {
        yield return null; // skip a frame. Wait until canvases are done updating.
        vertLayoutGroup.CalculateLayoutInputVertical();
        vertLayoutGroup.SetLayoutVertical();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(myRectTransform);
        float groupHeight = vertLayoutGroup.preferredHeight;
        // Apply the proper height.
        myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, groupHeight + 20);// Mathf.Abs(rt_if_phonetic.anchoredPosition.y)+rt_if_phonetic.rect.size.y);
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnGiveTermTileFocus(Term _term) {
        amIFocused = _term == myTerm;
        UpdatePhoneticFieldEnabled(true);
    }
    private void UpdatePhoneticFieldEnabled(bool doRefreshLayout) {
        bool doShowPhonetic = amIFocused || !string.IsNullOrWhiteSpace(myTerm.phonetic); // ONLY show phonetic field if I'm focused, OR if there's phonetic text!
        // If I should show or hide it, do!
        if (if_phonetic.gameObject.activeSelf != doShowPhonetic) {
            if_phonetic.gameObject.SetActive(doShowPhonetic);
            if (doRefreshLayout) {
                StartCoroutine(RefreshLayoutCoroutine());
            }
        }
    }
    public void OnSelectAnyField() {
        GameManagers.Instance.EventManager.OnGiveTermTileFocus(myTerm);
    }
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
        GameManagers.Instance.EventManager.ShowPopup_TermOptions(myTerm);
        //rt_options.gameObject.SetActive(true);
    }
    public void OnClickEditMySet() {
        //GameManagers.Instance.DataManager.SetCurrSet(myTerm.mySet);
        GameManagers.Instance.EventManager.OpenPanelEditSet(myTerm.mySet);
    }
    public void OnClickPlayClip() {
        GameManagers.Instance.EventManager.PlayTermClip(myTerm);
    }
    //public void HideOptions() {
    //    rt_options.gameObject.SetActive(false);
    //    rt_deleteConfirmation.gameObject.SetActive(false); // hide this one too, just in case.
    //    //RefreshVisuals();
    //}
    //public void HideDeleteConfirmation() {
    //    rt_deleteConfirmation.gameObject.SetActive(false);
    //}
}
