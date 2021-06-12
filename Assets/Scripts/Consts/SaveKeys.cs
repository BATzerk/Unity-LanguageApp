using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveKeys {
    // App Settings
    public static string CurrForeignCode = "CurrForeignLanguageCode";
    public static string DoShowCardDots = "DoShowCardDots";
    public static string DoShowCardStats = "DoShowCardStats";
    public static string DoTrimAudioClips = "DoTrimAudioClips";
    public static string DoNormalizeAudioClips = "DoNormalizeAudioClips";
    public static string IsTTSOn = "IsTTSOn";
    public static string TTSSpeechRate = "TTSSpeechRate";
    // Other Properties
    public static string LastPanelOpen = "LastPanelOpen";
    public static string LastStudySetOpenName = "LastStudySetOpenName";
    public static string SourdoughTimeLastRefilled = "SourdoughTimeLastRefilled";


    public static string StudySetLibrary(string langCode) { return "StudySetLibrary_" + langCode; }
    public static string TermAudioClip0(string guid) {
        return Path.Combine(Application.persistentDataPath, "Audio/TermClip0_" + guid + ".wav");//mp3
    }
}
