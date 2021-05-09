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
    [SerializeField] private Button b_studyAgain;
    [SerializeField] private Button b_toggleTTS;
    [SerializeField] private CardView currCardView;
    [SerializeField] private Image i_progressBarBack;
    [SerializeField] private Image i_progressBarFill;
    [SerializeField] private TextMeshProUGUI t_progress;
    [SerializeField] private TextMeshProUGUI t_ttsSpeed;
    [SerializeField] private TextMeshProUGUI t_finishedInformation;
    [SerializeField] private RectTransform rt_setFinished;
    [SerializeField] private RectTransform rt_setInProgress;
    //private List<CardView> cardViews;
    // References
    [SerializeField] private Sprite s_ttsButtonOn;
    //[SerializeField] private Sprite s_ttsButtonOnSlow;
    [SerializeField] private Sprite s_ttsButtonOff;
    private StudySet currStudySet;
    // Properties
    private bool isTTSOn;
    private float ttsSpeechRate;



    // Getters
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
        // Load TTS properties!
        isTTSOn = SaveStorage.GetBool(SaveKeys.IsTTSOn);
        ttsSpeechRate = SaveStorage.GetFloat(SaveKeys.TTSSpeechRate);
        UpdateTTSButtons();

        // Add event listeners.
        eventManager.SetContentsChangedEvent += RefreshCardVisuals;
    }
    private void OnDestroy() {
        // Remove event listeners.
        eventManager.SetContentsChangedEvent -= RefreshCardVisuals;
    }
    public void OpenSet(StudySet currStudySet) {
        this.currStudySet = currStudySet;
        SaveStorage.SetString(SaveKeys.LastStudySetOpenName, currStudySet.name);

        // Not in progress? Go ahead and reset the deck.
        if (!currStudySet.IsInProgress) {
            currStudySet.ShuffleAndRestartDeck();
        }

        // Start us off, boi.
        RefreshCardVisuals();
    }


    // ================================================================
    //  Update Visuals
    // ================================================================
    private void UpdateUndoButtonInteractable() {
        b_undo.interactable = currStudySet.pileYesesAndNosG.Count > 0;
    }
    private void RefreshCardVisuals() {
        if (currStudySet == null) { return; } // No current StudySet? We're not in flashcard mode! Do nothin'.
        // Update progress visuals
        int numDone = currStudySet.NumDone;
        int numInRound = currStudySet.NumInCurrentRound;
        t_progress.text = Mathf.Min(numInRound, numDone+1) + " / " + numInRound;
        float barWidth = i_progressBarBack.rectTransform.rect.width;
        float progressLoc = numDone / (float)numInRound;
        i_progressBarFill.rectTransform.sizeDelta = new Vector2(barWidth * progressLoc, i_progressBarFill.rectTransform.sizeDelta.y);

        // We've finished the set??
        if (currStudySet.NumDone >= currStudySet.NumInCurrentRound) {
            t_finishedInformation.text = GetRoundCompleteText(currStudySet);
            b_studyAgain.gameObject.SetActive(currStudySet.pileNoG.Count > 0); // only show "next round" button if there are cards to HAVE a next round with.
            rt_setFinished.gameObject.SetActive(true);
            rt_setInProgress.gameObject.SetActive(false);
        }
        // There are still cards left. Show the next!
        else {
            rt_setFinished.gameObject.SetActive(false);
            rt_setInProgress.gameObject.SetActive(true);
            currCardView.SetMyTerm(currStudySet.GetCurrTerm());
        }

        UpdateUndoButtonInteractable();
    }

    // ================================================================
    //  Events
    // ================================================================
    public void OnClickYes() {
        currStudySet.OnClickCurrTermYes();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
    }
    public void OnClickNo() {
        currStudySet.OnClickCurrTermNo();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
    }
    public void OnClickUndo() {
        currStudySet.RewindOneCard();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
    }
    public void OnClickShuffleAndReset() {
        currStudySet.ShuffleAndRestartDeck();
        dm.SaveStudySetLibrary();
        RefreshCardVisuals();
    }
    public void OnClickStudyAgain() {
        currStudySet.RestartNewRound();
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
        //Debug.Log("GetAllAvailableLanguages: ");
        //foreach (string str in TTS.GetAllAvailableLanguages()) {
        //    Debug.Log(str);
        //}
        ////SpeechUtteranceParameters parameters = new SpeechUtteranceParameters();
        //ISpeechSynthesisVoice voice = TTS.GetVoiceForLanguage("en");
        //Debug.Log("English voice: " + voice);
        //TTS.Speak(MyTerm.native, voice);
    }
    public void OnShowSideForeign() {
        if (isTTSOn) {
            SpeakTTSForeign();
        }
    }
    public void SpeakTTSForeign() {
        SpeechUtteranceParameters parameters = new SpeechUtteranceParameters();
        parameters.SpeechRate = ttsSpeechRate;
        ISpeechSynthesisVoice voice = TTS.GetVoiceForLanguage("da");
        TTS.Speak(currCardView.MyTerm.foreign, voice);
    }

    public void OnClickToggleTTS() {
        isTTSOn = !isTTSOn;
        UpdateTTSButtons();
        // Save the values!
        SaveStorage.SetBool(SaveKeys.IsTTSOn, isTTSOn);
    }
    public void OnClickCycleTTSSpeed() {
        if (ttsSpeechRate >= 1) {
            ttsSpeechRate = 0.75f;
        }
        else if (ttsSpeechRate >= 0.75f) {
            ttsSpeechRate = 0.5f;
        }
        else {
            ttsSpeechRate = 1;
        }
        UpdateTTSButtons();
        SaveStorage.SetFloat(SaveKeys.TTSSpeechRate, ttsSpeechRate);
    }
    private void UpdateTTSButtons() {
        t_ttsSpeed.text = ttsSpeechRate + "x";
        b_toggleTTS.image.sprite = isTTSOn ? s_ttsButtonOn : s_ttsButtonOff;
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
