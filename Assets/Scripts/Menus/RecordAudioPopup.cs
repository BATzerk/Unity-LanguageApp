using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;
using TMPro;

public class RecordAudioPopup : MonoBehaviour {
    // Components
    [SerializeField] private Button b_play;
    [SerializeField] private Button b_record;
    [SerializeField] private Button b_stopPlay;
    [SerializeField] private Button b_stopRecord;
    [SerializeField] private Button b_preDelete;
    [SerializeField] private GameObject scrim;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject go_preDelete;
    [SerializeField] private TextMeshProUGUI t_termName;
    [SerializeField] private TextMeshProUGUI t_audioDuration;
    // Properties
    private const int MaxAudioFileDuration = 5; // in SECONDS.
    private bool isPlayingClip; // so we know when the clip has finished playing naturally.
    private bool isRecordingClip; // so we know when the clip has finished recording naturally.
    // References
    [SerializeField] TermAudioClipPlayer clipPlayer;
    private Term currTerm;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    void Start() {
        // Start closed.
        Close();

        // Add event listeners
        GameManagers.Instance.EventManager.OpenRecordPopupEvent += OpenMe;
        GameManagers.Instance.EventManager.ClipLoadFailEvent += OnClipLoadFail;
        GameManagers.Instance.EventManager.ClipLoadSuccessEvent += OnClipLoadSuccess;
    }
    private void OnDestroy() {
        // Remove event listeners
        GameManagers.Instance.EventManager.OpenRecordPopupEvent -= OpenMe;
        GameManagers.Instance.EventManager.ClipLoadFailEvent -= OnClipLoadFail;
        GameManagers.Instance.EventManager.ClipLoadSuccessEvent -= OnClipLoadSuccess;
    }


    // ----------------------------------------------------------------
    //  Open / Close
    // ----------------------------------------------------------------
    private void OpenMe(Term currTerm) {
        this.gameObject.SetActive(true);
        SetCurrTerm(currTerm);
    }
    private void SetCurrTerm(Term currTerm) {
        this.currTerm = currTerm;
        t_termName.text = currTerm.foreign;

        StopPlay();
        StopRecord();
        OnClick_HidePreDelete();

        LoadClipForCurrTerm();
    }
    public void Close() {
        StopPlay();
        StopRecord(false); // stop recording, and don't save anything.

        this.gameObject.SetActive(false);
    }



    // ----------------------------------------------------------------
    //  Audio Save/Load
    // ----------------------------------------------------------------
    void LoadClipForCurrTerm() {
        string clipPath = SaveKeys.TermAudioClip0(currTerm.audio0Guid);
        bool isClipFile = File.Exists(clipPath);

        if (isClipFile) {
            clipPlayer.LoadClipFromPath(clipPath, false);
        }
        else {
            UpdateVisualsForNoClip();
        }
    }

    private void OnClipLoadFail() { UpdateVisualsForNoClip(); }
    private void OnClipLoadSuccess(AudioClip clip) { UpdateVisualsForClip(clip); }

    private void UpdateVisualsForClip(AudioClip clip) {
        b_play.interactable = true;
        b_preDelete.interactable = true;
        t_audioDuration.text = clip.length.ToString();
    }
    private void UpdateVisualsForNoClip() {
        b_play.interactable = false;
        b_preDelete.interactable = false;
        t_audioDuration.text = "";
        b_record.gameObject.SetActive(true);
    }



    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        // Done playing? Stop automatically.
        if (isPlayingClip && !clipPlayer.IsPlaying()) {// && audioSource.time >= audioSource.clip.length) {
            StopPlay();
        }

        // Done recording? Stop/save automatically.
        if (isRecordingClip && !Microphone.IsRecording(null)) {
            StopRecord();
        }
    }




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClick_HidePreDelete() { go_preDelete.SetActive(false); }
    public void OnClick_ShowPreDelete() { go_preDelete.SetActive(true); }
    public void OnClick_Delete() {
        dm.DeleteTermAudio0(currTerm);
        clipPlayer.SetClip(null);
        UpdateVisualsForNoClip();
        OnClick_HidePreDelete();
    }

    public void OnClick_Play() { StartPlay(); }
    public void OnClick_Record() { StartRecord(); }
    public void OnClick_StopPlay() { StopPlay(); }
    public void OnClick_StopRecord() { StopRecord(); }

    private void StartPlay() {
        isPlayingClip = true;
        isRecordingClip = false;
        clipPlayer.Play();
        b_play.gameObject.SetActive(false);
        b_record.gameObject.SetActive(false);
        b_stopPlay.gameObject.SetActive(true);
        b_stopRecord.gameObject.SetActive(false);
    }
    private void StopPlay() {
        isPlayingClip = false;
        isRecordingClip = false;
        clipPlayer.Stop();
        b_play.gameObject.SetActive(true);
        b_record.gameObject.SetActive(currTerm!=null && !currTerm.HasAudio0());
        b_stopPlay.gameObject.SetActive(false);
        b_stopRecord.gameObject.SetActive(false);
    }
    public void StartRecord() {
        isPlayingClip = false;
        isRecordingClip = true;
        clipPlayer.SetClip(Microphone.Start(null, false, MaxAudioFileDuration, 44100));//"Built-in Microphone"
        b_play.gameObject.SetActive(false);
        b_record.gameObject.SetActive(false);
        b_stopPlay.gameObject.SetActive(false);
        b_stopRecord.gameObject.SetActive(true);
    }
    private void StopRecord(bool doSaveFile=true) {
        // If we're actually recording, stop, and save!
        if (isRecordingClip) {
            Microphone.End(null);
            if (doSaveFile) {
                SaveAndAffiliateAudioFile();
            }
        }

        isPlayingClip = false;
        isRecordingClip = false;
        clipPlayer.Stop();
        b_play.gameObject.SetActive(true);
        //Debug.Log("is Clip: " + clipPlayer.IsClip() + "   "+ clipPlayer.GetClip());
        b_record.gameObject.SetActive(currTerm != null && !currTerm.HasAudio0());
        b_stopPlay.gameObject.SetActive(false);
        b_stopRecord.gameObject.SetActive(false);
    }

    private void SaveAndAffiliateAudioFile() {
        // First, delete the old clip.
        dm.DeleteTermAudio0(currTerm);

        AudioClip clip = clipPlayer.GetClip();
        Guid newGuid = Guid.NewGuid();

        // Assign it to the Term!
        currTerm.audio0Guid = newGuid.ToString();
        dm.SaveStudySetLibrary();

        // Save audio file.
        string filePath = SaveKeys.TermAudioClip0(currTerm.audio0Guid);
        SavWav.Save(filePath, clip);
        //EncodeMP3.ConvertAndWrite(clip, filePath, 128);
        Debug.Log("SAVED audio. Length: " + clip.length);

        // Set this as the current clip!
        UpdateVisualsForClip(clip);
    }



}
