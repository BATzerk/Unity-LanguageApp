using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomTabBar : MonoBehaviour {
    // Components
    [SerializeField] BottomTabBarButton b_appSettings;
    [SerializeField] BottomTabBarButton b_chooseSet;
    [SerializeField] BottomTabBarButton b_editSet;
    [SerializeField] BottomTabBarButton b_search;
    [SerializeField] BottomTabBarButton b_studyFlashcards;
    // References
    [SerializeField] MenuController menuController;


    // Update Visuals
    public void UpdateTabHighlighted(PanelTypes panelType) {
        // Unhighlight all first.
        b_appSettings.ShowHighlight(false);
        b_chooseSet.ShowHighlight(false);
        b_editSet.ShowHighlight(false);
        b_search.ShowHighlight(false);
        b_studyFlashcards.ShowHighlight(false);

        // Only highlight the relevant one!
        switch (panelType) {
            case PanelTypes.AppSettings:
                b_appSettings.ShowHighlight(true);
                break;
            case PanelTypes.ChooseSet:
                b_chooseSet.ShowHighlight(true);
                break;
            case PanelTypes.EditSet:
                b_editSet.ShowHighlight(true);
                break;
            case PanelTypes.SearchTerms:
                b_search.ShowHighlight(true);
                break;
            case PanelTypes.StudyFlashcards:
                b_studyFlashcards.ShowHighlight(true);
                break;
            default:
                Debug.LogWarning("Whoa! No tab in BottomTabBar for this panel: " + panelType);
                break;
        }

        // Now, importantly, update interactability of set-dependent buttons.
        bool isASet = GameManagers.Instance.DataManager.CurrSet != null;
        b_editSet.interactable = isASet;
        b_studyFlashcards.interactable = isASet;
    }
}
