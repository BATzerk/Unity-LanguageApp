using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudySetTileView : MonoBehaviour
{
    // Components
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private TextMeshProUGUI t_name;
    [SerializeField] private TextMeshProUGUI t_numTerms;
    [SerializeField] private GameObject go_progressBar;
    [SerializeField] private Image i_progressBarFill;
    [SerializeField] private Image i_progressBarBack;
    // References
    private StudySet mySet;
    private PanelStudyChooseSet myPanel;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(PanelStudyChooseSet myPanel, RectTransform tf_parent, StudySet mySet) {
        this.myPanel = myPanel;
        this.mySet = mySet;
        GameUtils.ParentAndReset(gameObject, tf_parent);

        UpdateVisuals();
    }


    // ----------------------------------------------------------------
    //  Update Visuals
    // ----------------------------------------------------------------
    public void UpdateVisuals() {
        // Update visuals
        t_name.text = mySet.name;
        t_numTerms.text = mySet.NumTotal.ToString() + " TERMS";
        //t_progress.text = mySet.IsInProgress ?
        //    ("progress: " + (int)(mySet.NumDone/(float)mySet.NumInCurrentRound)*100 + "%")
        //    : ""
        //;
        go_progressBar.SetActive(mySet.IsInProgress);
        if (mySet.IsInProgress) {
            float barWidth = i_progressBarBack.rectTransform.rect.width;
            float progressLoc = mySet.NumDone / (float)mySet.NumInCurrentRound;
            i_progressBarFill.rectTransform.sizeDelta = new Vector2(barWidth*progressLoc, i_progressBarFill.rectTransform.sizeDelta.y);
        }
    }


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
