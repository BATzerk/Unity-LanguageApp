using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudySetTileView : MonoBehaviour
{
    // Components
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_numTermsCardIcon;
    [SerializeField] private TextMeshProUGUI t_name;
    [SerializeField] private TextMeshProUGUI t_numTerms;
    [SerializeField] private TextMeshProUGUI t_avgTimesCompleted;
    [SerializeField] private GameObject go_progressBar;
    [SerializeField] private Image i_progressBarBack;
    [SerializeField] private Image i_progressBarFillRecent;
    [SerializeField] private Image i_progressBarFillYeses;
    // References
    private StudySet mySet;
    private PanelChooseSet myPanel;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(PanelChooseSet myPanel, RectTransform tf_parent) {
        this.myPanel = myPanel;
        GameUtils.ParentAndReset(gameObject, tf_parent);
    }
    public void SetMySet(StudySet mySet) {
        this.mySet = mySet;

        // Update visuals
        t_name.text = mySet.name;
        t_numTerms.text = mySet.NumTotal.ToString();// + " terms";
        t_avgTimesCompleted.text = TextUtils.DecimalPlaces1(mySet.GetAverageTimesCompleted());// numRoundsFinished.ToString();
        go_progressBar.SetActive(mySet.IsInProgress);
        if (mySet.IsInProgress) {
            float barWidth = i_progressBarBack.rectTransform.rect.width;
            float progLocYeses = (mySet.NumTotal-(mySet.pileYesesAndNosG.Count+mySet.pileQueueG.Count)) / (float)mySet.NumTotal;
            float progLocRecent = mySet.NumDone / (float)mySet.NumTotal;
            float yesWidth = barWidth * progLocYeses;
            i_progressBarFillYeses.rectTransform.sizeDelta = new Vector2(yesWidth, i_progressBarFillYeses.rectTransform.sizeDelta.y);
            i_progressBarFillRecent.rectTransform.anchoredPosition = new Vector2(yesWidth, 0);
            i_progressBarFillRecent.rectTransform.sizeDelta = new Vector2(barWidth * progLocRecent, i_progressBarFillRecent.rectTransform.sizeDelta.y);
        }
        rt_numTermsCardIcon.localEulerAngles = new Vector3(0, 0, Random.Range(-5f, 5f));
    }


    //// ----------------------------------------------------------------
    ////  Update Visuals
    //// ----------------------------------------------------------------
    //public void UpdateVisuals() {
    //}


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickedStudyMe() {
        myPanel.OnClickedStudyASet(mySet);
    }
    public void OnClickedEditMe() {
        myPanel.OnClickedEditASet(mySet);
    }
    public void OnClickedMoveMeUp() {
        GameManagers.Instance.DataManager.library.ChangeSetIndexInList(mySet, 1);
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        myPanel.UpdateAllTiles();
    }
}
