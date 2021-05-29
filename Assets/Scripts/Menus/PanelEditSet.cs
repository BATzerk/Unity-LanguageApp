using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelEditSet : BasePanel {
    // Overrides
    override public PanelTypes MyPanelType { get { return PanelTypes.EditSet; } }
    // Components
    [SerializeField] private TMP_InputField if_setName;
    [SerializeField] private RectTransform rt_scrollContent; // we just control the size of this. rt_tilesContent is parented to this.
    [SerializeField] private RectTransform rt_tilesContent; // all the TileViews go on here.
    private List<TermEditableTile> termTiles = new List<TermEditableTile>();
    // References
    [SerializeField] private MenuController menuController;


    // ================================================================
    //  Start / Destroy
    // ================================================================
    private void Start() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent += OnSetContentsChanged;
    }
    private void OnDestroy() {
        GameManagers.Instance.EventManager.SetContentsChangedEvent -= OnSetContentsChanged;
    }


    // ================================================================
    //  Doers
    // ================================================================
    protected override void OnOpened() {
        base.OnOpened();
        UpdateTileList();
    }

    private void OnSetContentsChanged() {
        if (gameObject.activeInHierarchy) { // If I'm open, update my tiles!
            UpdateTileList();
        }
    }
    private void UpdateTileList() {
        // -- TEXTS --
        if_setName.text = currSet.name; // update header

        // -- TILES --
        List<string> termGs = currSet.allTermGs;
        // Destroy any extras.
        int count=0;
        while (termTiles.Count > termGs.Count) {
            TermEditableTile tile = termTiles[termTiles.Count-1];
            termTiles.Remove(tile);
            Destroy(tile.gameObject);
            if (count++ > 9999) { AppDebugLog.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }
        // Add any missing.
        count = 0;
        while (termTiles.Count < termGs.Count) {
            TermEditableTile newTile = Instantiate(ResourcesHandler.Instance.TermEditableTile).GetComponent<TermEditableTile>();
            newTile.Initialize(true, rt_tilesContent);
            termTiles.Add(newTile);
            if (count++ > 9999) { AppDebugLog.LogError("Oops, count got too big in while loop in UpdateTileList."); break; }
        }

        // Now update all the existing tiles!
        for (int i=0; i<termGs.Count; i++) {
            Term term = dm.library.GetTerm(termGs[i]);
            TermEditableTile tile = termTiles[i];
            if (term == null) {
                Debug.LogError("Whoa, can't find a term in the library for this set.");
            }

            tile.SetMyTerm(i, term);
        }
    }




    private void Update() {
        // Update the parent content RT height!
        if (termTiles.Count > 0) {
            float contentHeight = Mathf.Abs(termTiles[termTiles.Count-1].gameObject.transform.localPosition.y);
            if (contentHeight > 0) { // IF the layout groups aren't just updating (and everything's 0 height for a frame)...
                rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, contentHeight + 700);
            }
        }
    }




    // ================================================================
    //  Events
    // ================================================================
    public void OnEndEditSetName() {
        currSet.name = if_setName.text;
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
    }
    public void AddNewTerm() {
        dm.library.AddNewTerm(new Term(), currSet);
        UpdateTileList();

        // Scroll down to the bottom now.
        rt_scrollContent.anchoredPosition = new Vector2(rt_scrollContent.anchoredPosition.x, rt_scrollContent.rect.height-500);// note: -500 is a hack.
        // Auto-focus on the English input field!
        termTiles[termTiles.Count-1].OpenKeyboardForNativeField();
    }





}







