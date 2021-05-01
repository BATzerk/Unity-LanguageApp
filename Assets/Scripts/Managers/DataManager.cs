using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager {
    // Properties
    public StudySetLibrary library;
    //public List<StudySet> studySets=new List<StudySet>();


    // ----------------------------------------------------------------
    //  Getters
    // ----------------------------------------------------------------


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public DataManager() {
        ReloadStudySetLibrary();
    }
    public void ReloadStudySetLibrary() {
        string jsonString = SaveStorage.GetString(SaveKeys.StudySetLibrary);
        library = JsonUtility.FromJson<StudySetLibrary>(jsonString);
    }
    public void SaveStudySetLibrary() {
        string jsonString = JsonUtility.ToJson(library);
        Debug.Log(jsonString);
        SaveStorage.SetString(SaveKeys.StudySetLibrary, jsonString);
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void ClearAllSaveData() {
		// NOOK IT
		SaveStorage.DeleteAll ();
		//ReloadLevels ();
		Debug.Log ("All SaveStorage CLEARED!");
	}
    public void ReplaceAllStudySetsWithPremadeHardcodedOnes() {
        // TEMP!
        List<Term> cards0 = new List<Term>();
        List<Term> cards1 = new List<Term>();
        List<Term> cardsNumbers = new List<Term>();
        cards0.Add(new Term("House", "Hus", "hoos"));
        cards0.Add(new Term("Car", "Bil", "beel"));
        cards1.Add(new Term("Green", "Grøn", "grøn"));
        cards1.Add(new Term("Blue", "Blå", "blå"));
        cardsNumbers.Add(new Term("Zero", "Nul", "nul"));
        cardsNumbers.Add(new Term("One", "En", "en"));
        cardsNumbers.Add(new Term("Two", "To", "to"));
        cardsNumbers.Add(new Term("Three", "Tre", "tRe"));
        cardsNumbers.Add(new Term("Four", "Fire", "feeah"));
        cardsNumbers.Add(new Term("Five", "Fem", "fem"));
        cardsNumbers.Add(new Term("Six", "Seks", "seks"));
        cardsNumbers.Add(new Term("Seven", "Syv", "sYv"));
        cardsNumbers.Add(new Term("Eight", "Otte", "otte"));
        cardsNumbers.Add(new Term("Nine", "Ni", "nee"));
        cardsNumbers.Add(new Term("Ten", "Ti", "tee"));

        library = new StudySetLibrary();
        library.sets = new List<StudySet>();
        library.sets.Add(new StudySet("Objects", cards0));
        library.sets.Add(new StudySet("Colors", cards1));
        library.sets.Add(new StudySet("Numbers", cardsNumbers));

        SaveStudySetLibrary();
    }



}


