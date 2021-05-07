using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TermAudioClipPlayer : MonoBehaviour {
    // Components
    [SerializeField] private AudioSource audioSource;
    // Properties
    private string currClipGuid; // so we know if we need to load a new clip



    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    //void Start() {
    //    // Add event listeners
    //    GameManagers.Instance.EventManager.TellClipPlayerToLoadClipEvent += LoadClipRequest;
    //}
    //private void OnDestroy() {
    //    // Remove event listeners
    //    GameManagers.Instance.EventManager.TellClipPlayerToLoadClipEvent -= LoadClipRequest;
    //}



    public void Play() { audioSource.Play(); }
    public void Stop() { audioSource.Stop(); }
    public bool IsPlaying() { return audioSource.isPlaying; }
    public AudioClip GetClip() { return audioSource.clip; }
    public void SetClip(AudioClip clip) { audioSource.clip = clip; }

    public void PlayTermClip(Term term) {
        // Is it a new clip? Load a new clip!
        if (currClipGuid != term.audio0Guid) {
            currClipGuid = term.audio0Guid;
            string clipPath = SaveKeys.TermAudioClip0(term.audio0Guid);
            LoadClipFromPath(clipPath, true);
        }
        // Otherwise, play what's loaded; it's already the correct clip.
        else {
            Play();
        }
    }



    public void LoadClipFromPath(string clipPath, bool doPlayImmediately) {
        StartCoroutine(LoadAudioClipCoroutine(clipPath, doPlayImmediately));
    }

    IEnumerator LoadAudioClipCoroutine(string fullPath, bool doPlayImmediately) {
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
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    audioSource.clip = clip;
                    GameManagers.Instance.EventManager.OnClipLoadSuccess(clip);
                    Debug.Log("Loaded audioClip!");
                    if (doPlayImmediately) {
                        Play();
                    }
                }
                else {
                    GameManagers.Instance.EventManager.OnClipLoadFail();
                    Debug.Log("Couldn't load a valid AudioClip.");
                }
            }
            else {
                Debug.Log("The download process is not completely finished.");
            }
        }
    }
}
