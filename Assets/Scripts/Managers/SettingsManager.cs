using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class SettingsManager {
    // Properties
    private bool doShowCardDots;
    private bool doShowCardStats;
    private bool doTrimAudioClips;
    private bool doNormalizeAudioClips;
    private bool isTTSOn;
    private float ttsSpeechRate;



    // ----------------------------------------------------------------
    //  Getters
    // ----------------------------------------------------------------
    public bool DoShowCardDots {
        get { return doShowCardDots; }
        set {
            doShowCardDots = value;
            SaveStorage.SetBool(SaveKeys.DoShowCardDots, value);
        }
    }
    public bool DoShowCardStats {
        get { return doShowCardStats; }
        set {
            doShowCardStats = value;
            SaveStorage.SetBool(SaveKeys.DoShowCardStats, value);
        }
    }

    public bool DoTrimAudioClips {
        get { return doTrimAudioClips; }
        set {
            doTrimAudioClips = value;
            SaveStorage.SetBool(SaveKeys.DoTrimAudioClips, value);
        }
    }
    public bool DoNormalizeAudioClips {
        get { return doNormalizeAudioClips; }
        set {
            doNormalizeAudioClips = value;
            SaveStorage.SetBool(SaveKeys.DoNormalizeAudioClips, value);
        }
    }
    public bool IsTTSOn {
        get { return isTTSOn; }
        set {
            isTTSOn = value;
            SaveStorage.SetBool(SaveKeys.IsTTSOn, value);
        }
    }
    public float TTSSpeechRate {
        get { return ttsSpeechRate; }
        set {
            ttsSpeechRate = value;
            SaveStorage.SetFloat(SaveKeys.TTSSpeechRate, value);
        }
    }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public SettingsManager() {
        doShowCardDots = SaveStorage.GetBool(SaveKeys.DoShowCardDots, true);
        doShowCardStats = SaveStorage.GetBool(SaveKeys.DoShowCardStats, true);
        doTrimAudioClips = SaveStorage.GetBool(SaveKeys.DoTrimAudioClips, true);
        doNormalizeAudioClips = SaveStorage.GetBool(SaveKeys.DoNormalizeAudioClips, true);
        isTTSOn = SaveStorage.GetBool(SaveKeys.IsTTSOn, true);
        ttsSpeechRate = SaveStorage.GetFloat(SaveKeys.TTSSpeechRate, 1);
    }
}


