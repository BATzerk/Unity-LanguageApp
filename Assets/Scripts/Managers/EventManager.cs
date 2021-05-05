using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager {
	// Actions and Event Variables
	public delegate void NoParamAction ();
    public delegate void TermAction(Term term);
    // Common
    public event NoParamAction ScreenSizeChangedEvent;
    public void OnScreenSizeChanged () { if (ScreenSizeChangedEvent!=null) { ScreenSizeChangedEvent (); } }



    // Gameplay
    public event NoParamAction SetContentsChangedEvent;
    public event TermAction ShowMoveTermPopupEvent;

    public void OnAnySetContentsChanged() { if (SetContentsChangedEvent != null) { SetContentsChangedEvent(); } }
    public void ShowMoveTermPopup(Term term) { if (ShowMoveTermPopupEvent != null) { ShowMoveTermPopupEvent(term); } }






}




