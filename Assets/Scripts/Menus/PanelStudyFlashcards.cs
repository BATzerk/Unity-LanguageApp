using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelStudyFlashcards : BasePanel {
    // Components
    [SerializeField] private Button b_undo;
    [SerializeField] private Button b_studyAgain;
    [SerializeField] private CardView currCardView;
    [SerializeField] private Image i_progressBarBack;
    [SerializeField] private Image i_progressBarFill;
    [SerializeField] private TextMeshProUGUI t_progress;
    [SerializeField] private TextMeshProUGUI t_finishedInformation;
    [SerializeField] private RectTransform rt_setFinished;
    [SerializeField] private RectTransform rt_setInProgress;
    // References
    private StudySet currStudySet;



    // Getters
    private static string GetRoundCompleteText(StudySet set) {
        //int numUnderstood = set.NumTotal - currStudySet.pileNo.Count;
        int numNewYeses = set.pileYes.Count;
        int numRemaining = set.pileNo.Count;
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
        eventManager.SetContentsChangedEvent += RefreshCardVisuals;
    }
    private void OnDestroy() {
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
        b_undo.interactable = currStudySet.pileYesesAndNos.Count > 0;
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
            b_studyAgain.gameObject.SetActive(currStudySet.pileNo.Count > 0); // only show "next round" button if there are cards to HAVE a next round with.
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





}
