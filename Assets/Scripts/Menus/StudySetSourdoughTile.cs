using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudySetSourdoughTile : MonoBehaviour {
    // Properties
    [SerializeField] private Button b_refillSourdoughSet;
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_numTermsCardIcon;
    [SerializeField] private TextMeshProUGUI t_name;
    [SerializeField] private TextMeshProUGUI t_numTerms;
    [SerializeField] private TextMeshProUGUI t_countdownValue;
    [SerializeField] private StudySetProgressBar_Dashed progressBar;
    // References
    private StudySet mySet;



    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    private void Start() {
        mySet = GameManagers.Instance.DataManager.library.setSourdough;
        rt_numTermsCardIcon.localEulerAngles = new Vector3(0, 0, Random.Range(-5f, 5f));

        UpdateVisuals();
    }



    // ----------------------------------------------------------------
    //  UpdateVisuals
    // ----------------------------------------------------------------
    public void UpdateVisuals() {
        // Update visuals
        b_refillSourdoughSet.gameObject.SetActive(GameManagers.Instance.DataManager.IsTimeToRefillSourdoughSet());
        t_name.text = mySet.name;
        t_numTerms.text = mySet.NumInCurrentRound.ToString();
        progressBar.UpdateVisuals(mySet);
        //go_progressBar.SetActive(mySet.IsInProgress);
        //if (mySet.IsInProgress) {
        //    float barWidth = i_progressBarBack.rectTransform.rect.width;
        //    float progLocYeses = (mySet.NumTotal - (mySet.pileYesesAndNosG.Count + mySet.pileQueueG.Count)) / (float)mySet.NumTotal;
        //    float progLocRecent = mySet.NumDone / (float)mySet.NumTotal;
        //    float yesWidth = barWidth * progLocYeses;
        //    i_progressBarFillYeses.rectTransform.sizeDelta = new Vector2(yesWidth, i_progressBarFillYeses.rectTransform.sizeDelta.y);
        //    i_progressBarFillRecent.rectTransform.anchoredPosition = new Vector2(yesWidth, 0);
        //    i_progressBarFillRecent.rectTransform.sizeDelta = new Vector2(barWidth * progLocRecent, i_progressBarFillRecent.rectTransform.sizeDelta.y);
        //}

        // Update countdown!
        float hoursLeft = GameManagers.Instance.DataManager.GetHoursUntilNextSourdoughRefill();
        //hoursLeft = Mathf.Clamp(hoursLeft, -999, 999); // just in case
        hoursLeft = Mathf.Ceil(hoursLeft*10f) / 10f; // cap me at only 1 decimal place.
        t_countdownValue.text = hoursLeft + "h";
    }


    private void Update() {
        // Refresh visuals every 60 frames.
        if (Time.frameCount%60 == 0) {
            UpdateVisuals();
        }
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickedStudyMe() {
        GameManagers.Instance.EventManager.OpenPanel_StudyFlashcards(mySet);
    }
    public void OnClick_RefillSourdoughSet() {
        GameManagers.Instance.DataManager.RefillSourdoughSet();
        UpdateVisuals();
    }
    //public void OnClickedEditMe() {
    //    myPanel.OnClickedEditASet(mySet);
    //}
}
