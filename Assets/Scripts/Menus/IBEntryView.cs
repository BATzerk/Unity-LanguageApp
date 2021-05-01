using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IBEntryView : MonoBehaviour
{
    // Components
    [SerializeField] private RectTransform myRT = null;
    [SerializeField] private TMP_InputField if_belief = null;
    [SerializeField] private TMP_InputField if_debate = null;
    [SerializeField] private TextMeshProUGUI t_myIndex = null;
    // References
    private PanelStudy myPanel;
    private IBData myData;
    // Properties
    private bool isInitialized=false;

    // ================================================================
    //  Initialize
    // ================================================================
    public void Initialize(PanelStudy _myPanel, IBData _myData)
    {
        this.myData = _myData;
        this.myPanel = _myPanel;

        // Parent me properly
        GameUtils.ParentAndReset(gameObject, myPanel.rt_entriesParent);
        myRT.SetSiblingIndex(myRT.parent.childCount-2);

        // Update texts
        if_belief.text = myData.belief;
        if_debate.text = myData.debate;
        t_myIndex.text = (myData.myIndex + 1).ToString();

        UpdateComponentPositionsWithDelay();

        isInitialized = true;
    }


    public void OnAnyInputFieldChanged()
    {
        if (!isInitialized) { return; } // Skip during initialization.
        myData.belief = if_belief.text;
        myData.debate = if_debate.text;
        PanelStudy.SaveIBData(myData);

        UpdateComponentPositionsWithDelay();
    }
    public void OnAnyInputFieldEndEdit()
    {
        //Debug.Log("SDFjklfsdajkl");
    }


    private void UpdateComponentPositionsWithDelay() { Invoke("UpdateComponentPositions", 0.05f); } // HACK delay.
    private void UpdateComponentPositions()
    {
        RectTransform rt_belief = if_belief.GetComponent<RectTransform>();
        RectTransform rt_debate = if_debate.GetComponent<RectTransform>();
        //rt_belief.ForceUpdateRectTransforms(); Note: Apparently this doesn't do enough to prevent the delay hack? Needs more exploring.
        //rt_debate.ForceUpdateRectTransforms();
        rt_debate.anchoredPosition = new Vector3(rt_debate.anchoredPosition.x, rt_belief.rect.yMin - 12, 0);

        myRT.sizeDelta = new Vector2(myRT.sizeDelta.x, Mathf.Abs(rt_debate.anchoredPosition.y)+rt_debate.rect.height + 20);
    }
    /*
     bbbb asdfkj asjdkf asdjklf asd fasjkldf jsklda fjklasdf jkldsa fjaklsd fjklas fjkldas fjkalsd fjkasdlf jkdsal fjklasd fjkalsd fjkla
    */

}
