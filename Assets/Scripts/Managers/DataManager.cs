using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager {
    // Properties
    private static float SourdoughHoursBetweenRefill = 4;
    private static int SourdoughMaxTerms = 20;
    private static int NumToughiesInSet = 20;
    public StudySetLibrary library;
    private StudySet _currSet; // whatever set we're editing, or playing.



    public StudySet CurrSet { get { return _currSet; } }
    public void SetCurrSet(StudySet set) {
        _currSet = set;
        SaveStorage.SetString(SaveKeys.LastStudySetOpenName, _currSet.name);
    }


    // ----------------------------------------------------------------
    //  Getters
    // ----------------------------------------------------------------
    public bool IsSourdoughSet(StudySet set) {
        return library.setSourdough == set;
    }
    public float GetHoursUntilNextSourdoughRefill() {
        DateTime timeLastRefilled = SaveStorage.GetDateTime(SaveKeys.SourdoughTimeLastRefilled);
        DateTime timeToNextRefill = timeLastRefilled.AddHours(SourdoughHoursBetweenRefill);
        TimeSpan timeSpan = timeToNextRefill - DateTime.Now;
        return (float)timeSpan.TotalHours;
    }
    public bool IsTimeToRefillSourdoughSet() {
        return GetHoursUntilNextSourdoughRefill() <= 0;
    }

    /// <summary>e.g. "I can. - Jeg kan. - ja kan"</summary>
    public string GetStudySetExportedString_NativeForeignPhonetic(StudySet set) {
        string str = "";
        foreach (string guid in set.allTermGs) {
            Term term = library.GetTerm(guid);
            str += term.native + " - " + term.foreign + " - " + term.phonetic;
            str += "\n";
        }
        return str;
    }
    /// <summary>e.g. "Jeg kan. [ja kan] - I can."</summary>
    public string GetStudySetExportedString_ForeignBracketPhoneticNative(StudySet set) {
        string str = "";
        foreach (string guid in set.allTermGs) {
            Term term = library.GetTerm(guid);
            str += term.foreign;
            if (term.phonetic.Length > 0) {
                str += " [" + term.phonetic + "]";
            }
            str += " - " + term.native;
            str += "\n";
        }
        return str;
    }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public DataManager() {
        ReloadStudySetLibrary();
    }
    public void ReloadStudySetLibrary() {
        string libSaveKey = SaveKeys.StudySetLibrary(SettingsManager.Instance.CurrForeignCode);
        //// QQQ if we can't find the correct key, use the OLD one. Should just be for ONE push to my phone.
        //if (SettingsManager.Instance.CurrForeignCode=="da" && !SaveStorage.HasKey(libSaveKey)) {
        //    if (SaveStorage.HasKey("StudySetLibrary")) {
        //        libSaveKey = "StudySetLibrary";
        //    }
        //}

        // NO save data?! Ok, default to Quizlet hardcoded ones! :)
        if (!SaveStorage.HasKey(libSaveKey)) {
            //ReplaceAllStudySetsWithPremadeHardcodedOnes();
            library = new StudySetLibrary();
        }
        // Otherwise, YES load what's been saved!
        else {
            string jsonString = SaveStorage.GetString(libSaveKey);
            library = JsonUtility.FromJson<StudySetLibrary>(jsonString);
            // Convert the unpacked term list to our efficient dictionary.
            library.RemakeTermsDictionaryFromList();

            // Reaffiliate all terms with their sets.
            foreach (StudySet set in library.GetMainAndBenchedSetsList()) {
                set.SetMyLibraryAndGiveMyTermsRefToMe(library);
            }
        }

        // Hardcoded. Set some properties manually.
        //library.setToughies.isRemixSet = true;
        //library.setSourdough.isRemixSet = true;
        //library.setAced.canIncludeMeInRemixes = false;
        //library.setInQueue.canIncludeMeInRemixes = false;
        //library.setShelved.canIncludeMeInRemixes = false;
        //library.setToValidate.canIncludeMeInRemixes = false;
        //library.setWantRecording.canIncludeMeInRemixes = false;
        //foreach (StudySet set in library.sets) {
        //    set.canIncludeMeInRemixes = true;
        //}

        // DEBUG. Print any terms that don't belong to the set they're in.
        foreach (StudySet set in library.sets) {
            for (int i=set.allTermGs.Count-1; i>=0; --i) {
                string termG = set.allTermGs[i];
                Term term = library.GetTerm(termG);
                if (term.mySet != set) {
                    AppDebugLog.LogError("MISMATCH BETWEEN SETS: " + set.name + ", " + term.MySetName() + ",   " + term.native);
                    set.RemoveTerm(termG); // Remove it from this set! ONLY trust the Term's set.
                }
            }
        }
    }
    public void SaveStudySetLibrary() {
        string jsonString = JsonUtility.ToJson(library);
        string libSaveKey = SaveKeys.StudySetLibrary(SettingsManager.Instance.CurrForeignCode);
        SaveStorage.SetString(libSaveKey, jsonString);
        Debug.Log("SAVED STUDYSET LIBRARY: " + jsonString);
    }




    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void ClearAllSaveData() {
		// NOOK IT
		SaveStorage.DeleteAll();
		Debug.Log ("All SaveStorage CLEARED!");
        ReloadStudySetLibrary();
        SceneHelper.ReloadScene();
	}



    // ----------------------------------------------------------------
    //  Set Doers
    // ----------------------------------------------------------------
    public void CopyAllSetsToClipboard() {
        string wholeStr = "";
        List<StudySet> everySet = library.GetMainAndBenchedSetsList();
        foreach (StudySet set in everySet) {
            wholeStr += set.name + "\n";
            wholeStr += GetStudySetExportedString_ForeignBracketPhoneticNative(set);
            wholeStr += "\n\n";
        }
        GameUtils.CopyToClipboard(wholeStr);
    }

    public void MoveTermToSet(Term term, StudySet newSet) {
        // Swap its set.
        StudySet prevSet = term.mySet;
        prevSet.RemoveTerm(term.myGuid);
        newSet.AddTerm(term.myGuid);
        // If it's not a main set, then also attempt to remove from the remix sets.
        if (!newSet.canIncludeMeInRemixes) {
            library.RemoveTermFromRemixSets(term.myGuid);
        }
        // Save!
        SaveStudySetLibrary();
    }
    public void DeleteTerm(Term term) {
        DeleteTermAudio0(term); // delete any audio files!
        library.DeleteTerm(term); // remove it from all sets.
        SaveStudySetLibrary(); // Save!
    }
    public void DeleteSet(StudySet set) {
        // First, delete all audio from any terms.
        foreach (string termG in set.allTermGs) {
            Term term = library.GetTerm(termG);
            DeleteTermAudio0(term);
        }
        // Now, remove the set from the library!
        library.sets.Remove(set);
    }



    // ----------------------------------------------------------------
    //  Sourdough Handling
    // ----------------------------------------------------------------
    public void RefillSourdoughSet() {
        // FIRST, update the terms in the sourdough set!
        StudySet setSD = library.setSourdough;
        List<string> termsLeave = new List<string>();
        List<string> termsStay = new List<string>();
        foreach (string termG in setSD.allTermGs) {
            if (setSD.pileNoG.Contains(termG)
             || setSD.pileQueueG.Contains(termG)) termsStay.Add(termG);
            else termsLeave.Add(termG);
        }
        foreach (string termG in termsLeave) library.GetTerm(termG).nSDLeaves++;
        foreach (string termG in termsStay) library.GetTerm(termG).nSDStays++;
        // Remove the yes-ed terms entirely from the set!
        for (int i=0; i<termsLeave.Count; i++) {
            setSD.RemoveTerm(termsLeave[i]);
        }

        // Refill the holes!
        int numToAdd = SourdoughMaxTerms - setSD.allTermGs.Count;
        // Make a safe copy-list of all terms!
        List<Term> allTerms = library.GetOnlyMainTerms();
        // Remove terms that are gonna STAY in Sourdough from the all-terms list, so we don't accidentally add the same ones again.
        for (int i=0; i<termsStay.Count; i++) {
            Term term = library.GetTerm(termsStay[i]);
            allTerms.Remove(term);
        }
        // Shuffle them all, then sort them by sourdough wins.
        GameUtils.Shuffle(allTerms);
        //allTerms = allTerms.OrderBy(c => c.yToNPlusRatio).ToList(); // TEST! Put the hard ones in first.
        allTerms = allTerms.OrderBy(c => c.nSDLeaves).ToList();

        // Add the right amount to the set.
        for (int i=0; i<numToAdd && i<allTerms.Count; i++) {
            setSD.AddTerm(allTerms[i].myGuid);
        }
        setSD.ShuffleAndRestartDeck();

        // Save library
        SaveStudySetLibrary();

        // Finally, save WHEN we last refilled (now)!
        SaveStorage.SetDateTime(SaveKeys.SourdoughTimeLastRefilled, DateTime.Now);
    }
    public void Debug_RebuildSourdoughSet() {
        StudySet setSD = library.setSourdough;
        // Remove all terms from Sourdough set.
        for (int i=setSD.allTermGs.Count-1; i>=0; --i) {
            setSD.RemoveTerm(setSD.allTermGs[i]);
        }
        // Now just refill it like normal.
        RefillSourdoughSet();
    }




    public void RemakeToughiesSet() {
        // Make, then shuffle (for proper, Xtra randomness), a safe copy-list of all terms!
        List<Term> allTerms = library.GetOnlyMainTerms();
        GameUtils.Shuffle(allTerms);
        // NOW (and only now) update the YToNPlusRatio.
        foreach (Term term in allTerms) {
            term.UpdateYToNPlusRatio();
        }
        // FIRST, sort by y-to-n-plus ratio.
        allTerms = allTerms.OrderBy(c => c.yToNPlusRatio).ToList();
        // NEXT, sort by how many times they've been IN the Toughies Set already!
        allTerms = allTerms.OrderBy(c => c.nTITS).ToList();


        // MAYBE: Now, cull out all the ones that AREN'T beyond the 1.7 threshold.

        // Pluck out a good 20 of 'em, and let them *know* they're in the Toughies Set, officially.
        List<Term> setTerms = new List<Term>();
        for (int i=0; i<NumToughiesInSet&&i<allTerms.Count; i++) {
            allTerms[i].nTITS ++;
            setTerms.Add(allTerms[i]);
        }
        // Shuffle what we got.
        library.setToughies.ReplaceAllTermsAndShuffleStartNewRound(setTerms);

        // Save library
        SaveStudySetLibrary();
    }








    // ----------------------------------------------------------------
    //  Audio
    // ----------------------------------------------------------------
    public void DeleteTermAudio0(Term term) {
        // No audio? Get outta here.
        if (!term.HasAudio0()) { //string.IsNullOrEmpty(term.audio0Guid)
            return;
        }
        // Delete the actual file!
        string clipPath = SaveKeys.TermAudioClip0(term.audio0Guid);
        File.Delete(clipPath);
        // Null out the Term's guid, and save our library!
        term.audio0Guid = "";
        SaveStudySetLibrary();
    }






    // ----------------------------------------------------------------
    //  DEBUG
    // ----------------------------------------------------------------
    public void ReplaceAllStudySetsWithPremadeHardcodedOnes() {
        library = new StudySetLibrary();

        // Pull what I need from this text file.
        TextAsset textAsset = Resources.Load<TextAsset>("Data/AllSetsBackup");
        string[] allLines = textAsset.text.Split('\n');
        bool wasPrevLineEmpty = true; // so we know if we've reached the name of a new StudySet, which always comes after a blank line.

        StudySet currSet=null;
        for (int i=0; i<allLines.Length; i++) {
            string line = allLines[i];
            if (string.IsNullOrWhiteSpace(line)) { // Skip blank lines.
                wasPrevLineEmpty = true;
                continue;
            }
            if (wasPrevLineEmpty) { // We made it to a new set! Set the name, and move on to the next line.
                string setName = line;
                switch (setName) {
                    case "ACED": currSet = library.setAced; break;
                    case "IN QUEUE": currSet = library.setInQueue; break;
                    case "SHELVED": currSet = library.setShelved; break;
                    case "TO VALIDATE": currSet = library.setToValidate; break;
                    case "WANT RECORDING": currSet = library.setWantRecording; break;
                    default:
                        currSet = new StudySet(library, line, SetTypes.Regular);
                        library.sets.Add(currSet); // of course, add regular sets to the library right away.
                        break;
                }
                wasPrevLineEmpty = false;
                continue;
            }
            AddTermFromExportedString(currSet, line);
            wasPrevLineEmpty = false;
        }

        SaveStudySetLibrary();
    }
    //private void AddHardcodedSetFromString(string setName, string allTermsStr) {
    //    // Make and add empty new set.
    //    StudySet newSet = new StudySet(library, setName, SetTypes.Regular);
    //    library.sets.Add(newSet);

    //    // Make all the new terms.
    //    string[] termStrings = allTermsStr.Split('\n');
    //    foreach (string str in termStrings) {
    //        AddTermFromExportedString(newSet, str);
    //    }
    //}

    private void AddTermFromExportedString(StudySet set, string str) {
        try {
            int splitIndex;
            if (str.Contains(" — ")) splitIndex = str.IndexOf(" — "); // use double-sized hyphen, if that's how it's (optionally) formatted.
            else splitIndex = str.IndexOf(" - "); // otherwise, split by the regular hyphen.
            string native = str.Substring(splitIndex + 3);
            string foreign = str.Substring(0, splitIndex);
            string phonetic = "";
            // pull out the phonetic pronunciation
            int lbIndex = foreign.LastIndexOf('['); // left bracket index
            int rbIndex = foreign.LastIndexOf(']'); // right bracket index
            if (rbIndex == foreign.Length - 1) { // if this one ENDS in a phonetic explanation...
                phonetic = foreign.Substring(lbIndex + 1);
                phonetic = phonetic.Substring(0, phonetic.Length - 1); // get rid of that last ] char.
                foreign = foreign.Substring(0, lbIndex - 1);
            }
            //int splitIndexA = str.IndexOf(" — ");
            //int splitIndexB = str.LastIndexOf(" — ");
            //string native = str.Substring(splitIndex + 3);
            //string foreign = str.Substring(0, splitIndex);
            //string phonetic = "";
            //// pull out the phonetic pronunciation
            //int lbIndex = foreign.LastIndexOf('['); // left bracket index
            //int rbIndex = foreign.LastIndexOf(']'); // right bracket index
            //if (rbIndex == foreign.Length - 1) { // if this one ENDS in a phonetic explanation...
            //    phonetic = foreign.Substring(lbIndex + 1);
            //    phonetic = phonetic.Substring(0, phonetic.Length - 1); // get rid of that last ] char.
            //    foreign = foreign.Substring(0, lbIndex - 1);
            //}
            Term newTerm = new Term(native, foreign, phonetic);
            // Add this term to the library!
            library.AddNewTerm(newTerm, set);
        }
        catch {
            AppDebugLog.LogError("Issue with imported term string: " + str);
        }
    }


    }


