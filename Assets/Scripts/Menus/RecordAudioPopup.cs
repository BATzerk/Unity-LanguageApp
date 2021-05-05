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
    [SerializeField] private GameObject buttonPlay;
    [SerializeField] private GameObject buttonRecord;
    [SerializeField] private GameObject buttonStop;
    //string url = "file://C:/Users/Pajkec/Desktop/muske WAW/maroon5.wav";
    void Start() {

        string path = Path.Combine(Application.persistentDataPath, "Audio");
        path = Path.Combine(path, "TestAudio.wav");
        WWW audioLoader = new WWW(path);
        while (!audioLoader.isDone) {
            Debug.Log("uploading");
        }

        Debug.Log("1");

        audioSource.clip = audioLoader.GetAudioClip(false, false, AudioType.WAV);
        //audio.Play();


        //Debug.Log("start");
        //StartCoroutine(StartAudio());
    }
    void Update() {
        if (!audioSource.isPlaying && audioSource.clip.isReadyToPlay) {
            Debug.Log("playing");
            audioSource.Play();
        }
    }

    /*
    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    //private void Start() {
    //    StopRecord();

    //    string tempPath = Path.Combine(Application.persistentDataPath, "Audio");
    //    tempPath = Path.Combine(tempPath, "TestAudio.wav");
    //    Debug.Log("loading path: " + tempPath);
    //    StartCoroutine(LoadAudio(tempPath));
    //}

    //IEnumerator LoadAudio(string path) {
    //    // Load Audio
    //    WWW audioLoader = new WWW(path);
    //    yield return audioLoader;

    //    // Convert it to AudioClip
    //    Debug.Log("audioLoader.bytesDownloaded: " + audioLoader.bytesDownloaded);
    //    Debug.Log("audioLoader.url: " + audioLoader.url);
    //    AudioClip loadedClip = audioLoader.GetAudioClip(false, false, AudioType.WAV);
    //    audioSource.clip = loadedClip;
    //    Debug.Log("Loaded audio: " + audioSource.clip.length);
    //}
    async void Start() {
        // build your absolute path
        string path = Path.Combine(Application.persistentDataPath, "Audio");
        path = Path.Combine(path, "TestAudio.wav");

        // wait for the load and set your property
        audioSource.clip = await LoadClip(path);

        //... do something with it
    }

    async Task<AudioClip> LoadClip(string path) {
        AudioClip clip = null;
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV)) {
            uwr.SendWebRequest();

            // wrap tasks in try/catch, otherwise it'll fail silently
            try {
                while (!uwr.isDone) await Task.Delay(5);

                if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                else {
                    clip = DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
            catch (Exception err) {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }

        Debug.Log("Returning audio: " + clip);
        return clip;
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
            string tempPath = Path.Combine(Application.persistentDataPath, "Audio");
            tempPath = Path.Combine(tempPath, "TestAudio.wav");
            SavWav.Save(tempPath, audioSource.clip);
            Debug.Log("SAVED audio. Length: " + audioSource.clip.length);
        }
    }*/



}
