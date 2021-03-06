using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BasePanel : MonoBehaviour {
    // Getters
    protected DataManager dm { get { return GameManagers.Instance.DataManager; } }
    protected EventManager eventManager { get { return GameManagers.Instance.EventManager; } }
    protected StudySet currSet { get { return dm.CurrSet; } }//set { dm.CurrSet = value; } }

    abstract public PanelTypes MyPanelType { get; }


    public void SetVisibility(bool _isVisible) {
        this.gameObject.SetActive(_isVisible);
        if (_isVisible) OnOpened();
        else OnClosed();
    }

    virtual protected void OnOpened() { }
    virtual protected void OnClosed() { }
}
