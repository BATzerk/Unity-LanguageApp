using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PanelEditSet : BasePanel
{
    // Components
    [SerializeField] private TMP_InputField if_setName;
    [SerializeField] private TMP_InputField if_pastedTerms; // for Brett's usage! To unload notes from Notes app into here.
    [SerializeField] public RectTransform rt_options;
    [SerializeField] public RectTransform rt_preDelete;
    [SerializeField] public RectTransform rt_scrollContent; // we just control the size of this. rt_tilesContent is parented to this.
    [SerializeField] public RectTransform rt_tilesContent; // all the TileViews go on here.
    private List<TermEditableTile> termTiles = new List<TermEditableTile>();
    // References
    [SerializeField] private MenuController menuController;
    private StudySet currStudySet;


    // ================================================================
    //  Start / Destroy
    // ================================================================
    private void Start() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent += UpdateTileList;
    }
    private void OnDestroy() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent -= UpdateTileList;
    }


    // ================================================================
    //  Doers
    // ================================================================
    public void OpenSet(StudySet currStudySet) {
        this.currStudySet = currStudySet;
        SaveStorage.SetString(SaveKeys.LastStudySetOpenName, currStudySet.name);

        if_setName.text = currStudySet.name;

        UpdateTileList();
    }

    private void UpdateTileList() {
        List<string> termGs = currStudySet.allTermGs;
        // Destroy any extras.
        int count=0;
        while (termTiles.Count > termGs.Count) {
            TermEditableTile tile = termTiles[termTiles.Count-1];
            termTiles.Remove(tile);
            Destroy(tile.gameObject);
            if (count++ > 9999) { Debug.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }
        // Add any missing.
        count = 0;
        while (termTiles.Count < termGs.Count) {
            TermEditableTile newTile = Instantiate(ResourcesHandler.Instance.TermEditableTile).GetComponent<TermEditableTile>();
            newTile.Initialize(true, rt_tilesContent);
            termTiles.Add(newTile);
            if (count++ > 9999) { Debug.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }

        // Now update all the existing tiles!
        for (int i=0; i<termGs.Count; i++) {
            Term term = dm.library.GetTerm(termGs[i]);
            TermEditableTile tile = termTiles[i];

            tile.SetMyTerm(i, term);
        }

        HideOptions();
        HidePreDelete();
    }

    public void ShowOptions() {
        rt_options.gameObject.SetActive(true);
    }
    public void HideOptions() {
        rt_options.gameObject.SetActive(false);
        rt_preDelete.gameObject.SetActive(false); // also hide the pre-delete, just in case.
    }
    public void ShowPreDelete() {
        rt_preDelete.gameObject.SetActive(true);
    }
    public void HidePreDelete() {
        rt_preDelete.gameObject.SetActive(false);
    }



    private void Update() {
        // Update the parent content RT height!
        if (termTiles.Count > 0) {
            float contentHeight = Mathf.Abs(termTiles[termTiles.Count-1].gameObject.transform.localPosition.y);
            if (contentHeight > 0) { // IF the layout groups aren't just updating (and everything's 0 height for a frame)...
                rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, contentHeight + 400);
            }
        }
    }




    // ================================================================
    //  Events
    // ================================================================
    public void OnClick_CopyToClipboard() {
        GameUtils.CopyToClipboard(dm.GetStudySetExportedString_ForeignBracketPhoneticNative(currStudySet));
        HideOptions();
    }
    public void OnClick_ConfirmDeleteSet() {
        dm.DeleteSet(currStudySet);
        dm.SaveStudySetLibrary();
        menuController.OpenPanel_StudyChooseSet();
    }


    public void OnEndEditSetName() {
        currStudySet.name = if_setName.text;
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
    }
    public void AddNewTerm() {
        dm.library.AddNewTerm(new Term(), currStudySet);
        UpdateTileList();

        // Scroll down to the bottom now.
        rt_scrollContent.anchoredPosition = new Vector2(rt_scrollContent.anchoredPosition.x, rt_scrollContent.rect.height-500);// note: -500 is a hack.
        // Auto-focus on the English input field!
        termTiles[termTiles.Count-1].OpenKeyboardForNativeField();
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
                Debug.LogError("Some issue with an imported term string: \"" + str + "\"");
            }
        }

        // Print any issues.
        if (!string.IsNullOrWhiteSpace(errorStr)) {
            Debug.LogError(errorStr);
        }

        // Okay, NOW let's go ahead and add all the new terms to the StudySet!
        foreach (Term term in newTerms) {
            dm.library.AddNewTerm(term, currStudySet);
        }
        GameManagers.Instance.DataManager.SaveStudySetLibrary();


        // Clear text field
        if_pastedTerms.text = "";
        // Update tiles now!
        UpdateTileList();

        // joint hyphen  —
        // single hyphen -
    }





}







