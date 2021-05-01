using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour {
    // Components
    [SerializeField] private RectTransform rt_native;
    [SerializeField] private RectTransform rt_foreign;
    [SerializeField] TextMeshProUGUI t_english;
    [SerializeField] TextMeshProUGUI t_danish;
    [SerializeField] TextMeshProUGUI t_phonetic;
    // References
    [SerializeField] private PanelStudyFlashcards myPanel;
    public Term MyTerm { get; private set; }
    // Properties
    private bool isNativeSide; // if FALSE, we're showing the foreign side.


    // ----------------------------------------------------------------
    //  Setting Visuals
    // ----------------------------------------------------------------
    public void SetMyTerm(Term term)
    {
        this.MyTerm = term;

        // Update visuals!
        t_english.text = term.english;
        t_danish.text = term.danish;
        t_phonetic.text = term.phonetic;
        ShowSideNative(true);
    }


    void ShowSideNative(bool doAnimate=true)
    {
        isNativeSide = true;
        rt_native.gameObject.SetActive(isNativeSide);
        rt_foreign.gameObject.SetActive(!isNativeSide);
    }
    void ShowSideForeign(bool doAnimate=true)
    {
        isNativeSide = false;
        rt_native.gameObject.SetActive(isNativeSide);
        rt_foreign.gameObject.SetActive(!isNativeSide);
    }
    void FlipCard() {
        if (isNativeSide) ShowSideForeign();
        else ShowSideNative();
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    //public void OnClickYes() {
    //    myPanel.OnClickCurrCardYes();
    //}
    //public void OnClickNo() {
    //    myPanel.OnClickCurrCardNo();
    //}
    public void OnClickCardFace() {
        FlipCard();
    }




}
