using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudySetSourdoughTile : MonoBehaviour {
    // Properties
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_numTermsCardIcon;
    [SerializeField] private TextMeshProUGUI t_name;
    [SerializeField] private TextMeshProUGUI t_numTerms;
    [SerializeField] private TextMeshProUGUI t_countdownValue;
    [SerializeField] private GameObject go_progressBar;
    [SerializeField] private Image i_progressBarBack;
    [SerializeField] private Image i_progressBarFillRecent;
    [SerializeField] private Image i_progressBarFillYeses;
    // References
    private StudySet mySet;
    private PanelStudyChooseSet myPanel;


    // ----------------------------------------------------------------
    //  UpdateVisuals
    // ----------------------------------------------------------------
    public void UpdateVisuals(PanelStudyChooseSet myPanel, StudySet mySet) {
        this.myPanel = myPanel;
        this.mySet = mySet;

        // Update visuals
        t_name.text = mySet.name;
        t_numTerms.text = mySet.NumInCurrentRound.ToString();
        go_progressBar.SetActive(mySet.IsInProgress);
        if (mySet.IsInProgress) {
            float barWidth = i_progressBarBack.rectTransform.rect.width;
            float progLocYeses = (mySet.NumTotal - (mySet.pileYesesAndNosG.Count + mySet.pileQueueG.Count)) / (float)mySet.NumTotal;
            float progLocRecent = mySet.NumDone / (float)mySet.NumTotal;
            float yesWidth = barWidth * progLocYeses;
            i_progressBarFillYeses.rectTransform.sizeDelta = new Vector2(yesWidth, i_progressBarFillYeses.rectTransform.sizeDelta.y);
            i_progressBarFillRecent.rectTransform.anchoredPosition = new Vector2(yesWidth, 0);
            i_progressBarFillRecent.rectTransform.sizeDelta = new Vector2(barWidth * progLocRecent, i_progressBarFillRecent.rectTransform.sizeDelta.y);
        }
        rt_numTermsCardIcon.localEulerAngles = new Vector3(0, 0, Random.Range(-5f, 5f));

        // Update countdown!
        float hoursLeft = GameManagers.Instance.DataManager.GetHoursUntilNextSourdoughLoaf();
        hoursLeft = Mathf.Ceil(hoursLeft*10f) / 10f; // cap me at only 1 decimal place.
        t_countdownValue.text = hoursLeft + "h";
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickedStudyMe() {
        myPanel.OnClickedStudyASet(mySet);
    }
    //public void OnClickedEditMe() {
    //    myPanel.OnClickedEditASet(mySet);
    //}
}
