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
    [SerializeField] private TextMeshProUGUI t_numRoundsStarted;
    [SerializeField] private GameObject go_progressBar;
    [SerializeField] private Image i_progressBarBack;
    [SerializeField] private Image i_progressBarFill;
    // References
    private StudySet mySet;
    private PanelStudyChooseSet myPanel;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(PanelStudyChooseSet myPanel, RectTransform tf_parent) {
        this.myPanel = myPanel;
        GameUtils.ParentAndReset(gameObject, tf_parent);
    }
    public void SetMySet(StudySet mySet) {
        this.mySet = mySet;

        // Update visuals
        t_name.text = mySet.name;
        t_numTerms.text = mySet.NumTotal.ToString();// + " terms";
        t_numRoundsStarted.text = mySet.numRoundsFinished.ToString();
        go_progressBar.SetActive(mySet.IsInProgress);
        if (mySet.IsInProgress) {
            float barWidth = i_progressBarBack.rectTransform.rect.width;
            float progressLoc = mySet.NumDone / (float)mySet.NumInCurrentRound;
            i_progressBarFill.rectTransform.sizeDelta = new Vector2(barWidth * progressLoc, i_progressBarFill.rectTransform.sizeDelta.y);
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
}
