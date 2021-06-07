using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupSetOptions : BasePopup {
    // Components
    [SerializeField] private Button b_preDelete;
    [SerializeField] private TMP_InputField if_setName;
    [SerializeField] private TMP_InputField if_pastedTerms; // for Brett's usage! To unload notes from Notes app into here.
    [SerializeField] private RectTransform rt_preDelete;
    [SerializeField] private StudySetProgressBar progressBar;
    // References
    [SerializeField] private MenuController menuController;




    // ================================================================
    //  Start
    // ================================================================
    void Start() {
        Hide(); // Start off hidden, of course.
    }



    // ================================================================
    //  Show / Hide
    // ================================================================
    public void Show() {//StudySet _set) {
        //this.currSet = _set;
        this.gameObject.SetActive(true);
        if (currSet == null) {
            Debug.LogError("Oops! Trying to open PopupSetOptions, but DataManager's currSet value hasn't been set! We need a set to have options for.");
        }
        if_setName.text = currSet.name;
        progressBar.UpdateVisuals();

        // Hide/Show components
        bool isRemixSet = currSet.isRemixSet;
        b_preDelete.gameObject.SetActive(!isRemixSet);
        if_pastedTerms.gameObject.SetActive(!isRemixSet);
        
    }
    public void Hide() {
        this.gameObject.SetActive(false);
        rt_preDelete.gameObject.SetActive(false); // also hide the pre-delete, just in case.
    }
    public void ShowPreDelete() {
        rt_preDelete.gameObject.SetActive(true);
    }
    public void HidePreDelete() {
        rt_preDelete.gameObject.SetActive(false);
    }


    // ================================================================
    //  Events
    // ================================================================
    public void OnEndEditSetName() {
        currSet.name = if_setName.text;
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        eventManager.OnAnySetContentsChanged();
    }

    public void OnClick_CopyToClipboard() {
        GameUtils.CopyToClipboard(dm.GetStudySetExportedString_ForeignBracketPhoneticNative(currSet));
        Hide();
    }
    public void OnClick_ConfirmDeleteSet() {
        dm.DeleteSet(currSet);
        dm.SaveStudySetLibrary();
        menuController.OpenPanel_ChooseSet();
        Hide();
    }
    //public void OnClick_StartNewRound() {
    //    currSet.StartNewRound();
    //    GameManagers.Instance.DataManager.SaveStudySetLibrary();
    //    eventManager.OnAnySetContentsChanged();
    //    progressBar.UpdateVisuals();
    //}
    public void OnClick_ShuffleAndRestart() {
        currSet.ShuffleAndRestartDeck();
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        eventManager.OnAnySetContentsChanged();
        progressBar.UpdateVisuals();
    }
    public void OnEndEditPastedNewTerms() {
        string pastedTerms = if_pastedTerms.text;
        if (string.IsNullOrWhiteSpace(pastedTerms)) { return; } // No string? Get outta here.

        string[] termStrings = pastedTerms.Split('\n');

        string errorStr = ""; // we'll add to this as we find issues.
        List<Term> newTerms = new List<Term>();
        foreach (string str in termStrings) {
            try {
                int splitIndex;
                if (str.Contains(" — ")) splitIndex = str.IndexOf(" — "); // use double-sized hyphen, if that's how it's (optionally) formatted.
                else splitIndex = str.IndexOf(" - "); // otherwise, split by the regular hyphen.
                string native = str.Substring(splitIndex + 3);
                string foreign = str.Substring(0, splitIndex);
                string phonetic = "";
                // pull out the phonetic pronunciation
                int lbIndex = foreign.LastIndexOf('['); // left bracket index
                int rbIndex = foreign.LastIndexOf(']'); // right bracket index
                if (rbIndex == foreign.Length - 1) { // if this one ENDS in a phonetic explanation...
                    phonetic = foreign.Substring(lbIndex + 1);
                    phonetic = phonetic.Substring(0, phonetic.Length - 1); // get rid of that last ] char.
                    foreign = foreign.Substring(0, lbIndex - 1);
                }
                newTerms.Add(new Term(native, foreign, phonetic));
            }
            catch {
                AppDebugLog.LogError("Some issue with an imported term string: \"" + str + "\"");
            }
        }

        // Print any issues.
        if (!string.IsNullOrWhiteSpace(errorStr)) {
            AppDebugLog.LogError(errorStr);
        }

        // Okay, NOW let's go ahead and add all the new terms to the StudySet!
        foreach (Term term in newTerms) {
            dm.library.AddNewTerm(term, currSet);
        }
        GameManagers.Instance.DataManager.SaveStudySetLibrary();


        // Clear text field
        if_pastedTerms.text = "";
        // Dispatch event!
        eventManager.OnAnySetContentsChanged();
    }



}
