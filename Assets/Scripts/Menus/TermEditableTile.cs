using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TermEditableTile : MonoBehaviour
{
    // Components
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_deleteConfirmation;
    [SerializeField] private TextMeshProUGUI t_myNumber;
    [SerializeField] private TMP_InputField if_native;
    [SerializeField] private TMP_InputField if_foreign;
    [SerializeField] private TMP_InputField if_phonetic;
    // References
    private Term myTerm;
    private PanelEditSet myPanel;


    public void Initialize(PanelEditSet myPanel, RectTransform tf_parent) {
        this.myPanel = myPanel;
        GameUtils.ParentAndReset(gameObject, tf_parent);
    }
    public void UpdateContent(int myIndex, Term myTerm) {//, float posY) {
        this.myTerm = myTerm;

        // Update visuals
        //myRectTransform.anchoredPosition = new Vector2(0, posY);
        t_myNumber.text = (myIndex + 1).ToString();
        if_native.text = myTerm.english;
        if_foreign.text = myTerm.danish;
        if_phonetic.text = myTerm.phonetic;
        HideDeleteConfirmation();
    }


    public void OnFinishedEditingAnyField() {
        // Update my term by my texts.
        myTerm.english = if_native.text;
        myTerm.danish = if_foreign.text;
        myTerm.phonetic = if_phonetic.text;
        // The moment we're done editing a field, save the ENTIRE library again!
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
    }


    public void OnClickPreDelete() {
        rt_deleteConfirmation.gameObject.SetActive(true);
    }
    public void HideDeleteConfirmation() {
        rt_deleteConfirmation.gameObject.SetActive(false);
    }
    public void OnClickDeleteConfirmed() {
        myPanel.RemoveTermTile(myTerm);
        //CloseDeleteConfirmation();
    }
}
