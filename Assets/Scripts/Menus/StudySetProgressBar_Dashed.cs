using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudySetProgressBar_Dashed : MonoBehaviour {
    // Properties
    private static Color colorYesed = new Color255(96, 224, 145, 70).ToColor();//99, 118, 123).ToColor();
    private static Color colorRecentYes = new Color255(96, 224, 145, 140).ToColor();//50, 180, 210, 110).ToColor();
    private static Color colorRecentNo = new Color255(225, 200, 96, 210).ToColor();//50, 180, 210, 255).ToColor();
    private static Color colorUpcoming = new Color255(0, 0, 0, 210).ToColor();
    // Components
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_layoutGroup;
    private List<Image> i_dashes=new List<Image>();




    // Update Visuals
    public void UpdateVisuals(StudySet mySet) {
        this.gameObject.SetActive(mySet.IsInProgress);

        // Add missing dashes
        int numDashesToAdd = mySet.NumTotal - i_dashes.Count;
        for (int i=0; i<numDashesToAdd; i++) {
            Image newImg = new GameObject().AddComponent<Image>();
            newImg.gameObject.name = "PBarDash_" + i;
            GameUtils.ParentAndReset(newImg.gameObject, rt_layoutGroup);
            i_dashes.Add(newImg);
        }

        // Hide surplus dashes.
        for (int i=0; i<i_dashes.Count; i++) {
            i_dashes[i].gameObject.SetActive(i < mySet.NumTotal);
        }

        // Update all dash visuals!
        int numYesed = mySet.NumTotal - (mySet.pileYesesAndNosG.Count + mySet.pileQueueG.Count);
        int numRecent = mySet.pileYesesAndNosG.Count;
        for (int i=0; i< mySet.NumTotal; i++) {
            Color color;
            if (i < numYesed) // Yesed
                color = colorYesed;
            else if (i < numYesed+numRecent) { // Recent
                bool wasAYes = mySet.pileYesG.Contains(mySet.pileYesesAndNosG[i-numYesed]);
                color = wasAYes ? colorRecentYes : colorRecentNo;
            }
            else // Upcoming
                color = colorUpcoming;
            i_dashes[i].color = color;
        }

        // Update bar width!
        float barWidth = Mathf.Min(myRectTransform.rect.width, mySet.NumTotal * 9);
        rt_layoutGroup.sizeDelta = new Vector2(barWidth, myRectTransform.rect.height);
    }
}
