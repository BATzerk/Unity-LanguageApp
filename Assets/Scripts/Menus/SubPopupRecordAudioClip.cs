using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;
using TMPro;

public class SubPopupRecordAudioClip : MonoBehaviour {
    // Components
    [SerializeField] private Button b_play;
    [SerializeField] private Button b_stop;
    [SerializeField] private Button b_record;
    [SerializeField] private Button b_preDelete;
    [SerializeField] private GameObject go_deleteConfirmation;
    [SerializeField] private Image i_recordingScrim;
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
        //// Start closed.
        //Close();

        // Add event listeners
        GameManagers.Instance.EventManager.ClipLoadFailEvent += OnClipLoadFail;
        GameManagers.Instance.EventManager.ClipLoadSuccessEvent += OnClipLoadSuccess;
    }
    private void OnDestroy() {
        // Remove event listeners
        GameManagers.Instance.EventManager.ClipLoadFailEvent -= OnClipLoadFail;
        GameManagers.Instance.EventManager.ClipLoadSuccessEvent -= OnClipLoadSuccess;
    }


    // ----------------------------------------------------------------
    //  Open / Close
    // ----------------------------------------------------------------
    public void OnOwnerClose() {
        StopPlay();
        StopRecord(false); // stop recording, and don't save anything.
    }
    public void OnOwnerOpen(Term currTerm) {
        this.currTerm = currTerm;
        this.gameObject.SetActive(true);

        StopPlay();
        StopRecord();
        OnClick_HidePreDelete();

        LoadClipForCurrTerm();
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
        b_preDelete.gameObject.SetActive(true);
        t_audioDuration.text = clip.length.ToString();
    }
    private void UpdateVisualsForNoClip() {
        b_play.interactable = false;
        b_preDelete.gameObject.SetActive(false);
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
    public void OnClick_HidePreDelete() { go_deleteConfirmation.SetActive(false); }
    public void OnClick_ShowPreDelete() { go_deleteConfirmation.SetActive(true); }
    public void OnClick_ConfirmDelete() {
        dm.DeleteTermAudio0(currTerm);
        clipPlayer.SetClip(null);
        UpdateVisualsForNoClip();
        GameManagers.Instance.EventManager.OnAnySetContentsChanged();
        OnClick_HidePreDelete();
    }

    public void OnClick_Play() { StartPlay(); }
    public void OnClick_Record() {
        isPlayingClip = false;
        b_play.gameObject.SetActive(false);
        b_record.gameObject.SetActive(false);
        b_stop.gameObject.SetActive(true);
        i_recordingScrim.enabled = true;
        isRecordingClip = true;
        clipPlayer.SetClip(Microphone.Start(null, false, MaxAudioFileDuration, 44100));//"Built-in Microphone"
    }
    public void OnClick_Stop() {
        if (isRecordingClip) { StopRecord(); }
        else { StopPlay(); }
    }

    private void StartPlay() {
        isPlayingClip = true;
        isRecordingClip = false;
        //clipPlayer.Play();
        clipPlayer.PlayTermClip(currTerm);
        b_play.gameObject.SetActive(false);
        b_record.gameObject.SetActive(false);
        b_stop.gameObject.SetActive(true);
    }
    private void StopPlay() {
        isPlayingClip = false;
        isRecordingClip = false;
        clipPlayer.Stop();
        bool isAudio0 = currTerm!=null && currTerm.HasAudio0();
        b_play.gameObject.SetActive(isAudio0);
        b_record.gameObject.SetActive(!isAudio0);
        b_stop.gameObject.SetActive(false);
    }
    //private IEnumerator StartRecord_Coroutine() {
    //    yield return null; // Skip a frame before ACTually starting, so the buttons look right while we load to start recording!
    //}
    private void StopRecord(bool doSaveFile = true) {
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
        bool isAudio0 = currTerm!=null && currTerm.HasAudio0();
        b_play.gameObject.SetActive(isAudio0);
        b_record.gameObject.SetActive(!isAudio0);
        b_stop.gameObject.SetActive(false);
        i_recordingScrim.enabled = false;
    }

    private void SaveAndAffiliateAudioFile() {
        // First, delete the old clip.
        dm.DeleteTermAudio0(currTerm);

        AudioClip clip = clipPlayer.GetClip();
        clip = AudioEditor.GetTrailingSilenceTrimmed(clip);
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
        GameManagers.Instance.EventManager.OnAnySetContentsChanged();
    }





}
