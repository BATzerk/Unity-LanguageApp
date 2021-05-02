using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour {
    // Components
    [SerializeField] private RectTransform rt_options;
    [SerializeField] private RectTransform rt_sideNative;
    [SerializeField] private RectTransform rt_sideForeign;
    [SerializeField] TextMeshProUGUI t_native;
    [SerializeField] TextMeshProUGUI t_foreign;
    [SerializeField] TextMeshProUGUI t_phonetic;
    [SerializeField] TMP_InputField if_native;
    [SerializeField] TMP_InputField if_foreign;
    [SerializeField] TMP_InputField if_phonetic;
    // References
    [SerializeField] private PanelStudyFlashcards myPanel;
    public Term MyTerm { get; private set; }
    // Properties
    private bool isNativeSide; // if FALSE, we're showing the foreign side.


    // ----------------------------------------------------------------
    //  Setting Visuals
    // ----------------------------------------------------------------
    public void SetMyTerm(Term term) {
        this.MyTerm = term;

        // Update visuals!
        UpdateTextFieldsFromTerm();
        HideOptions();
        ShowSideNative(true);
    }

    private void UpdateTextFieldsFromTerm() {
        t_native.text = MyTerm.english;
        t_foreign.text = MyTerm.danish;
        t_phonetic.text = MyTerm.phonetic;
        if_native.text = MyTerm.english;
        if_foreign.text = MyTerm.danish;
        if_phonetic.text = MyTerm.phonetic;
    }

    void ShowSideNative(bool doAnimate=true) {
        isNativeSide = true;
        rt_sideNative.gameObject.SetActive(isNativeSide);
        rt_sideForeign.gameObject.SetActive(!isNativeSide);
    }
    void ShowSideForeign(bool doAnimate=true) {
        isNativeSide = false;
        rt_sideNative.gameObject.SetActive(isNativeSide);
        rt_sideForeign.gameObject.SetActive(!isNativeSide);
    }
    public void FlipCard() {
        if (isNativeSide) ShowSideForeign();
        else ShowSideNative();
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickCardFace() {
        FlipCard();
    }
    public void HideOptions() {
        rt_options.gameObject.SetActive(false);
    }
    public void ShowOptions() {
        rt_options.gameObject.SetActive(true);
    }
    public void OnEndEditAnyTextField() {
        // Update my actual term now!
        MyTerm.english = if_native.text;
        MyTerm.danish = if_foreign.text;
        MyTerm.phonetic = if_phonetic.text;
        // Save 'em, Joe. :)
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        // Refresh all my texts now.
        UpdateTextFieldsFromTerm();
    }




}
