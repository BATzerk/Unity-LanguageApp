using System;
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
public class Card
{
    public Guid myGuid;
    public string english;
    public string danish;
    public string phonetic;
    public Card(Guid myGuid)
    {
        this.myGuid = myGuid;
        english = "";
        danish = "";
        phonetic = "";
    }
    public Card(Guid myGuid, string english,string danish,string phonetic)
    {
        this.myGuid = myGuid;
        this.english = english;
        this.danish = danish;
        this.phonetic = phonetic;
    }


    // Getters
    //public bool IsEmpty() { return String.IsNullOrWhiteSpace(belief) && String.IsNullOrWhiteSpace(debate); }
}


[Serializable]
public class StudySet
{
    public List<Card> allCards;
    public string name;
    public List<Card> pileYes;
    public List<Card> pileNo;
    public List<Card> pileQueue;
    public List<Card> pileYesesAndNos; // the yesses and nos, in order. So we can rewind.

    // Getters
    public int NumDone { get { return pileYesesAndNos.Count; } }
    public int NumTotal { get { return allCards.Count; } }
    public int NumInCurrentRound { get { return pileQueue.Count + pileYesesAndNos.Count; } }
    public bool IsInProgress { get { return pileQueue!=null && pileQueue.Count>0; } }
    public Card GetCurrCard() {
        // Safety check.
        if (pileQueue==null || pileQueue.Count<=0) { Debug.LogError("Oops! Trying to GetCurrCard, but there's nothing in this StudySet's pileQueue."); return null; }
        return pileQueue[0];
    }

    // Initialize
    public StudySet(string name, List<Card> cards) {
        this.name = name;
        this.allCards = cards;
    }

    // Doers
    public void ShuffleAndRestartDeck() {
        pileQueue = new List<Card>(allCards);
        pileYes = new List<Card>();
        pileNo = new List<Card>();
        pileYesesAndNos = new List<Card>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueue);
    }
    /// Makes a new round, but made up of the "no" pile cards.
    public void RestartNewRound() {
        pileQueue = new List<Card>(pileNo);
        pileYes = new List<Card>();
        pileNo = new List<Card>();
        pileYesesAndNos = new List<Card>();
        // Shuffle 'em!
        GameUtils.Shuffle(pileQueue);
    }

    // Events
    public void OnClickCurrCardYes()
    {
        Card c = GetCurrCard();
        pileQueue.Remove(c);
        pileYes.Add(c);
        pileYesesAndNos.Add(c);
    }
    public void OnClickCurrCardNo()
    {
        Card c = GetCurrCard();
        pileQueue.Remove(c);
        pileNo.Add(c);
        pileYesesAndNos.Add(c);
    }
    public void OnClickRewindOne()
    {
        if (pileYesesAndNos.Count == 0) { Debug.LogError("Oops, trying to rewind, but we have nothing in pileYesesAndNos list."); }
        Card prev = pileYesesAndNos[pileYesesAndNos.Count-1];
        pileQueue.Insert(0, prev);
        if (pileYes.Contains(prev)) pileYes.Remove(prev);
        else pileNo.Remove(prev);
    }


}