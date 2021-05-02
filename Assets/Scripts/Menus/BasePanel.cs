using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{

    public void SetVisibility(bool _isVisible) {
        this.gameObject.SetActive(_isVisible);
        if (_isVisible) OnOpened();
        else OnClosed();
    }

    virtual protected void OnOpened() { }
    virtual protected void OnClosed() { }
}
