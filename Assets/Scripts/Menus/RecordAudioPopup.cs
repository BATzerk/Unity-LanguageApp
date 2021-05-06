using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;

public class RecordAudioPopup : MonoBehaviour
{
    // Components
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject scrim;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject buttonPlay;
    [SerializeField] private GameObject buttonRecord;
    [SerializeField] private GameObject buttonStop;
    // References
    private Term currTerm;


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
        this.currTerm = currTerm;

        this.gameObject.SetActive(true);
        StopRecord();
        LoadClipForCurrTerm();
    }
    public void Close() {
        this.gameObject.SetActive(false);
    }



    string testClipPath { get { return Path.Combine(Application.persistentDataPath, "Audio/TestAudio.wav"); } }
    void LoadClipForCurrTerm() {
        StartCoroutine(GetAudioClip(testClipPath));
    }
    IEnumerator GetAudioClip(string fullPath) {
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
                    audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
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




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClick_Record() { StartRecord(); }
    public void OnClick_Stop() { StopRecord(); }
    public void OnClick_Play() {
        audioSource.Play();
    }

    public void StartRecord() {
        audioSource.clip = Microphone.Start(null, false, 10, 44100);//"Built-in Microphone"
                                                                    // Update visuals.
        buttonRecord.SetActive(false);
        buttonStop.SetActive(true);
        buttonPlay.SetActive(false);
    }
    private void StopRecord() {
        // Update visuals.
        buttonRecord.SetActive(true);
        buttonStop.SetActive(false);
        buttonPlay.SetActive(true);
        // If we're actually recording, stop, and save!
        if (Microphone.IsRecording(null)) {
            Microphone.End(null);
            // Save it!
            SavWav.Save(testClipPath, audioSource.clip);
            Debug.Log("SAVED audio. Length: " + audioSource.clip.length);
        }
    }



}
