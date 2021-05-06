using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveKeys {
    public static string StudySetLibrary = "StudySetLibrary";
    public static string TermAudioLibrary = "TermAudioLibrary";

    public static string TermAudioClip0(Guid guid) {
        return Path.Combine(Application.persistentDataPath, "Audio/TermClip0_" + guid.ToString() + ".wav"); ;
    }
    //public static string TermAudioClip0_Save(Guid guid) {
    //    return Path.Combine(Application.persistentDataPath, "Audio/TermClip0_" + guid.ToString() + ".wav"); ;
    //}
}
