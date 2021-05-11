﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour {
    // Components
    [SerializeField] private Button b_playClip;
    [SerializeField] private GameObject go_swipeBannerNo;
    [SerializeField] private GameObject go_swipeBannerYes;
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_sideNative;
    [SerializeField] private RectTransform rt_sideForeign;
    [SerializeField] TextMeshProUGUI t_native;
    [SerializeField] TextMeshProUGUI t_foreign;
    [SerializeField] TextMeshProUGUI t_phonetic;
    // References
    //private PanelStudyFlashcards myPanel;
    //private TermAudioClipPlayer clipPlayer;
    [SerializeField] private PanelStudyFlashcards myPanel;
    [SerializeField] private TermAudioClipPlayer clipPlayer;
    public Term MyTerm { get; private set; }
    // Properties
    private bool isNativeSide; // if FALSE, we're showing the foreign side.
    private float timeWhenSetTerm; // Time.time when we last called SetMyTerm.
    //private Vector2 targetPos;



    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    void Start() {
        // Add event listeners
        GameManagers.Instance.EventManager.SetContentsChangedEvent += UpdateTextFieldsFromTerm;
    }
    private void OnDestroy() {
        // Remove event listeners
        GameManagers.Instance.EventManager.SetContentsChangedEvent -= UpdateTextFieldsFromTerm;
    }


    // ----------------------------------------------------------------
    //  Setting Visuals
    // ----------------------------------------------------------------
    //public void Initialize(PanelStudyFlashcards myPanel, Transform tf_parent, TermAudioClipPlayer clipPlayer) {
    //    this.myPanel = myPanel;
    //    this.clipPlayer = clipPlayer;
    //    GameUtils.ParentAndReset(this.gameObject, tf_parent);
    //}
    public void SetMyTerm(Term term) {
        this.MyTerm = term;
        timeWhenSetTerm = Time.time;

        // Update visuals!
        UpdateTextFieldsFromTerm();
        ShowSideNative(true);
        b_playClip.gameObject.SetActive(term.HasAudio0());

        // Reset swipiness.
        go_swipeBannerNo.SetActive(false);
        go_swipeBannerYes.SetActive(false);
        myRectTransform.anchoredPosition = posNeutral;
        myRectTransform.localEulerAngles = Vector3.zero;
    }

    private void UpdateTextFieldsFromTerm() {
        if (MyTerm != null) {
            t_native.text = MyTerm.native;
            t_foreign.text = MyTerm.foreign;
            t_phonetic.text = MyTerm.phonetic;
        }
    }

    void ShowSideNative(bool doAnimate=true) {
        isNativeSide = true;
        rt_sideNative.gameObject.SetActive(isNativeSide);
        rt_sideForeign.gameObject.SetActive(!isNativeSide);
        myPanel.OnShowSideNative();
    }
    void ShowSideForeign(bool doAnimate=true) {
        isNativeSide = false;
        rt_sideNative.gameObject.SetActive(isNativeSide);
        rt_sideForeign.gameObject.SetActive(!isNativeSide);
        myPanel.OnShowSideForeign();
    }
    public void FlipCard() {
        if (isNativeSide) ShowSideForeign();
        else ShowSideNative();
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickCardFace() {
        // If we're ARE at neutral pos, and haven't JUST set my term, flip the card!
        if (Vector2.Distance(myRectTransform.anchoredPosition, posNeutral) < 1
            && Time.time>timeWhenSetTerm+0.5f) {
            FlipCard();
        }
    }
    public void OnClickOptions() {
        GameManagers.Instance.EventManager.ShowPopup_TermOptions(MyTerm);
    }
    public void OnClickPlayAudioClip() {
        clipPlayer.PlayTermClip(MyTerm);
    }
    public void OnClickSpeakTTSForeign() {
        myPanel.SpeakTTSForeign();
    }





    // -------- SWIPING --------
    private bool isSwiping;
    private float swipeRotScale; // randomized every time we click down! For flavor.
    private int swipeOutcomeDir; // -1 no; 0 return to center; 1 yes.
    private Vector2 mouseDownPos;
    private Vector2 posNeutral;

    private Vector2 getMousePos() { return new Vector2(Input.mousePosition.x, Input.mousePosition.y); }

    public void OnCursorDown() {
        isSwiping = true;
        mouseDownPos = getMousePos();// myRectTransform.anchoredPosition - getMousePos();
        // Randomize swipeRotScale.
        swipeRotScale = Random.Range(0.02f, 0.05f);
        //if (Random.Range(0, 1f) < 0.5f) swipeRotScale *= -1;
    }
    public void OnCursorUp() {
        isSwiping = false;
        // Far enough to consider this a swipe??
        if (swipeOutcomeDir == -1) {
            myPanel.OnClickNo();
        }
        else if (swipeOutcomeDir == 1) {
            myPanel.OnClickYes();
        }
    }

    private void Update() {
        if (isSwiping) {
            Vector2 posOffset = (getMousePos()-mouseDownPos) / MainCanvas.Canvas.scaleFactor;
            myRectTransform.anchoredPosition = posNeutral + posOffset;
            // Update swipeOutcomeDir!
            if (posOffset.x < -120) swipeOutcomeDir = -1;
            else if (posOffset.x > 120) swipeOutcomeDir = 1;
            else swipeOutcomeDir = 0;
        }
        else {
            myRectTransform.anchoredPosition += (posNeutral-myRectTransform.anchoredPosition) / 4f;
            swipeOutcomeDir = 0;
        }
        float offsetX = posNeutral.x - myRectTransform.anchoredPosition.x;
        // Rotation
        myRectTransform.localEulerAngles = new Vector3(0, 0, offsetX * swipeRotScale);

        // Swipe outcome visuals
        go_swipeBannerNo.SetActive(swipeOutcomeDir == -1);
        go_swipeBannerYes.SetActive(swipeOutcomeDir == 1);
    }




}
