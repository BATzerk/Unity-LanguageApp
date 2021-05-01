using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetVisibility(bool _isVisible)
    {
        this.gameObject.SetActive(_isVisible);
    }
}
