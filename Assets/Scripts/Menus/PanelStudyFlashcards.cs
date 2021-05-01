using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelStudyFlashcards : BasePanel {
    // Components
    [SerializeField] private CardView currCardView;
    [SerializeField] private TextMeshProUGUI t_progress;
    [SerializeField] private TextMeshProUGUI t_finishedHeader;
    [SerializeField] private RectTransform rt_setFinished;
    [SerializeField] private RectTransform rt_setInProgress;
    // References
    private StudySet currStudySet;



    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ================================================================
    //  Start
    // ================================================================
    void Start() {
    }

    public void OpenSet(StudySet currStudySet) {
        this.currStudySet = currStudySet;

        // Not in progress? Go ahead and reset the deck.
        if (!currStudySet.IsInProgress) {
            currStudySet.ShuffleAndRestartDeck();
        }

        // Start us off, boi.
        RefreshCardVisuals();
    }
    private void RefreshCardVisuals() {
        t_progress.text = currStudySet.NumDone + " / " + currStudySet.NumInCurrentRound;

        // We've finished the set??
        if (currStudySet.NumDone >= currStudySet.NumInCurrentRound) {
            int numUnderstood = currStudySet.NumTotal - currStudySet.pileNo.Count;
            int numRemaining = currStudySet.pileNo.Count;
            t_finishedHeader.text = "Round complete!\n\n" + numUnderstood + " mastered\n" + numRemaining + " remaining";
            rt_setFinished.gameObject.SetActive(true);
            rt_setInProgress.gameObject.SetActive(false);
        }
        // There are still cards left. Show the next!
        else {
            rt_setFinished.gameObject.SetActive(false);
            rt_setInProgress.gameObject.SetActive(true);
            currCardView.SetMyCard(currStudySet.GetCurrCard());
        }
    }

    public void OnClickYes() {
        currStudySet.OnClickCurrCardYes();
        RefreshCardVisuals();
    }
    public void OnClickNo() {
        currStudySet.OnClickCurrCardNo();
        RefreshCardVisuals();
    }
    public void OnClickRewindOne() {
        currStudySet.OnClickRewindOne();
        RefreshCardVisuals();
    }
    public void OnClickShuffleAndReset() {
        currStudySet.ShuffleAndRestartDeck();
        RefreshCardVisuals();
    }
    public void OnClickStudyAgain() {
        currStudySet.RestartNewRound();
        RefreshCardVisuals();
    }





}
