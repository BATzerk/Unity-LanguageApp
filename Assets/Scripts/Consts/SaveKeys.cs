﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveKeys {
    public static string IsTTSOn = "IsTTSOn";
    public static string TTSSpeechRate = "TTSSpeechRate";
    public static string LastPanelOpen = "LastPanelOpen";
    public static string LastStudySetOpenName = "LastStudySetOpenName";
    public static string StudySetLibrary = "StudySetLibrary";
    public static string SourdoughTimeLastRefilled = "SourdoughTimeLastRefilled";

    public static string TermAudioClip0(string guid) {
        return Path.Combine(Application.persistentDataPath, "Audio/TermClip0_" + guid + ".wav");//mp3
    }
}
