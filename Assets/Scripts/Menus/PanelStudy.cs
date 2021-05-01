using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelStudy : BasePanel
{
    // Components
    [SerializeField] private TextMeshProUGUI t_date = null;
    [SerializeField] public  RectTransform rt_entriesParent = null;
    private List<IBEntryView> entryViews = new List<IBEntryView>();
    // Properties
    private DateTime selectedDate;



    // ================================================================
    //  Start
    // ================================================================
    void Start() {
        selectedDate = DateTime.Today;

        RefreshVisuals();
    }
    private void RefreshVisuals() {
        // Refresh date text
        t_date.text = TextUtils.MediumDateString(selectedDate);


        // Load datas.
        List<IBData> loadedDatas = new List<IBData>();
        int index = 0;
        while (true)
        {
            IBData data = LoadIBData(selectedDate, index);
            if (data == null || data.IsEmpty()) { break; } // No entry? Quit loop.
            loadedDatas.Add(data);
            if (index++ > 99) { break; } // Safety check.
        }



        // Destroy entryViews.
        for (int i=entryViews.Count-1; i>=0; --i) {
            GameObject.Destroy(entryViews[i].gameObject);
        }
        // Populate entryViews.
        entryViews = new List<IBEntryView>();
        for (int i=0; i<loadedDatas.Count; i++)
        {
            IBEntryView newView = Instantiate(ResourcesHandler.Instance.IBEntryView).GetComponent<IBEntryView>();
            newView.Initialize(this, loadedDatas[i]);
            entryViews.Add(newView);
        }
    }




    private static IBData LoadIBData(DateTime date, int index)
    {
        string saveKey = SaveKeys.IBEntry(CustomDate.FromDateTime(date), index);
        string saveDataString = SaveStorage.GetString(saveKey);
        return JsonUtility.FromJson<IBData>(saveDataString);
    }
    public static void SaveIBData(IBData data)
    {
        string saveKey = SaveKeys.IBEntry(data.myDate, data.myIndex);
        SaveStorage.SetString(saveKey, JsonUtility.ToJson(data));
    }



    void Update()
    {
        //Unity HACK. To make sure components are arranged correctly.
        if (Time.frameCount % 10 == 0)
        {
            rt_entriesParent.sizeDelta = new Vector2(rt_entriesParent.sizeDelta.x + 1, rt_entriesParent.sizeDelta.y);
            rt_entriesParent.sizeDelta = new Vector2(rt_entriesParent.sizeDelta.x - 1, rt_entriesParent.sizeDelta.y);
        }
    }


    // ================================================================
    //  Events
    // ================================================================
    public void AddIBEntryView()
    {
        IBEntryView newView = Instantiate(ResourcesHandler.Instance.IBEntryView).GetComponent<IBEntryView>();
        newView.Initialize(this, new IBData(CustomDate.FromDateTime(selectedDate), entryViews.Count));
        entryViews.Add(newView);
    }




}
