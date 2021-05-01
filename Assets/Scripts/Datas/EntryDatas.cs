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
    //public CustomDate myDate;
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


    // Getters
    //public bool IsEmpty() { return String.IsNullOrWhiteSpace(belief) && String.IsNullOrWhiteSpace(debate); }
}
