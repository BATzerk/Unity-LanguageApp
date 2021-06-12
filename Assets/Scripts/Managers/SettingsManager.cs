using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class SettingsManager {
    // ----------------------------------------------------------------
    //  Instance
    // ----------------------------------------------------------------
    public static bool IsInitializing { get; private set; }
    public SettingsManager() {
        currForeignCode = SaveStorage.GetString(SaveKeys.CurrForeignCode, "da");
        doShowCardDots = SaveStorage.GetBool(SaveKeys.DoShowCardDots, true);
        doShowCardStats = SaveStorage.GetBool(SaveKeys.DoShowCardStats, true);
        doTrimAudioClips = SaveStorage.GetBool(SaveKeys.DoTrimAudioClips, true);
        doNormalizeAudioClips = SaveStorage.GetBool(SaveKeys.DoNormalizeAudioClips, true);
        isTTSOn = SaveStorage.GetBool(SaveKeys.IsTTSOn, true);
        ttsSpeechRate = SaveStorage.GetFloat(SaveKeys.TTSSpeechRate, 1);
        UpdateForeignCodeValues();
    }
    // Instance
    static private SettingsManager instance;
    static public SettingsManager Instance {
        get {
            if (instance == null) {
                // We're ALREADY initializing?? Uh-oh. Return null, or we'll be caught in an infinite loop of recursion!
                if (IsInitializing) {
                    AppDebugLog.LogError("SettingsManager access loop infinite recursion error! It's trying to access itself before it's done being initialized.");
                    return null; // So the program doesn't freeze.
                }
                else {
                    IsInitializing = true;
                    instance = new SettingsManager();
                }
            }
            else {
                IsInitializing = false; // Don't HAVE to update this value at all, but it's nice to for accuracy.
            }
            return instance;
        }
    }


    // Saved Properties
    private string currForeignCode;
    private bool doShowCardDots;
    private bool doShowCardStats;
    private bool doTrimAudioClips;
    private bool doNormalizeAudioClips;
    private bool isTTSOn;
    private float ttsSpeechRate;
    // Unsaved Properties
    public string CurrForeignNameAbbr { get; private set; } // e.g. "Dan"
    public string CurrForeignNameFull { get; private set; } // e.g. "Danish"


    private void UpdateForeignCodeValues() {
        switch (currForeignCode) {
            case "da":
                CurrForeignNameAbbr = "Dan";
                CurrForeignNameFull = "Danish";
                break;
            case "en":
                CurrForeignNameAbbr = "Eng";
                CurrForeignNameFull = "English";
                break;
            case "fr":
                CurrForeignNameAbbr = "Fre";
                CurrForeignNameFull = "French";
                break;
            case "it":
                CurrForeignNameAbbr = "Ita";
                CurrForeignNameFull = "Italian";
                break;
            case "de":
                CurrForeignNameAbbr = "Ger";
                CurrForeignNameFull = "German";
                break;
            case "es":
                CurrForeignNameAbbr = "Spa";
                CurrForeignNameFull = "Spanish";
                break;
            default:
                CurrForeignNameAbbr = "Und";
                CurrForeignNameFull = "Undefined";
                AppDebugLog.LogError("Oops! Foreign code not supported: " + currForeignCode);
                break;
        }
    }

    // ----------------------------------------------------------------
    //  Getters
    // ----------------------------------------------------------------
    public string NativeLanguageCode { get { return "en"; } }
    public string CurrForeignCode { get { return currForeignCode; } }


    // ----------------------------------------------------------------
    //  Getters / Setters
    // ----------------------------------------------------------------
    public void SetCurrForeignCode(string str) {
        currForeignCode = str;
        SaveStorage.SetString(SaveKeys.CurrForeignCode, currForeignCode);
        UpdateForeignCodeValues();
    }
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

}


