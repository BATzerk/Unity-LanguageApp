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
    public event NoParamAction ClipLoadFailEvent;
    public event NoParamAction CloseTermOptionsPopupEvent;
    public event NoParamAction SetContentsChangedEvent;
    public event NoParamAction PopupAppOptionsClosedEvent;
    public event AudioClipAction ClipLoadSuccessEvent;
    public event StudySetAction OpenPanelEditSetEvent;
    public event StudySetAction OpenPanelStudyFlashcardsEvent;
    public event TermAction PlayTermClipEvent;
    public event TermAction SpeakTTSNativeEvent;
    public event TermAction SpeakTTSForeignEvent;
    public event TermAction ShowPopup_MoveTermEvent;
    public event TermAction ShowPopup_TermOptionsEvent;

    public void OnAnySetContentsChanged() { if (SetContentsChangedEvent != null) { SetContentsChangedEvent(); } }

    public void CloseTermOptionsPopup() { if (CloseTermOptionsPopupEvent != null) { CloseTermOptionsPopupEvent(); } }
    public void OpenPanelEditSet(StudySet set) { if (OpenPanelEditSetEvent != null) { OpenPanelEditSetEvent(set); } }
    public void SpeakTTSNative(Term term) { if (SpeakTTSNativeEvent != null) { SpeakTTSNativeEvent(term); } }
    public void SpeakTTSForeign(Term term) { if (SpeakTTSForeignEvent != null) { SpeakTTSForeignEvent(term); } }
    public void OpenPanel_StudyFlashcards(StudySet set) { if (OpenPanelStudyFlashcardsEvent != null) { OpenPanelStudyFlashcardsEvent(set); } }
    public void ShowPopup_MoveTerm(Term term) { if (ShowPopup_MoveTermEvent != null) { ShowPopup_MoveTermEvent(term); } }
    public void ShowPopup_TermOptions(Term term) { if (ShowPopup_TermOptionsEvent != null) { ShowPopup_TermOptionsEvent(term); } }
    public void OnPopupAppOptionsClosed() { if (PopupAppOptionsClosedEvent != null) { PopupAppOptionsClosedEvent(); } }

    public void PlayTermClip(Term term) { if (PlayTermClipEvent != null) { PlayTermClipEvent(term); } }
    public void OnClipLoadFail() { if (ClipLoadFailEvent != null) { ClipLoadFailEvent(); } }
    public void OnClipLoadSuccess(AudioClip clip) { if (ClipLoadSuccessEvent != null) { ClipLoadSuccessEvent(clip); } }






}




