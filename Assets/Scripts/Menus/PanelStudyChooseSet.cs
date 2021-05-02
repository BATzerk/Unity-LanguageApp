using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelStudyChooseSet : BasePanel
{
    // Components
    [SerializeField] public RectTransform rt_scrollContent; // we just control the size of this. rt_tilesContent is parented to this.
    [SerializeField] public RectTransform rt_tilesContent; // all the TileViews go on here.
    private List<StudySetTileView> tileViews=new List<StudySetTileView>();
    // References
    [SerializeField] private MenuController menuController;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ================================================================
    //  Start
    // ================================================================
    void Start() {
        RemakeTileViews();
    }

    private void RemakeTileViews() {
        // Destroy 'em all.
        for (int i=tileViews.Count-1; i>=0; i--) {
            Destroy(tileViews[i].gameObject);
        }
        tileViews = new List<StudySetTileView>();

        // Make 'em all.
        foreach (StudySet set in dm.library.sets) {
            StudySetTileView newView = Instantiate(ResourcesHandler.Instance.StudySetTileView).GetComponent<StudySetTileView>();
            newView.Initialize(this, rt_tilesContent, set);
            tileViews.Add(newView);
        }
        const float tileHeight = 60;
        const float tileSpacing = 10;
        float contentHeight = tileViews.Count * (tileHeight+tileSpacing) + 200;
        rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, contentHeight);
    }

    protected override void OnOpened() {
        base.OnOpened();
        // Update all tile visuals!
        foreach (StudySetTileView tile in tileViews) tile.UpdateVisuals();
    }


    // ================================================================
    //  Events
    // ================================================================
    public void OnClickedStudyASet(StudySet studySet) {
        menuController.OpenPanel_StudyFlashcards(studySet);
    }
    public void OnClickedEditASet(StudySet studySet) {
        menuController.OpenPanel_EditSet(studySet);
    }

    public void OnClickAddSet() {
        List<Term> startingTerms = new List<Term>();
        startingTerms.Add(new Term());
        dm.library.sets.Add(new StudySet("Untitled", startingTerms));
        RemakeTileViews();
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
    }






}
