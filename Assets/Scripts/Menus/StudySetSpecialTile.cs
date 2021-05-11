using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// The tiles for the special StudySets, like Aced, Shelved, etc.
public class StudySetSpecialTile : MonoBehaviour
{
    // Components
    [SerializeField] private RectTransform rt_numTermsCardIcon;
    [SerializeField] private TextMeshProUGUI t_name;
    [SerializeField] private TextMeshProUGUI t_numTerms;
    // References
    private StudySet mySet;
    private PanelChooseSet myPanel;


    // ----------------------------------------------------------------
    //  Update Visuals
    // ----------------------------------------------------------------
    public void UpdateVisuals(PanelChooseSet myPanel, StudySet mySet) {
        this.myPanel = myPanel;
        this.mySet = mySet;

        // Update visuals
        int numTerms = mySet.NumTotal;
        t_name.text = mySet.name;
        t_numTerms.text = numTerms.ToString();// + " terms";
        rt_numTermsCardIcon.localEulerAngles = new Vector3(0, 0, Random.Range(-5f, 5f));
        t_numTerms.gameObject.SetActive(numTerms > 0);
        rt_numTermsCardIcon.gameObject.SetActive(numTerms > 0);
    }




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickedMe() {
        myPanel.OnClickedEditASet(mySet);
    }
    // TEMP! Placeholder until we have more functionality in edit set screen.
    public void OnClickedStudyMe() {
        myPanel.OnClickedStudyASet(mySet);
    }
}
