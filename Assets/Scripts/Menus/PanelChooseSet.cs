using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelChooseSet : BasePanel {
    // Overrides
    override public PanelTypes MyPanelType { get { return PanelTypes.ChooseSet; } }
    // Components
    [SerializeField] private StudySetSpecialTile setTile_aced;
    [SerializeField] private StudySetSpecialTile setTile_shelved;
    [SerializeField] private StudySetSpecialTile setTile_toValidate;
    [SerializeField] private StudySetSpecialTile setTile_wantRecording;
    [SerializeField] private StudySetToughiesTile setTile_toughies;
    [SerializeField] public RectTransform rt_scrollContent; // we just control the size of this. rt_tilesContent is parented to this.
    [SerializeField] public RectTransform rt_tilesContent; // all the TileViews go on here.
    private List<StudySetTileView> tileViews=new List<StudySetTileView>();
    // References
    [SerializeField] private MenuController menuController;
    // Properties
    const float tileHeight = 56;
    const float tileSpacing = 2;




    // ================================================================
    //  Start
    // ================================================================
    void Start() {
        UpdateAllTiles();
    }

    public void UpdateAllTiles() {
        // First, let's update JUST the special ones.
        setTile_aced.UpdateVisuals(this, dm.library.setAced);
        setTile_shelved.UpdateVisuals(this, dm.library.setShelved);
        setTile_toValidate.UpdateVisuals(this, dm.library.setToValidate);
        setTile_wantRecording.UpdateVisuals(this, dm.library.setWantRecording);
        setTile_toughies.UpdateVisuals();

        List<StudySet> sets = dm.library.sets;

        // Destroy any extras.
        int count = 0;
        while (tileViews.Count > sets.Count) {
            StudySetTileView tile = tileViews[tileViews.Count - 1];
            tileViews.Remove(tile);
            Destroy(tile.gameObject);
            if (count++ > 9999) { AppDebugLog.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }
        // Add any missing.
        count = 0;
        while (tileViews.Count < sets.Count) {
            StudySetTileView newTile = Instantiate(ResourcesHandler.Instance.StudySetTileView).GetComponent<StudySetTileView>();
            newTile.Initialize(this, rt_tilesContent);
            tileViews.Add(newTile);
            if (count++ > 9999) { AppDebugLog.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }

        // Now update all the existing tiles!
        for (int i=0; i<tileViews.Count; i++) {
            tileViews[i].SetMySet(sets[i]);
        }

        // Update the parent content RT height!
        float contentHeight = tileViews.Count * (tileHeight+tileSpacing) + 200;
        rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, contentHeight);
    }

    protected override void OnOpened() {
        base.OnOpened();
        // Update all tile visuals!
        UpdateAllTiles();
    }


    // ================================================================
    //  Events
    // ================================================================
    public void OnClickedStudyASet(StudySet set) {
        menuController.OpenPanel_StudyFlashcards(set);
    }
    public void OnClickedEditASet(StudySet set) {
        menuController.OpenPanel_EditSet(set);
    }

    public void OnClick_EditSet_Aced() {
        menuController.OpenPanel_EditSet(dm.library.setAced);
    }
    public void OnClick_EditSet_Shelved() {
        menuController.OpenPanel_EditSet(dm.library.setShelved);
    }
    public void OnClick_EditSet_ToValidate() {
        menuController.OpenPanel_EditSet(dm.library.setToValidate);
    }
    public void OnClick_EditSet_WantRecording() {
        menuController.OpenPanel_EditSet(dm.library.setWantRecording);
    }

    public void OnClickAddSet() {
        //List<Term> startingTerms = new List<Term>();
        //startingTerms.Add(new Term());
        dm.library.sets.Add(new StudySet(dm.library, "Untitled"));
        UpdateAllTiles();
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
    }







}
