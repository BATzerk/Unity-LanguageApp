using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelStudyFlashcards : BasePanel {
    // Overrides
    override public PanelTypes MyPanelType { get { return PanelTypes.StudyFlashcards; } }
    // Components
    [SerializeField] private Button b_undo;
    [SerializeField] private Button b_resetSet;
    [SerializeField] private Button b_studyAgain;
    [SerializeField] private Button b_makeNewToughiesSet;
    [SerializeField] private Button b_toggleTTS;
    [SerializeField] private CardView currCardView;
    [SerializeField] private StudySetProgressBar_Dashed progressBar;
    [SerializeField] private TextMeshProUGUI t_setName;
    [SerializeField] private TextMeshProUGUI t_progress; //TODO: Cut this.
    [SerializeField] private TextMeshProUGUI t_ttsSpeed;
    [SerializeField] private TextMeshProUGUI t_finishedInformation;
    [SerializeField] private RectTransform rt_setFinished;
    [SerializeField] private RectTransform rt_setInProgress;
    //private List<CardView> cardViews;
    // References
    [SerializeField] private Sprite s_ttsButtonOn;
    [SerializeField] private Sprite s_ttsButtonOff;


    // Getters
    private SettingsManager sm { get { return SettingsManager.Instance; } }
    private static string GetRoundCompleteText(StudySet set) {
        //int numUnderstood = set.NumTotal - currStudySet.pileNo.Count;
        int numNewYeses = set.pileYesG.Count;
        int numRemaining = set.pileNoG.Count;
        string returnStr = "";
        if (numRemaining>0 && numNewYeses > 0) returnStr += "learned " + numNewYeses + " new ones!\n";
        if (numRemaining > 0) returnStr += numRemaining + " remaining\n";
        else returnStr += "\n\nYou got 'em all, woot!";
        return returnStr;
    }



    // ================================================================
    //  Start / Destroy
    // ================================================================
    private void Start() {
        // Update TTS visuals
        UpdateTTSButtons();

        // Add event listeners.
        eventManager.SetContentsChangedEvent += RefreshVisuals;
    }
    private void OnDestroy() {
        // Remove event listeners.
        eventManager.SetContentsChangedEvent -= RefreshVisuals;
    }



    protected override void OnOpened() {
        // Not in progress? Go ahead and reset the deck.
        if (!currSet.IsInProgress) {
            currSet.ShuffleAndRestartDeck();
        }

        // Start us off, boi.
        RefreshVisuals();
    }


    // ================================================================
    //  Update Visuals
    // ================================================================
    private void UpdateUndoButtonInteractable() {
        b_undo.interactable = currSet.pileYesesAndNosG.Count > 0;
    }
    private void RefreshVisuals() {
        if (currSet == null) { return; } // No current StudySet? We're not in flashcard mode! Do nothin'.
        // -- BAR AND TEXTS --
        int numDone = currSet.NumDone;
        int numInRound = currSet.NumInCurrentRound;
        t_progress.text = Mathf.Min(numInRound, numDone + 1) + " / " + numInRound;
        progressBar.UpdateVisuals(currSet);
        t_setName.text = currSet.name;

        // -- FINISHED/UNFINISHED COMPONENTS --
        // We've finished the set??
        if (currSet.NumDone >= currSet.NumInCurrentRound) {
            bool isToughies = currSet == dm.library.setToughies;
            t_finishedInformation.text = GetRoundCompleteText(currSet);
            b_resetSet.gameObject.SetActive(!dm.IsSourdoughSet(currSet)); // only show "reset deck" button if it's NOT the Sourdough set.
            b_studyAgain.gameObject.SetActive(currSet.pileNoG.Count > 0); // only show "next round" button if there are cards to HAVE a next round with.
            rt_setFinished.gameObject.SetActive(true);
            rt_setInProgress.gameObject.SetActive(false);
            b_makeNewToughiesSet.gameObject.SetActive(isToughies);
        }
        // There are still cards left. Show the next!
        else {
            rt_setFinished.gameObject.SetActive(false);
            rt_setInProgress.gameObject.SetActive(true);
            currCardView.SetMyTerm(currSet.GetCurrTerm());
        }

        UpdateUndoButtonInteractable();
    }

    // ================================================================
    //  Events
    // ================================================================
    public void OnClickYes() {
        currSet.OnClickCurrTermYes();
        dm.SaveStudySetLibrary();
        RefreshVisuals();
    }
    public void OnClickNo() {
        currSet.OnClickCurrTermNo();
        dm.SaveStudySetLibrary();
        RefreshVisuals();
    }
    public void OnClickUndo() {
        currSet.RewindOneCard();
        dm.SaveStudySetLibrary();
        RefreshVisuals();
    }
    public void OnClickShuffleAndReset() {
        currSet.ShuffleAndRestartDeck();
        dm.SaveStudySetLibrary();
        RefreshVisuals();
    }
    public void OnClickStudyAgain() {
        currSet.StartNewRound();
        dm.SaveStudySetLibrary();
        RefreshVisuals();
    }
    public void OnClickRemakeToughiesSet() {
        dm.RemakeToughiesSet();
        RefreshVisuals();
    }



    // ================================================================
    //  Update
    // ================================================================
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            currCardView.FlipCard();

        if (Input.GetKeyDown(KeyCode.Z))
            OnClickUndo();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnClickNo();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            OnClickYes();
    }



    public void OnShowSideNative() {
        //if (isTTSOn) {
        //    SpeakTTSNative(currCardView.MyTerm);
        //}
    }
    public void OnShowSideForeign() {
        if (sm.IsTTSOn) {
            GameManagers.Instance.EventManager.SpeakTTSForeign(currCardView.MyTerm);
        }
    }

    public void OnClickCycleTTSSettings() {
        if (!sm.IsTTSOn) { // off to on.
            sm.IsTTSOn = true;
            sm.TTSSpeechRate = 1;
        }
        else if (sm.TTSSpeechRate >= 1) { // on 1x to 0.75x.
            sm.TTSSpeechRate = 0.75f;
        }
        else if (sm.TTSSpeechRate >= 0.75f) { // on 0.75x to 0.5x.
            sm.TTSSpeechRate = 0.5f;
        }
        else { // on 0.5x to off.
            sm.IsTTSOn = false;
        }
        UpdateTTSButtons();
    }
    private void UpdateTTSButtons() {
        t_ttsSpeed.text = sm.IsTTSOn ? (sm.TTSSpeechRate + "x") : "";
        b_toggleTTS.image.sprite = sm.IsTTSOn ? s_ttsButtonOn : s_ttsButtonOff;
        //if (!isTTSOn) {
        //    b_toggleTTS.image.sprite = s_ttsButtonOff;
        //}
        //else if (ttsSpeechRate == 1) {
        //    b_toggleTTS.image.sprite = s_ttsButtonOn;
        //}
        //else {
        //    b_toggleTTS.image.sprite = s_ttsButtonOnSlow;
        //}
    }




 }
