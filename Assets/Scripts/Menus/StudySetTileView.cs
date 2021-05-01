using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StudySetTileView : MonoBehaviour
{
    // Components
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private TextMeshProUGUI t_name;
    // References
    private StudySet mySet;
    private PanelStudyChooseSet myPanel;


    public void Initialize(PanelStudyChooseSet myPanel, RectTransform tf_parent, StudySet mySet, float posY)
    {
        this.myPanel = myPanel;
        this.mySet = mySet;
        GameUtils.ParentAndReset(gameObject, tf_parent);
        myRectTransform.anchoredPosition = new Vector2(0, posY);

        // Update visuals
        t_name.text = mySet.name;
    }


    public void OnClickedStudyMe() {
        myPanel.OnClickedStudyASet(mySet);
    }
    public void OnClickedEditMe() {
        myPanel.OnClickedEditASet(mySet);
    }
}
