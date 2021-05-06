using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;
using TMPro;

public class RecordAudioPopup : MonoBehaviour
{
    // Components
    [SerializeField] private AudioSource audioSource;
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
    private Term currTerm;

    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    void Start() {
        // Start closed.
        Close();

        // Add event listeners
        GameManagers.Instance.EventManager.OpenRecordPopupEvent += OpenMe;
    }
    private void OnDestroy() {
        // Remove event listeners
        GameManagers.Instance.EventManager.OpenRecordPopupEvent -= OpenMe;
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
        t_termName.text = currTerm.danish;

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
    //string testClipPath { get { return Path.Combine(Application.persistentDataPath, "Audio/TestAudio.wav"); } }
    void LoadClipForCurrTerm() {
        string clipPath = SaveKeys.TermAudioClip0(currTerm.audio0Guid);
        StartCoroutine(LoadAudioClip(clipPath));
    }
    IEnumerator LoadAudioClip(string fullPath) {
        fullPath = "file:///" + fullPath; // HACK for Mac? We can't find the audio file without this prefix, but *only* for loading, not saving.
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.WAV)) {
            ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;

            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError) {
                Debug.LogError(uwr.error);
                yield break;
            }

            DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;

            if (dlHandler.isDone) {
                audioSource.clip = dlHandler.audioClip;

                if (audioSource.clip != null) {
                    OnClipSuccessfullyLoaded(DownloadHandlerAudioClip.GetContent(uwr));
                    Debug.Log("Loaded audioClip!");
                }
                else {
                    Debug.Log("Couldn't load a valid AudioClip.");
                }
            }
            else {
                Debug.Log("The download process is not completely finished.");
            }
        }
    }
    private void OnClipSuccessfullyLoaded(AudioClip clip) {
        audioSource.clip = clip;
        b_preDelete.interactable = clip != null;
        t_audioDuration.text = clip == null ? "" : clip.length.ToString();
    }



    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        // Done playing? Stop automatically.
        if (isPlayingClip && !audioSource.isPlaying) {// && audioSource.time >= audioSource.clip.length) {
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
        OnClick_HidePreDelete();
    }

    public void OnClick_Play() { StartPlay(); }
    public void OnClick_Record() { StartRecord(); }
    public void OnClick_StopPlay() { StopPlay(); }
    public void OnClick_StopRecord() { StopRecord(); }

    private void StartPlay() {
        isPlayingClip = true;
        isRecordingClip = false;
        audioSource.Play();
        b_play.gameObject.SetActive(false);
        b_record.gameObject.SetActive(false);
        b_stopPlay.gameObject.SetActive(true);
        b_stopRecord.gameObject.SetActive(false);
    }
    private void StopPlay() {
        isPlayingClip = false;
        isRecordingClip = false;
        audioSource.Stop();
        b_play.gameObject.SetActive(true);
        b_record.gameObject.SetActive(true);
        b_stopPlay.gameObject.SetActive(false);
        b_stopRecord.gameObject.SetActive(false);
    }
    public void StartRecord() {
        isPlayingClip = false;
        isRecordingClip = true;
        audioSource.clip = Microphone.Start(null, false, MaxAudioFileDuration, 44100);//"Built-in Microphone"
        b_play.gameObject.SetActive(false);
        b_record.gameObject.SetActive(false);
        b_stopPlay.gameObject.SetActive(false);
        b_stopRecord.gameObject.SetActive(true);
    }
    private void StopRecord(bool doSaveFile=true) {
        // If we're actually recording, stop, and save!
        if (isRecordingClip) {
            Microphone.End(null);
            // Save it!
            if (doSaveFile) {
                string filePath = SaveKeys.TermAudioClip0(currTerm.audio0Guid);
                SavWav.Save(filePath, audioSource.clip);
                Debug.Log("SAVED audio. Length: " + audioSource.clip.length);
            }
        }

        isPlayingClip = false;
        isRecordingClip = false;
        audioSource.Stop();
        b_play.gameObject.SetActive(true);
        b_record.gameObject.SetActive(true);
        b_stopPlay.gameObject.SetActive(false);
        b_stopRecord.gameObject.SetActive(false);
    }



}
