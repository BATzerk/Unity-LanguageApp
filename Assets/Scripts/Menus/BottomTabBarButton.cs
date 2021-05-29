using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomTabBarButton : MonoBehaviour {
    // Components
    [SerializeField] Button myButton;
    [SerializeField] Image i_highlight;

    public bool interactable {
        get { return myButton.interactable; }
        set { myButton.interactable = value; }
    }

    public void ShowHighlight(bool isHighlight) {
        i_highlight.enabled = isHighlight;
    }
}
