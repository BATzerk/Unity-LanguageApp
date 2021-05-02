using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelStudyFlashcards : BasePanel {
    // Components
    [SerializeField] private Button b_undo;
    [SerializeField] private CardView currCardView;
    [SerializeField] private Image i_progressBarBack;
    [SerializeField] private Image i_progressBarFill;
    [SerializeField] private MoveTermPopup moveTermPopup;
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
    void Start() { }
    public void OpenSet(StudySet currStudySet) {
        this.currStudySet = currStudySet;

        // Not in progress? Go ahead and reset the deck.
        if (!currStudySet.IsInProgress) {
            currStudySet.ShuffleAndRestartDeck();
        }

        // Start us off, boi.
        moveTermPopup.Hide();
        RefreshCardVisuals();
    }


    // ================================================================
    //  Update Visuals
    // ================================================================
    private void UpdateUndoButtonInteractable() {
        b_undo.interactable = currStudySet.pileYesesAndNos.Count > 0;
    }
    public void RefreshCardVisuals() {
        // Update progress visuals
        int numDone = currStudySet.NumDone;
        int numInRound = currStudySet.NumInCurrentRound;
        t_progress.text = Mathf.Min(numInRound, numDone+1) + " / " + numInRound;
        float barWidth = i_progressBarBack.rectTransform.rect.width;
        float progressLoc = numDone / (float)numInRound;
        i_progressBarFill.rectTransform.sizeDelta = new Vector2(barWidth * progressLoc, i_progressBarFill.rectTransform.sizeDelta.y);

        // We've finished the set??
        if (currStudySet.NumDone >= currStudySet.NumInCurrentRound) {
            //int numUnderstood = currStudySet.NumTotal - currStudySet.pileNo.Count;
            int numNewYeses = currStudySet.pileYes.Count;
            int numRemaining = currStudySet.pileNo.Count;
            t_finishedHeader.text = "Round complete!\n\n" + "learned " + numNewYeses + " new ones!\n" + numRemaining + " remaining";
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
