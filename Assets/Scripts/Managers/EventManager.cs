using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager {
	// Actions and Event Variables
	public delegate void NoParamAction ();
    public delegate void AudioClipAction(AudioClip clip);
    public delegate void StudySetAction(StudySet set);
    public delegate void StringAction(string str);
    public delegate void TermAction(Term term);
    // Common
    public event NoParamAction ScreenSizeChangedEvent;
    public void OnScreenSizeChanged () { if (ScreenSizeChangedEvent!=null) { ScreenSizeChangedEvent (); } }



    // Gameplay
    public event NoParamAction SetContentsChangedEvent;
    public event NoParamAction ClipLoadFailEvent;
    public event AudioClipAction ClipLoadSuccessEvent;
    public event StudySetAction OpenPanelEditSetEvent;
    public event TermAction PlayTermClipEvent;
    public event TermAction OpenRecordPopupEvent;
    public event TermAction ShowMoveTermPopupEvent;

    public void PlayTermClip(Term term) { if (PlayTermClipEvent != null) { PlayTermClipEvent(term); } }
    public void OnAnySetContentsChanged() { if (SetContentsChangedEvent != null) { SetContentsChangedEvent(); } }
    public void OpenPanelEditSet(StudySet set) { if (OpenPanelEditSetEvent != null) { OpenPanelEditSetEvent(set); } }
    public void OpenRecordPopup(Term term) { if (OpenRecordPopupEvent != null) { OpenRecordPopupEvent(term); } }
    public void ShowMoveTermPopup(Term term) { if (ShowMoveTermPopupEvent != null) { ShowMoveTermPopupEvent(term); } }
    public void OnClipLoadFail() { if (ClipLoadFailEvent != null) { ClipLoadFailEvent(); } }
    public void OnClipLoadSuccess(AudioClip clip) { if (ClipLoadSuccessEvent != null) { ClipLoadSuccessEvent(clip); } }






}




