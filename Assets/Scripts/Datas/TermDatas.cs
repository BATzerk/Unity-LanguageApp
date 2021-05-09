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
public class Term {
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
    }
    public Term(string native,string foreign,string phonetic) {
        this.native = native;
        this.foreign = foreign;
        this.phonetic = phonetic;
    }


    // Getters
    //public bool IsEmpty() { return String.IsNullOrWhiteSpace(belief) && String.IsNullOrWhiteSpace(debate); }
}





[Serializable]
public class StudySetLibrary {
    public List<StudySet> sets = new List<StudySet>(); // the main sets.
    public StudySet setAced = new StudySet("ACED");
    public StudySet setShelved = new StudySet("SHELVED");
    public StudySet setToValidate = new StudySet("TO VALIDATE");
    public StudySet setWantRecording = new StudySet("WANT RECORDING");
    public StudySet setSourdough = new StudySet("SOURDOUGH SET", true);

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
}
[Serializable]
public class StudySet {
    // Properties and Components
    public bool isSourdoughSet;
    public List<Term> allTerms;
    public string name;
    public int numRoundsFinished;
    public int numRoundsStarted;
    public List<Term> pileYes = new List<Term>();
    public List<Term> pileNo = new List<Term>();
    public List<Term> pileQueue = new List<Term>();
    public List<Term> pileYesesAndNos = new List<Term>(); // the yesses and nos, in order. So we can rewind.

    // Getters
    public int NumDone { get { return pileYesesAndNos.Count; } }
    public int NumTotal { get { return allTerms.Count; } }
    public int NumInCurrentRound { get { return pileQueue.Count + pileYesesAndNos.Count; } }
    public bool IsInProgress { get { return pileQueue.Count>0 || pileYesesAndNos.Count>0; } }
    public Term GetCurrTerm() {
        // Safety check.
        if (pileQueue==null || pileQueue.Count<=0) { Debug.LogError("Oops! Trying to GetCurrTerm, but there's nothing in this StudySet's pileQueue."); return null; }
        return pileQueue[0];
    }
    /// <summary>e.g. "I can. - Jeg kan. - ja kan"</summary>
    public string GetAsExportedString_NativeForeignPhonetic() {
        string str = "";
        foreach (Term term in allTerms) {
            str += term.native + " - " + term.foreign + " - " + term.phonetic;
            str += "\n";
        }
        return str;
    }
    /// <summary>e.g. "Jeg kan. [ja kan] - I can."</summary>
    public string GetAsExportedString_ForeignBracketPhoneticNative() {
        string str = "";
        foreach (Term term in allTerms) {
            str += term.foreign;
            if (term.phonetic.Length > 0) {
                str += " [" + term.phonetic + "]";
            }
            str += " - " + term.foreign;
            str += "\n";
        }
        return str;
    }

    // Initialize
    public StudySet(string name, bool isSourdough=false) {
        this.name = name;
        this.isSourdoughSet = isSourdough;
        this.allTerms = new List<Term>();
    }
    public StudySet(string name, List<Term> terms) {
        this.name = name;
        this.allTerms = terms;
        GiveAllMyTermsRefToMe();
    }
    public StudySet(string name, string allTermsStr) {
        this.name = name;
        string[] termStrings = allTermsStr.Split('\n');

        this.allTerms = new List<Term>();
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
                allTerms.Add(new Term(native, foreign, phonetic));
            }
            catch {
                Debug.LogError("Issue with imported term string: " + str);
            }
        }
        GiveAllMyTermsRefToMe();
    }

    // Doers
    public void GiveAllMyTermsRefToMe() {
        foreach (Term t in allTerms) t.mySet = this; // go through the list so they all know they belong to me.
        //foreach (Term t in pileNo) t.mySet = this; // go through the list so they all know they belong to me.
        //foreach (Term t in pileQueue) t.mySet = this; // go through the list so they all know they belong to me.
        //foreach (Term t in pileYes) t.mySet = this; // go through the list so they all know they belong to me.
        //foreach (Term t in pileYesesAndNos) t.mySet = this; // go through the list so they all know they belong to me.

    }
    public void AddTerm() {
        AddTerm(new Term());
    }
    public void AddTerm(Term newTerm) {
        if (!isSourdoughSet) newTerm.mySet = this; // ONLY set mySet if I'm NOT the SourdoughSet ('cause SD set doesn't actually own any terms).
        allTerms.Add(newTerm);
        // Are we in a round? Great, insert it randomly into the queue!
        if (pileQueue.Count > 0) {
            int randIndex = UnityEngine.Random.Range(0, pileQueue.Count - 1);
            pileQueue.Insert(randIndex, newTerm);
        }
    }
    public void RemoveTerm(Term term) {
        allTerms.Remove(term);
        // Remove from all possible lists.
        pileYes.Remove(term);
        pileNo.Remove(term);
        pileQueue.Remove(term);
        pileYesesAndNos.Remove(term);
    }
    public void ShuffleAndRestartDeck() {
        pileQueue = new List<Term>(allTerms);
        pileYes = new List<Term>();
        pileNo = new List<Term>();
        pileYesesAndNos = new List<Term>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueue);
        numRoundsStarted++; // Increment numRoundsStarted.
    }
    /// Makes a new round, but made up of the "no" pile terms.
    public void RestartNewRound() {
        pileQueue = new List<Term>(pileNo);
        pileYes = new List<Term>();
        pileNo = new List<Term>();
        pileYesesAndNos = new List<Term>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueue);
        numRoundsStarted++; // Increment numRoundsStarted.
    }

    // Events
    public void OnClickCurrTermYes() {
        Term c = GetCurrTerm();
        c.totalYeses++;
        pileQueue.Remove(c);
        pileYes.Add(c);
        pileYesesAndNos.Add(c);
        if (pileQueue.Count == 0) numRoundsFinished++; // Queue empty? We finished the round! :)
    }
    public void OnClickCurrTermNo() {
        Term c = GetCurrTerm();
        c.totalNos++;
        pileQueue.Remove(c);
        pileNo.Add(c);
        pileYesesAndNos.Add(c);
        if (pileQueue.Count == 0) numRoundsFinished++; // Queue empty? We finished the round! :)
    }
    public void RewindOneCard() {
        if (pileYesesAndNos.Count == 0) { Debug.LogError("Oops, trying to rewind, but we have nothing in pileYesesAndNos list."); return; }
        Term prev = pileYesesAndNos[pileYesesAndNos.Count-1];
        pileQueue.Insert(0, prev);
        pileYesesAndNos.Remove(prev);
        if (pileYes.Contains(prev)) {
            pileYes.Remove(prev);
            prev.totalYeses--;
        }
        else {
            pileNo.Remove(prev);
            prev.totalNos--;
        }
    }


}