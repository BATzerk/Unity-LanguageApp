using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelStudyChooseSet : BasePanel
{
    // Components
    [SerializeField] public  RectTransform rt_tileViewsContent = null; // all the TileViews go on here.
    private List<StudySetTileView> tileViews=new List<StudySetTileView>();
    // References
    [SerializeField] private MenuController menuController;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ================================================================
    //  Start
    // ================================================================
    void Start() {
        RemakeTileViews();
    }

    private void RemakeTileViews()
    {
        // Destroy 'em all.
        for (int i=tileViews.Count-1; i>=0; i--) {
            Destroy(tileViews[i]);
        }
        tileViews = new List<StudySetTileView>();

        // Make 'em all.
        float tempY = -20;
        foreach (StudySet set in dm.studySets)
        {
            StudySetTileView newView = Instantiate(ResourcesHandler.Instance.StudySetTileView).GetComponent<StudySetTileView>();
            newView.Initialize(this, rt_tileViewsContent, set, tempY);
            tileViews.Add(newView);
            tempY -= 70;
        }
    }


    // ================================================================
    //  Events
    // ================================================================
    public void OnClickedTileView(StudySet studySet) {
        menuController.OpenPanel_StudyFlashcards(studySet);
    }



    //    RefreshVisuals();
    //}
    //private void RefreshVisuals() {
    //    // Refresh date text
    //    t_date.text = TextUtils.MediumDateString(selectedDate);


    //    // Load datas.
    //    List<IBData> loadedDatas = new List<IBData>();
    //    int index = 0;
    //    while (true)
    //    {
    //        IBData data = LoadIBData(selectedDate, index);
    //        if (data == null || data.IsEmpty()) { break; } // No entry? Quit loop.
    //        loadedDatas.Add(data);
    //        if (index++ > 99) { break; } // Safety check.
    //    }



    //    // Destroy entryViews.
    //    for (int i=entryViews.Count-1; i>=0; --i) {
    //        GameObject.Destroy(entryViews[i].gameObject);
    //    }
    //    // Populate entryViews.
    //    entryViews = new List<IBEntryView>();
    //    for (int i=0; i<loadedDatas.Count; i++)
    //    {
    //        IBEntryView newView = Instantiate(ResourcesHandler.Instance.IBEntryView).GetComponent<IBEntryView>();
    //        newView.Initialize(this, loadedDatas[i]);
    //        entryViews.Add(newView);
    //    }
    //}




    //private static IBData LoadIBData(DateTime date, int index)
    //{
    //    string saveKey = SaveKeys.IBEntry(CustomDate.FromDateTime(date), index);
    //    string saveDataString = SaveStorage.GetString(saveKey);
    //    return JsonUtility.FromJson<IBData>(saveDataString);
    //}
    //public static void SaveIBData(IBData data)
    //{
    //    string saveKey = SaveKeys.IBEntry(data.myDate, data.myIndex);
    //    SaveStorage.SetString(saveKey, JsonUtility.ToJson(data));
    //}



    //void Update()
    //{
    //    //Unity HACK. To make sure components are arranged correctly.
    //    if (Time.frameCount % 10 == 0)
    //    {
    //        rt_entriesParent.sizeDelta = new Vector2(rt_entriesParent.sizeDelta.x + 1, rt_entriesParent.sizeDelta.y);
    //        rt_entriesParent.sizeDelta = new Vector2(rt_entriesParent.sizeDelta.x - 1, rt_entriesParent.sizeDelta.y);
    //    }
    //}


    //// ================================================================
    ////  Events
    //// ================================================================




}
