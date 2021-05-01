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
    private StudySet myStudySet;
    private PanelStudyChooseSet myPanel;


    public void Initialize(PanelStudyChooseSet myPanel, RectTransform tf_parent, StudySet myStudySet, float posY)
    {
        this.myPanel = myPanel;
        this.myStudySet = myStudySet;
        GameUtils.ParentAndReset(gameObject, tf_parent);
        myRectTransform.anchoredPosition = new Vector2(0, posY);

        // Update visuals
        t_name.text = myStudySet.name;
    }


    public void OnClicked()
    {
        myPanel.OnClickedTileView(myStudySet);
    }
}
