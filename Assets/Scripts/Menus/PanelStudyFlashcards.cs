using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GoodEnough.TextToSpeech;

public class PanelStudyFlashcards : BasePanel {
    // Components
    [SerializeField] private Button b_undo;
    [SerializeField] private Button b_resetSet;
    [SerializeField] private Button b_studyAgain;
    [SerializeField] private Button b_toggleTTS;
    [SerializeField] private CardView currCardView;
    [SerializeField] private Image i_progressBarBack;
    [SerializeField] private Image i_progressBarFillRecent;
    [SerializeField] private Image i_progressBarFillYeses;
    [SerializeField] private TextMeshProUGUI t_setName;
    [SerializeField] private TextMeshProUGUI t_progress;
    [SerializeField] private TextMeshProUGUI t_ttsSpeed;
    [SerializeField] private TextMeshProUGUI t_finishedInformation;
    [SerializeField] private RectTransform rt_setFinished;
    [SerializeField] private RectTransform rt_setInProgress;
    //private List<CardView> cardViews;
    // References
    [SerializeField] private Sprite s_ttsButtonOn;
    [SerializeField] private Sprite s_ttsButtonOff;
    private StudySet currSet;



    // Getters
    private SettingsManager sm { get { return GameManagers.Instance.SettingsManager; } }
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
        eventManager.SetContentsChangedEvent += RefreshCardVisuals;
    }
    private void OnDestroy() {
        // Remove event listeners.
        eventManager.SetContentsChangedEvent -= RefreshCardVisuals;
    }
    public void OpenSet(StudySet currSet) {
        this.currSet = currSet;
        SaveStorage.SetString(SaveKeys.LastStudySetOpenName, currSet.name);

        t_setName.text = currSet.name;

        // Not in progress? Go ahead and reset the deck.
        if (!currSet.IsInProgress) {
            currSet.ShuffleAndRestartDeck();
        }

        // Start us off, boi.
        RefreshCardVisuals();
    }


    // ================================================================
    //  Update Visuals
    // ================================================================
    private void UpdateUndoButtonInteractable() {
        b_undo.interactable = currSet.pileYesesAndNosG.Count > 0;
    }
    private void RefreshCardVisuals() {
        if (currSet == null) { return; } // No current StudySet? We're not in flashcard mode! Do nothin'.
        // Update progress visuals
        int numDone = currSet.NumDone;
        int numInRound = currSet.NumInCurrentRound;
        t_progress.text = Mathf.Min(numInRound, numDone+1) + " / " + numInRound;
        float barWidth = i_progressBarBack.rectTransform.rect.width;
        float progLocYeses = (currSet.NumTotal - (currSet.pileYesesAndNosG.Count + currSet.pileQueueG.Count)) / (float)currSet.NumTotal;
        float progLocRecent = currSet.NumDone / (float)currSet.NumTotal;
        float yesWidth = barWidth * progLocYeses;
        i_progressBarFillYeses.rectTransform.sizeDelta = new Vector2(yesWidth, i_progressBarFillYeses.rectTransform.sizeDelta.y);
        i_progressBarFillRecent.rectTransform.anchoredPosition = new Vector2(yesWidth, 0);
        i_progressBarFillRecent.rectTransform.sizeDelta = new Vector2(barWidth * progLocRecent, i_progressBarFillRecent.rectTransform.sizeDelta.y);

        // We've finished the set??
        if (currSet.NumDone >= currSet.NumInCurrentRound) {
            t_finishedInformation.text = GetRoundCompleteText(currSet);
            b_resetSet.gameObject.SetActive(!dm.IsSourdoughSet(currSet)); // only show "reset deck" button if it's NOT the Sourdough set.
            b_studyAgain.gameObject.SetActive(currSet.pileNoG.Count > 0); // only show "next round" button if there are cards to HAVE a next round with.
            rt_setFinished.gameObject.SetActive(true);
            rt_setInProgress.gameObject.SetActive(false);
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
        RefreshCardVisuals();
    }
    public void OnClickNo() {
        currSet.OnClickCurrTermNo();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
    }
    public void OnClickUndo() {
        currSet.RewindOneCard();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
    }
    public void OnClickShuffleAndReset() {
        currSet.ShuffleAndRestartDeck();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
    }
    public void OnClickStudyAgain() {
        currSet.RestartNewRound();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
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
        //    SpeakTTSNative();
        //}
    }
    public void OnShowSideForeign() {
        if (sm.IsTTSOn) {
            SpeakTTSForeign();
        }
    }
    public void SpeakTTSNative() {
        if (TTS.IsSpeaking) { return; } // Already talking? Don't queue anything up.
        SpeechUtteranceParameters parameters = new SpeechUtteranceParameters();
        parameters.Voice = TTS.GetVoiceForLanguage("en");
        parameters.PitchMultiplier = UnityEngine.Random.Range(0.3f, 2f);
        parameters.SpeechRate = sm.TTSSpeechRate / 2f; // note: divide by 2: 0.5 is actually normal-speed.
        TTS.Speak(currCardView.MyTerm.native, parameters);
    }
    public void SpeakTTSForeign() {
        if (TTS.IsSpeaking) { return; } // Already talking? Don't queue anything up.
        SpeechUtteranceParameters parameters = new SpeechUtteranceParameters();
        parameters.Voice = TTS.GetVoiceForLanguage("da");
        parameters.PitchMultiplier = UnityEngine.Random.Range(0.9f, 1.1f);
        parameters.SpeechRate = sm.TTSSpeechRate / 2f; // note: divide by 2: 0.5 is actually normal-speed.
        TTS.Speak(currCardView.MyTerm.foreign, parameters);
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
