using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudySetProgressBar_Solid : MonoBehaviour {
    // Components
    [SerializeField] private Image i_barBack;
    [SerializeField] private Image i_barFillRecent;
    [SerializeField] private Image i_barFillYeses;

    private StudySet currSet { get { return GameManagers.Instance.DataManager.CurrSet; } }


    // Update Visuals
    public void UpdateVisuals() {
        float barWidth = i_barBack.rectTransform.rect.width;
        float progLocYeses = (currSet.NumTotal - (currSet.pileYesesAndNosG.Count + currSet.pileQueueG.Count)) / (float)currSet.NumTotal;
        float progLocRecent = currSet.NumDone / (float)currSet.NumTotal;
        float yesWidth = barWidth * progLocYeses;
        i_barFillYeses.rectTransform.sizeDelta = new Vector2(yesWidth, i_barFillYeses.rectTransform.sizeDelta.y);
        i_barFillRecent.rectTransform.anchoredPosition = new Vector2(yesWidth, 0);
        i_barFillRecent.rectTransform.sizeDelta = new Vector2(barWidth * progLocRecent, i_barFillRecent.rectTransform.sizeDelta.y);
    }
}
