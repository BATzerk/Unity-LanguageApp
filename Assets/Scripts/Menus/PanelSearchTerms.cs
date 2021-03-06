using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelSearchTerms : BasePanel {
    // Overrides
    override public PanelTypes MyPanelType { get { return PanelTypes.SearchTerms; } }
    // Components
    [SerializeField] private TMP_InputField if_search;
    [SerializeField] private TextMeshProUGUI t_numResults;
    [SerializeField] private TextMeshProUGUI t_andMore; // ONLY shows up if there are more search results than we can show.
    [SerializeField] public RectTransform rt_scrollContent; // we just control the size of this. rt_tilesContent is parented to this.
    [SerializeField] public RectTransform rt_tilesContent; // all the TileViews go on here.
    private List<TermEditableTile> termTiles = new List<TermEditableTile>();
    // Properties
    private const int MaxResultsToShow = 30; // don't show more than this many tiles.
    // References
    private List<Term> resultTerms; // just the terms that fit the search result!


    // Getters
    private List<Term> allTerms { get { return GameManagers.Instance.DataManager.library.allTermsL; } }



    protected override void OnOpened() {
        base.OnOpened();

        // Auto-focus on the search text field!
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(if_search.gameObject, null);
        RefreshTermTiles();
    }


    public void RefreshTermTiles() {
        // Update resultTerms!
        string searchStr = if_search.text.ToUpperInvariant();
        resultTerms = new List<Term>();
        if (searchStr.Length > 0) {
            foreach (Term term in allTerms) {
                if (term.native.ToUpperInvariant().Contains(searchStr)
                 || term.foreign.ToUpperInvariant().Contains(searchStr)
                 || term.phonetic.ToUpperInvariant().Contains(searchStr)) {
                        resultTerms.Add(term);
                }
            }
        }

        // Update t_andMore.
        int numHiddenResults = resultTerms.Count - MaxResultsToShow;
        t_andMore.gameObject.SetActive(numHiddenResults > 0);
        if (numHiddenResults > 0) {
            t_andMore.text = "...and " + numHiddenResults + " more";
        }

        // Update t_numResults
        t_numResults.text = searchStr.Length==0 ? "" : resultTerms.Count + " results";


        // Destroy any extras.
        int count = 0;
        while (termTiles.Count > resultTerms.Count) {
            TermEditableTile tile = termTiles[termTiles.Count - 1];
            termTiles.Remove(tile);
            Destroy(tile.gameObject);
            if (count++ > 9999) { AppDebugLog.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }
        // Add any missing.
        count = 0;
        while (termTiles.Count<resultTerms.Count && termTiles.Count<MaxResultsToShow) {
            TermEditableTile newTile = Instantiate(ResourcesHandler.Instance.TermEditableTile).GetComponent<TermEditableTile>();
            newTile.Initialize(false, rt_tilesContent);
            termTiles.Add(newTile);
            if (count++ > 9999) { AppDebugLog.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }

        // Now update all the tiles!
        for (int i=0; i<resultTerms.Count&&i<MaxResultsToShow; i++) {
            TermEditableTile tile = termTiles[i];
            tile.SetMyTerm(i, resultTerms[i]);
        }

        // Update the parent content RT height!
        const float tileHeight = 130;
        float tileSpacing = rt_tilesContent.GetComponent<VerticalLayoutGroup>().spacing;
        float contentHeight = termTiles.Count * (tileHeight + tileSpacing) + 450;
        rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, contentHeight);
    }






}
