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
    [SerializeField] public RectTransform rt_options;
    [SerializeField] public RectTransform rt_preDelete;
    [SerializeField] public RectTransform rt_scrollContent; // we just control the size of this. rt_tilesContent is parented to this.
    [SerializeField] public RectTransform rt_tilesContent; // all the TileViews go on here.
    private List<TermEditableTile> termTiles = new List<TermEditableTile>();
    // References
    private StudySet currStudySet;


    // ================================================================
    //  Start
    // ================================================================
    void Start()
    {
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

        // Now go through the whole list and add/remove
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


    // ================================================================
    //  Events
    // ================================================================
    public void OnClick_CopyToClipboard() {
        GameUtils.CopyToClipboard(currStudySet.GetAsExportedString());
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
    public void RemoveTermTile(Term term) {
        currStudySet.RemoveTerm(term);
        UpdateTileList();
    }
    public void AddNewTerm() {
        currStudySet.AddTerm();
        UpdateTileList();
        // Scroll down to the bottom now.
        rt_scrollContent.anchoredPosition = new Vector2(rt_scrollContent.anchoredPosition.x, rt_scrollContent.sizeDelta.y-800);// note: -800 is a hack.
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



}







