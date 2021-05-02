﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct CustomDate
{
    public int Year, Month, Day;
    public CustomDate(int year, int month, int day)
    {
        this.Year = year;
        this.Month = month;
        this.Day = day;
    }

    static public CustomDate FromDateTime(DateTime date)
    {
        return new CustomDate(date.Year, date.Month, date.Day);
    }
}


[Serializable]
public class Term
{
    public string english;
    public string danish;
    public string phonetic;
    [NonSerialized] public StudySet mySet;
    public Term() {
        english = "";
        danish = "";
        phonetic = "";
    }
    public Term(string english,string danish,string phonetic) {
        this.english = english;
        this.danish = danish;
        this.phonetic = phonetic;
    }


    // Getters
    //public bool IsEmpty() { return String.IsNullOrWhiteSpace(belief) && String.IsNullOrWhiteSpace(debate); }
}

[Serializable]
public class StudySetLibrary {
    public List<StudySet> sets = new List<StudySet>();
}
[Serializable]
public class StudySet
{
    public List<Term> allTerms;
    public string name;
    public List<Term> pileYes;
    public List<Term> pileNo;
    public List<Term> pileQueue;
    public List<Term> pileYesesAndNos; // the yesses and nos, in order. So we can rewind.

    // Getters
    public int NumDone { get { return pileYesesAndNos.Count; } }
    public int NumTotal { get { return allTerms.Count; } }
    public int NumInCurrentRound { get { return pileQueue.Count + pileYesesAndNos.Count; } }
    public bool IsInProgress { get { return pileQueue!=null && pileQueue.Count>0; } }
    public Term GetCurrTerm() {
        // Safety check.
        if (pileQueue==null || pileQueue.Count<=0) { Debug.LogError("Oops! Trying to GetCurrTerm, but there's nothing in this StudySet's pileQueue."); return null; }
        return pileQueue[0];
    }
    public string GetAsExportedString() {
        string str = "";
        foreach (Term term in allTerms) {
            str += term.english + " - " + term.danish + " - " + term.phonetic;
            str += "\n";
        }
        return str;
    }

    // Initialize
    public StudySet(string name, List<Term> terms) {
        this.name = name;
        this.allTerms = terms;
        foreach (Term t in allTerms) t.mySet = this; // go through the list so they all know they belong to me.
    }
    public StudySet(string name, string allTermsStr) {
        this.name = name;
        string[] termStrings = allTermsStr.Split('\n');

        this.allTerms = new List<Term>();
        foreach (string str in termStrings) {
            try {
                int splitIndex = str.IndexOf(" - ");
                string native = str.Substring(splitIndex+3);
                string foreign = str.Substring(0, splitIndex);
                string phonetic = "";
                // pull out the phonetic pronunciation
                int lbIndex = foreign.LastIndexOf('['); // left bracket index
                int rbIndex = foreign.LastIndexOf(']'); // right bracket index
                if (rbIndex == foreign.Length-1) { // if this one ENDS in a phonetic explanation...
                    phonetic = foreign.Substring(lbIndex+1);
                    phonetic = phonetic.Substring(0, phonetic.Length - 1); // get rid of that last ] char.
                    foreign = foreign.Substring(0, lbIndex - 1);
                }
                allTerms.Add(new Term(native, foreign, phonetic));
            }
            catch {
                Debug.LogError("Issue with imported term string: " + str);
            }
        }
        foreach (Term t in allTerms) t.mySet = this; // go through the list so they all know they belong to me.
    }

    // Doers
    public void AddTerm() {
        Term newTerm = new Term();
        newTerm.mySet = this;
        allTerms.Add(newTerm);
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
    }
    /// Makes a new round, but made up of the "no" pile terms.
    public void RestartNewRound() {
        pileQueue = new List<Term>(pileNo);
        pileYes = new List<Term>();
        pileNo = new List<Term>();
        pileYesesAndNos = new List<Term>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueue);
    }

    // Events
    public void OnClickCurrTermYes()
    {
        Term c = GetCurrTerm();
        pileQueue.Remove(c);
        pileYes.Add(c);
        pileYesesAndNos.Add(c);
    }
    public void OnClickCurrTermNo()
    {
        Term c = GetCurrTerm();
        pileQueue.Remove(c);
        pileNo.Add(c);
        pileYesesAndNos.Add(c);
    }
    public void RewindOneCard() {
        if (pileYesesAndNos.Count == 0) { Debug.LogError("Oops, trying to rewind, but we have nothing in pileYesesAndNos list."); return; }
        Term prev = pileYesesAndNos[pileYesesAndNos.Count-1];
        pileQueue.Insert(0, prev);
        pileYesesAndNos.Remove(prev);
        if (pileYes.Contains(prev)) pileYes.Remove(prev);
        else pileNo.Remove(prev);
    }


}