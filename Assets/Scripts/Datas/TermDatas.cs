using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public struct CustomDate {
    public int Year, Month, Day;
    public CustomDate(int year, int month, int day) {
        this.Year = year;
        this.Month = month;
        this.Day = day;
    }

    static public CustomDate FromDateTime(DateTime date) {
        return new CustomDate(date.Year, date.Month, date.Day);
    }
}



[Serializable]
public class StudySetLibrary {
    private Dictionary<string, Term> allTermsD = new Dictionary<string, Term>(); // this is NOT serialized in C#!
    public List<Term> allTermsL = new List<Term>(); // this IS serialized
    public List<StudySet> sets = new List<StudySet>(); // the main sets.
    public StudySet setAced;
    public StudySet setShelved;
    public StudySet setToValidate;
    public StudySet setWantRecording;
    public StudySet setToughies;
    public StudySet setSourdough;
    public StudySetLibrary() {
        setAced = new StudySet(this, "ACED");
        setShelved = new StudySet(this, "SHELVED");
        setToValidate = new StudySet(this, "TO VALIDATE");
        setWantRecording = new StudySet(this, "WANT RECORDING");
        setToughies = new StudySet(this, "TOUGHIES", true);
        setSourdough = new StudySet(this, "SOURDOUGH SET", true);
    }

    public void RemakeTermsDictionaryFromList() {
        allTermsD = new Dictionary<string, Term>();
        foreach (Term term in allTermsL) {
            allTermsD.Add(term.myGuid, term);
        }
    }


    public Term GetTerm(string guid) { return allTermsD[guid]; }
    public List<Term> GetOnlyMainTerms() {
        List<Term> mainTerms = new List<Term>();
        foreach (StudySet set in sets) {
            foreach (string termG in set.allTermGs) {
                mainTerms.Add(GetTerm(termG));
            }
        }
        return mainTerms;
    }
    public List<StudySet> GetMainAndSpecialSetsList() {
        List<StudySet> list = new List<StudySet>();
        foreach (StudySet set in sets) list.Add(set);
        list.Add(setAced);
        list.Add(setShelved);
        list.Add(setToValidate);
        list.Add(setWantRecording);
        return list;
    }
    public List<StudySet> GetMainAndSourdoughSets() {
        List<StudySet> list = new List<StudySet>();
        foreach (StudySet set in sets) list.Add(set);
        list.Add(setSourdough);
        return list;
    }
    public List<StudySet> GetEverySingleSet() {
        List<StudySet> list = new List<StudySet>();
        foreach (StudySet set in sets) list.Add(set);
        list.Add(setAced);
        list.Add(setShelved);
        list.Add(setToValidate);
        list.Add(setWantRecording);
        list.Add(setToughies);
        list.Add(setSourdough);
        return list;
    }


    public StudySet GetSetByName(string name) {
        foreach (StudySet set in GetMainAndSpecialSetsList()) {
            if (set.name == name) return set;
        }
        return null;
    }


    /// Adds a term to the main list, and to the provided set.
    public void AddNewTerm(Term term, StudySet set) {
        allTermsD.Add(term.myGuid, term);
        allTermsL.Add(term);
        term.mySet = set; // set ref to set.
        set.AddTerm(term.myGuid);
    }
    /// Removes a term from ALL sets, all references to any sets it was in, AND all terms lists. NOTE: This doesn't delete its audio. Make sure to call this in tandem with calling that.
    public void DeleteTerm(Term term) {
        allTermsD.Remove(term.myGuid);
        allTermsL.Remove(term);
        term.mySet.RemoveTerm(term.myGuid);
        RemoveTermFromRemixSets(term.myGuid);
    }
    public void RemoveTermFromRemixSets(string termG) {
        setSourdough.RemoveTerm(termG);
        setToughies.RemoveTerm(termG);
    }

    public void ChangeSetIndexInList(StudySet set, int indexDelta) {
        if (!sets.Contains(set)) { return; } // Safety check.

        int newIndex = sets.IndexOf(set) + indexDelta;
        newIndex = Mathf.Clamp(newIndex, 0, sets.Count-1);
        sets.Remove(set);
        sets.Insert(newIndex, set);
    }


    public void Debug_ReshuffleAllSets() {
        List<StudySet> allSets = GetEverySingleSet();
        foreach (StudySet set in allSets) {
            set.ShuffleAndRestartDeck();
        }
    }
}



[Serializable]
public class Term {
    public string myGuid;
    public int totalYeses=0; // increments whenever we swipe RIGHT to this term.
    public int totalNos=0;
    public int nSDLeaves = 0; // numSourdoughLeaves (how many times we left the SourdoughSet)
    public int nSDStays = 0; // numSourdoughStays (how many times we stayed in the SourdoughSet, for the next loaf)
    public int nTITS = 0; // numTimesInToughiesSet
    public string native;
    public string foreign;
    public string phonetic;
    public string audio0Guid;
    [NonSerialized] public float yToNPlusRatio=-1; // JUST for Toughies set. Update this MANUALLY if you want to sort us by it!
    [NonSerialized] public StudySet mySet;

    public bool HasAudio0() { return !string.IsNullOrEmpty(audio0Guid); }
    public string MySetName() { return mySet==null ? "NULL" : mySet.name; }


    public Term() {
        native = "";
        foreign = "";
        phonetic = "";
        audio0Guid = "";
        myGuid = Guid.NewGuid().ToString();
    }
    public Term(string native,string foreign,string phonetic) {
        this.native = native;
        this.foreign = foreign;
        this.phonetic = phonetic;
        myGuid = Guid.NewGuid().ToString();
    }


    public void UpdateYToNPlusRatio() {
        yToNPlusRatio = (totalYeses+2) / (totalNos+2);
    }

    // Getters
    //public bool IsEmpty() { return String.IsNullOrWhiteSpace(belief) && String.IsNullOrWhiteSpace(debate); }
}




[Serializable]
public class StudySet {
    // Properties and Components
    [NonSerialized] public bool isRemixSet=false; // if FALSE (usually false), I'm the set my terms belong to. True for Sourdough set.
    [NonSerialized] public bool canIncludeMeInRemixes=true; // true for all main sets; false for Aced, To Validate, etc.
    public List<string> allTermGs; // guids.
    public string name;
    public int numRoundsFinished;
    public int numRoundsStarted;
    public List<string> pileYesG = new List<string>();
    public List<string> pileNoG = new List<string>();
    public List<string> pileQueueG = new List<string>();
    public List<string> pileYesesAndNosG = new List<string>(); // the yesses and nos, in order. So we can rewind.
    // References
    [NonSerialized] private StudySetLibrary myLibrary;

    // Getters
    public int NumDone { get { return pileYesesAndNosG.Count; } }
    public int NumTotal { get { return allTermGs.Count; } }
    public int NumInCurrentRound { get { return pileQueueG.Count + pileYesesAndNosG.Count; } }
    public bool IsInProgress { get { return pileQueueG.Count>0 || pileYesesAndNosG.Count>0; } }
    public Term GetCurrTerm() {
        // Safety check.
        if (pileQueueG==null || pileQueueG.Count<=0) { AppDebugLog.LogError("Oops! Trying to GetCurrTerm, but there's nothing in this StudySet's pileQueue."); return null; }
        return myLibrary.GetTerm(pileQueueG[0]);
    }
    // How many times we've gone through this pack, but based on the TERMS' info. So it can be like 1.228, if we've removed some terms or something (or are midway through it).
    public float GetAverageTimesCompleted() {
        if (NumTotal == 0) { return 0; } // Safety check.
        float termsSum = 0;
        Term term;
        foreach (string termG in allTermGs) {
            term = myLibrary.GetTerm(termG);
            termsSum += term.totalNos + term.totalYeses;
        }
        return termsSum / (float)NumTotal;
    }

    // Initialize
    public StudySet(StudySetLibrary myLibrary, string name, bool isRemixSet=false) {
        this.myLibrary = myLibrary;
        this.name = name;
        this.isRemixSet = isRemixSet;
        this.allTermGs = new List<string>();
    }
    public StudySet(StudySetLibrary myLibrary, string name, string allTermsStr) {
        this.myLibrary = myLibrary;
        this.name = name;
        string[] termStrings = allTermsStr.Split('\n');

        this.allTermGs = new List<string>();
        foreach (string str in termStrings) {
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
                myLibrary.AddNewTerm(new Term(native, foreign, phonetic), this);
            }
            catch {
                AppDebugLog.LogError("Issue with imported term string: " + str);
            }
        }
        SetMyLibraryAndGiveMyTermsRefToMe(myLibrary);
    }

    // Doers
    public void SetMyLibraryAndGiveMyTermsRefToMe(StudySetLibrary library) {
        this.myLibrary = library;
        foreach (string g in allTermGs) myLibrary.GetTerm(g).mySet = this; // go through the list so they all know they belong to me.
    }
    public void AddTerm(string termGuid) {
        Term term = myLibrary.GetTerm(termGuid);
        if (allTermGs.Contains(termGuid)) { AppDebugLog.LogError("Oops! Attempting to add a Term to a list it's already in. Add aborted. Term: " + term.native); return; } // Safety check! ONLY add if it's not already in our list. No dupes.
        if (!isRemixSet) term.mySet = this; // ONLY set mySet if I own my terms! (So, don't do it for the Remix sets.)
        allTermGs.Add(termGuid);
        // Are we in a round? Great, insert it randomly into the queue!
        if (pileQueueG.Count > 0) {
            int randIndex = UnityEngine.Random.Range(0, pileQueueG.Count - 1);
            pileQueueG.Insert(randIndex, termGuid);
        }
    }
    public void RemoveTerm(string termGuid) {
        allTermGs.Remove(termGuid);
        // Remove from all possible lists.
        pileYesG.Remove(termGuid);
        pileNoG.Remove(termGuid);
        pileQueueG.Remove(termGuid);
        pileYesesAndNosG.Remove(termGuid);
    }
    public void ShuffleAndRestartDeck() {
        pileQueueG = new List<string>(allTermGs);
        pileYesG = new List<string>();
        pileNoG = new List<string>();
        pileYesesAndNosG = new List<string>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueueG);
        numRoundsStarted++; // Increment numRoundsStarted.
    }
    /// Makes a new round, but made up of the "no" pile terms.
    public void StartNewRound() {
        pileQueueG = new List<string>(pileNoG);
        pileYesG = new List<string>();
        pileNoG = new List<string>();
        pileYesesAndNosG = new List<string>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueueG);
        numRoundsStarted++; // Increment numRoundsStarted.
    }

    public void ReplaceAllTermsAndShuffleStartNewRound(List<Term> newTerms) {
        if (!isRemixSet) { AppDebugLog.LogError("Whoa! Trying to ReplaceAndShuffleAllTerms on a set that OWNS its terms. This function is only meant for remix sets, which DON'T own their terms, like the Sourdough/Toughies sets."); }
        // Clear lists.
        allTermGs.Clear();
        pileYesG.Clear();
        pileNoG.Clear();
        pileQueueG.Clear();
        pileYesesAndNosG.Clear();
        // Replenish me.
        for (int i=0; i<newTerms.Count; i++) {
            allTermGs.Add(newTerms[i].myGuid);
        }
        // Make that sweet, sweet new round.
        ShuffleAndRestartDeck();
    }

    // Events
    public void OnClickCurrTermYes() {
        Term c = GetCurrTerm();
        c.totalYeses++;
        pileQueueG.Remove(c.myGuid);
        pileYesG.Add(c.myGuid);
        pileYesesAndNosG.Add(c.myGuid);
        if (pileQueueG.Count == 0) numRoundsFinished++; // Queue empty? We finished the round! :)
    }
    public void OnClickCurrTermNo() {
        Term c = GetCurrTerm();
        c.totalNos++;
        pileQueueG.Remove(c.myGuid);
        pileNoG.Add(c.myGuid);
        pileYesesAndNosG.Add(c.myGuid);
        if (pileQueueG.Count == 0) numRoundsFinished++; // Queue empty? We finished the round! :)
    }
    public void RewindOneCard() {
        if (pileYesesAndNosG.Count == 0) { AppDebugLog.LogError("Oops, trying to rewind, but we have nothing in pileYesesAndNos list."); return; }
        string prevG = pileYesesAndNosG[pileYesesAndNosG.Count-1];
        Term prevT = myLibrary.GetTerm(prevG);
        pileQueueG.Insert(0, prevG);
        pileYesesAndNosG.Remove(prevG);
        if (pileYesG.Contains(prevG)) {
            pileYesG.Remove(prevG);
            prevT.totalYeses--;
        }
        else {
            pileNoG.Remove(prevG);
            prevT.totalNos--;
        }
    }


}