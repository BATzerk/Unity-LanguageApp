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
    [SerializeField] public RectTransform rt_termTileContent; // all the TermTiles go on here.
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
            newTile.Initialize(this, rt_termTileContent);
            termTiles.Add(newTile);
            if (count++ > 9999) { Debug.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }

        // Now go through the whole list and add/remove
        float tempY = -20;
        for (int i=0; i<currStudySet.allTerms.Count; i++) {
            Term term = currStudySet.allTerms[i];
            TermEditableTile tile = termTiles[i];

            tile.UpdateContent(i, term);//, tempY);
            tempY -= 120;
        }

        // Update the parent content RT height!
        rt_termTileContent.sizeDelta = new Vector2(rt_termTileContent.sizeDelta.x, -tempY + 100);

    }


    // ================================================================
    //  Events
    // ================================================================
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
        rt_termTileContent.anchoredPosition = new Vector2(rt_termTileContent.anchoredPosition.x, rt_termTileContent.sizeDelta.y);
    }



}







