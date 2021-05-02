using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveTermPopupSetTile : MonoBehaviour {
    // Components
    [SerializeField] private Image i_currSetBorder; // shows if I'm the current term's set.
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private TextMeshProUGUI t_name;
    // References
    private StudySet mySet;
    private MoveTermPopup myPopup;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(MoveTermPopup myPopup, RectTransform tf_parent, StudySet mySet, bool isSameSet) {
        this.myPopup = myPopup;
        this.mySet = mySet;
        GameUtils.ParentAndReset(gameObject, tf_parent);

        i_currSetBorder.gameObject.SetActive(isSameSet);
        t_name.text = mySet.name;

        //UpdateVisuals();
    }


    //// ----------------------------------------------------------------
    ////  Update Visuals
    //// ----------------------------------------------------------------
    //private void UpdateVisuals() {
    //}


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickMe() {
        myPopup.OnClickStudySet(mySet);
    }


}
