using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelAppSettings : BasePanel {
    // Overrides
    override public PanelTypes MyPanelType { get { return PanelTypes.AppSettings; } }
    // Components
    [SerializeField] private PopupSetCurrForeignLanguage popupSetCurrForeignLanguage;
    [SerializeField] private Toggle toggle_doShowCardDots;
    [SerializeField] private Toggle toggle_doShowCardStats;
    [SerializeField] private Toggle toggle_doAutoTrimAudioClips;
    [SerializeField] private Toggle toggle_doNormalizeAudioClips;
    [SerializeField] private Toggle toggle_doAutoPlayTTS;
    // References
    [SerializeField] private MenuController menuController;


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    //void Start() {
    //    // Start closed.
    //    ClosePopup();

    //    //// Add event listeners
    //    //GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent += OpenPopup;
    //}
    ////private void OnDestroy() {
    ////    // Remove event listeners
    ////    GameManagers.Instance.EventManager.ShowPopup_TermOptionsEvent -= OpenPopup;
    ////}

    // ----------------------------------------------------------------
    //  Open / Close
    // ----------------------------------------------------------------
    //public void ClosePopup() {
    //    this.gameObject.SetActive(false);
    //    GameManagers.Instance.EventManager.OnPopupAppOptionsClosed();
    //}
    override protected void OnOpened() {
        base.OnOpened();
        toggle_doShowCardDots.isOn = SettingsManager.Instance.DoShowCardDots;
        toggle_doShowCardStats.isOn = SettingsManager.Instance.DoShowCardStats;
        toggle_doAutoTrimAudioClips.isOn = SettingsManager.Instance.DoTrimAudioClips;
        toggle_doNormalizeAudioClips.isOn = SettingsManager.Instance.DoNormalizeAudioClips;
        toggle_doAutoPlayTTS.isOn = SettingsManager.Instance.IsTTSOn;

        popupSetCurrForeignLanguage.Hide(); // hide this by default.
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClick_CopyAllSetsToClipboard() {
        GameManagers.Instance.DataManager.CopyAllSetsToClipboard();
    }
    public void OnClick_ForceRefillSourdoughSet() {
        GameManagers.Instance.DataManager.RefillSourdoughSet();
        menuController.OpenPanel_ChooseSet();
    }
    public void OnClick_RebuildSourdoughSet() {
        GameManagers.Instance.DataManager.Debug_RebuildSourdoughSet();
        menuController.OpenPanel_ChooseSet();
    }
    public void OnValueChanged_DoShowCardDots() {
        SettingsManager.Instance.DoShowCardDots = toggle_doShowCardDots.isOn;
    }
    public void OnValueChanged_DoShowCardStats() {
        SettingsManager.Instance.DoShowCardStats = toggle_doShowCardStats.isOn;
    }
    public void OnValueChanged_DoAutoTrimAudioClips() {
        SettingsManager.Instance.DoTrimAudioClips = toggle_doAutoTrimAudioClips.isOn;
    }
    public void OnValueChanged_DoNormalizeAudioClips() {
        SettingsManager.Instance.DoNormalizeAudioClips = toggle_doNormalizeAudioClips.isOn;
    }
    public void OnValueChanged_DoAutoPlayTTS() {
        SettingsManager.Instance.IsTTSOn = toggle_doAutoPlayTTS.isOn;
    }

}
