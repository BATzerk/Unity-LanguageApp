﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupAppOptions : MonoBehaviour {
    // Components
    [SerializeField] private Toggle toggle_doShowCardDots;
    [SerializeField] private Toggle toggle_doShowCardStats;
    [SerializeField] private Toggle toggle_doAutoTrimAudioClips;
    [SerializeField] private Toggle toggle_doNormalizeAudioClips;


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    void Start() {
        // Start closed.
        ClosePopup();

        //// Add event listeners
        //GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent += OpenPopup;
    }
    //private void OnDestroy() {
    //    // Remove event listeners
    //    GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent -= OpenPopup;
    //}

    // ----------------------------------------------------------------
    //  Open / Close
    // ----------------------------------------------------------------
    public void ClosePopup() {
        this.gameObject.SetActive(false);
        GameManagers.Instance.EventManager.OnPopupAppOptionsClosed();
    }
    public void OpenPopup() {
        this.gameObject.SetActive(true);
        toggle_doShowCardDots.isOn = GameManagers.Instance.SettingsManager.DoShowCardDots;
        toggle_doShowCardStats.isOn = GameManagers.Instance.SettingsManager.DoShowCardStats;
        toggle_doAutoTrimAudioClips.isOn = GameManagers.Instance.SettingsManager.DoTrimAudioClips;
        toggle_doNormalizeAudioClips.isOn = GameManagers.Instance.SettingsManager.DoNormalizeAudioClips;
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClick_CopyAllSetsToClipboard() {
        GameManagers.Instance.DataManager.CopyAllSetsToClipboard();
    }
    public void OnClick_ForceRefillSourdoughSet() {
        GameManagers.Instance.DataManager.RefillSourdoughSet();
        SceneHelper.ReloadScene();
    }
    public void OnClick_ReshuffleAllSets() {
        GameManagers.Instance.DataManager.library.Debug_ReshuffleAllSets();
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        SceneHelper.ReloadScene();
    }
    public void OnValueChanged_DoShowCardDots() {
        GameManagers.Instance.SettingsManager.DoShowCardDots = toggle_doShowCardDots.isOn;
    }
    public void OnValueChanged_DoShowCardStats() {
        GameManagers.Instance.SettingsManager.DoShowCardStats = toggle_doShowCardStats.isOn;
    }
    public void OnValueChanged_DoAutoTrimAudioClips() {
        GameManagers.Instance.SettingsManager.DoTrimAudioClips = toggle_doAutoTrimAudioClips.isOn;
    }
    public void OnValueChanged_DoNormalizeAudioClips() {
        GameManagers.Instance.SettingsManager.DoNormalizeAudioClips = toggle_doNormalizeAudioClips.isOn;
    }

}
