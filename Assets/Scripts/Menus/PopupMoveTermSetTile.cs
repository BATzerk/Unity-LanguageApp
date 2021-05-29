using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupMoveTermSetTile : BasePopup {
    // Components
    [SerializeField] private Image i_currSetBorder; // shows if I'm the current term's set.
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private TextMeshProUGUI t_name;
    [SerializeField] private TextMeshProUGUI t_numTerms;
    // References
    private StudySet mySet;
    private PopupMoveTerm myPopup;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(PopupMoveTerm myPopup, RectTransform tf_parent, StudySet mySet, bool isSameSet) {
        this.myPopup = myPopup;
        this.mySet = mySet;
        GameUtils.ParentAndReset(gameObject, tf_parent);

        i_currSetBorder.gameObject.SetActive(isSameSet);
        t_name.text = mySet.name;
        t_numTerms.text = mySet.NumTotal.ToString();
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickMe() {
        myPopup.OnClickStudySet(mySet);
    }


}
