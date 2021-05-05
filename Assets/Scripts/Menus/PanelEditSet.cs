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

        if_setName.text = currStudySet.name;

        UpdateTileList();
    }

    private void UpdateTileList() {
        List<Term> terms = currStudySet.allTerms;
        // Destroy any extras.
        int count=0;
        while (termTiles.Count > terms.Count) {
            TermEditableTile tile = termTiles[termTiles.Count-1];
            termTiles.Remove(tile);
            Destroy(tile.gameObject);
            if (count++ > 9999) { Debug.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }
        // Add any missing.
        count = 0;
        while (termTiles.Count < terms.Count) {
            TermEditableTile newTile = Instantiate(ResourcesHandler.Instance.TermEditableTile).GetComponent<TermEditableTile>();
            newTile.Initialize(this, rt_tilesContent);
            termTiles.Add(newTile);
            if (count++ > 9999) { Debug.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }

        // Now update all the existing tiles!
        for (int i=0; i<currStudySet.allTerms.Count; i++) {
            Term term = currStudySet.allTerms[i];
            TermEditableTile tile = termTiles[i];

            tile.UpdateContent(i, term);
        }

        // Update the parent content RT height!
        const float tileHeight = 130;
        float tileSpacing = rt_tilesContent.GetComponent<VerticalLayoutGroup>().spacing;
        float contentHeight = termTiles.Count * (tileHeight + tileSpacing) + 450;
        rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, contentHeight);

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




    // ================================================================
    //  Events
    // ================================================================
    public void OnClick_CopyToClipboard() {
        GameUtils.CopyToClipboard(currStudySet.GetAsExportedString_ForeignBracketPhoneticNative());
        HideOptions();
    }
    public void OnClick_ConfirmDeleteSet() {
        GameManagers.Instance.DataManager.library.sets.Remove(currStudySet);
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        SceneHelper.ReloadScene(); // TODO: Boot me back to the previous panel instead.
    }


    public void OnEndEditSetName() {
        currStudySet.name = if_setName.text;
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
    }
    public void AddNewTerm() {
        currStudySet.AddTerm();
        UpdateTileList();
        // Scroll down to the bottom now.
        rt_scrollContent.anchoredPosition = new Vector2(rt_scrollContent.anchoredPosition.x, rt_scrollContent.sizeDelta.y-800);// note: -800 is a hack.
    }
    public void OnEndEditPastedNewTerms() {
        string pastedTerms = if_pastedTerms.text;
        if (string.IsNullOrWhiteSpace(pastedTerms)) { return; } // No string? Get outta here.

        string[] termStrings = pastedTerms.Split('\n');

        string errorStr = ""; // we'll add to this as we find issues.
        List<Term> newTerms = new List<Term>();
        foreach (string str in termStrings) {
            try {
                //if (str.Contains(" - ")) {
                //    errorStr += "Oops! We don't parse hyphens; only dash characters.\n";
                //}
                int splitIndex = str.IndexOf(" - ");//—
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
            currStudySet.AddTerm(term);
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







