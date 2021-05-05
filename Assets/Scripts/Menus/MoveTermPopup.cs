using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveTermPopup : MonoBehaviour
{
    // Components
    [SerializeField] public RectTransform rt_scrollContent; // we just control the size of this. rt_tilesContent is parented to this.
    [SerializeField] public RectTransform rt_tilesContent; // all the TileViews go on here.
    [SerializeField] private TextMeshProUGUI t_currTermName;
    private List<MoveTermPopupSetTile> tiles=new List<MoveTermPopupSetTile>();
    // References
    private Term currTerm;


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    private void Start() {
        Hide(); // Start off hidden, of course.

        GameManagers.Instance.EventManager.ShowMoveTermPopupEvent += Show;
    }
    private void OnDestroy() {
        GameManagers.Instance.EventManager.ShowMoveTermPopupEvent -= Show;
    }

    // ----------------------------------------------------------------
    //  Hide / Show
    // ----------------------------------------------------------------
    public void Hide() {
        this.gameObject.SetActive(false);
    }
    public void Show(Term currTerm) {
        this.currTerm = currTerm;
        this.gameObject.SetActive(true);

        t_currTermName.text = currTerm.english;

        // Destroy 'em all.
        for (int i = tiles.Count-1; i>=0; i--) {
            Destroy(tiles[i].gameObject);
        }
        tiles = new List<MoveTermPopupSetTile>();

        // Make 'em all.
        StudySetLibrary library = GameManagers.Instance.DataManager.library;
        List<StudySet> setsToShow = new List<StudySet>();
        // Add regular set list.
        foreach (StudySet set in library.sets) setsToShow.Add(set);
        // Add special sets.
        setsToShow.Add(library.setAced);
        setsToShow.Add(library.setShelved);
        setsToShow.Add(library.setToValidate);
        setsToShow.Add(library.setWantRecording);

        foreach (StudySet set in setsToShow) {
            MoveTermPopupSetTile newView = Instantiate(ResourcesHandler.Instance.MoveTermPopupSetTile).GetComponent<MoveTermPopupSetTile>();
            bool isSameSet = set == currTerm.mySet;
            newView.Initialize(this, rt_tilesContent, set, isSameSet);
            tiles.Add(newView);
        }
        const float tileHeight = 60;
        const float tileSpacing = 10;
        float contentHeight = tiles.Count * (tileHeight + tileSpacing) + 200;
        rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, contentHeight);
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickStudySet(StudySet set) {
        // Actually move it!
        GameManagers.Instance.DataManager.MoveTermToSet(currTerm, set);
        // Dispatch event so folks can update their visuals.
        GameManagers.Instance.EventManager.OnAnySetContentsChanged();
        // See ya!
        Hide();
    }



}
