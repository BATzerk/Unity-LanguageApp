using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubPopAppDebugLogDisplay : MonoBehaviour {
    // Components
    [SerializeField] private TextMeshProUGUI t_log;
    [SerializeField] private RectTransform rt_scrollContent;


    private void Update() {
        t_log.text = AppDebugLog.logStr;
        rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, t_log.preferredHeight + 100);
    }
}
