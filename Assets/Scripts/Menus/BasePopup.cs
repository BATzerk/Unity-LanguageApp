using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePopup : MonoBehaviour {
    // Getters
    protected DataManager dm { get { return GameManagers.Instance.DataManager; } }
    protected EventManager eventManager { get { return GameManagers.Instance.EventManager; } }
    protected StudySet currSet { get { return dm.CurrSet; } }//set { dm.CurrSet = value; } }




}
