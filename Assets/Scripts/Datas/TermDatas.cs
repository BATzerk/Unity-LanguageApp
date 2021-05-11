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
    public StudySet setAced;// = new StudySet(this, "ACED");
    public StudySet setShelved;// = new StudySet(this, "SHELVED");
    public StudySet setToValidate;// = new StudySet("TO VALIDATE");
    public StudySet setWantRecording;// = new StudySet("WANT RECORDING");
    public StudySet setSourdough;// = new StudySet("SOURDOUGH SET", true);
    public StudySetLibrary() {
        setAced = new StudySet(this, "ACED");
        setShelved = new StudySet(this, "SHELVED");
        setToValidate = new StudySet(this, "TO VALIDATE");
        setWantRecording = new StudySet(this, "WANT RECORDING");
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
    /// Removes a term from the main list, and all references to its set.
    public void RemoveTerm(Term term) {
        allTermsD.Remove(term.myGuid);
        allTermsL.Remove(term);
        term.mySet.RemoveTerm(term.myGuid);
        // TODO: Remove from sourdough set
    }

    public void ChangeSetIndexInList(StudySet set, int indexDelta) {
        if (!sets.Contains(set)) { return; } // Safety check.

        int newIndex = sets.IndexOf(set) + indexDelta;
        newIndex = Mathf.Clamp(newIndex, 0, sets.Count-1);
        sets.Remove(set);
        sets.Insert(newIndex, set);
    }
}



[Serializable]
public class Term {
    public string myGuid;
    public int totalYeses=0; // increments whenever we swipe RIGHT to this term.
    public int totalNos=0;
    public int nSDLeaves = 0; // numSourdoughLeaves (how many times we left the SourdoughSet)
    public int nSDStays = 0; // numSourdoughStays (how many times we stayed in the SourdoughSet, for the next loaf)
    public string native;
    public string foreign;
    public string phonetic;
    public string audio0Guid;
    [NonSerialized] public StudySet mySet;

    public bool HasAudio0() { return !string.IsNullOrEmpty(audio0Guid); }


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


    // Getters
    //public bool IsEmpty() { return String.IsNullOrWhiteSpace(belief) && String.IsNullOrWhiteSpace(debate); }
}




[Serializable]
public class StudySet {
    // Properties and Components
    public bool isSourdoughSet;
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
        if (pileQueueG==null || pileQueueG.Count<=0) { Debug.LogError("Oops! Trying to GetCurrTerm, but there's nothing in this StudySet's pileQueue."); return null; }
        return myLibrary.GetTerm(pileQueueG[0]);
    }

    // Initialize
    public StudySet(StudySetLibrary myLibrary, string name, bool isSourdough=false) {
        this.myLibrary = myLibrary;
        this.name = name;
        this.isSourdoughSet = isSourdough;
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
                Debug.LogError("Issue with imported term string: " + str);
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
        if (!isSourdoughSet) term.mySet = this; // ONLY set mySet if I'm NOT the SourdoughSet ('cause SD set doesn't actually own any terms).
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
    public void RestartNewRound() {
        pileQueueG = new List<string>(pileNoG);
        pileYesG = new List<string>();
        pileNoG = new List<string>();
        pileYesesAndNosG = new List<string>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueueG);
        numRoundsStarted++; // Increment numRoundsStarted.
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
        if (pileYesesAndNosG.Count == 0) { Debug.LogError("Oops, trying to rewind, but we have nothing in pileYesesAndNos list."); return; }
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