using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TermAudioClipPlayer : MonoBehaviour {
    // Components
    [SerializeField] private AudioSource audioSource;
    // Properties
    private string currClipGuid; // so we know if we need to load a new clip


    // Getters
    //public bool IsClip() { return audioSource.clip != null; }



    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    void Start() {
        // Add event listeners
        GameManagers.Instance.EventManager.PlayTermClipEvent += PlayTermClip;
    }
    private void OnDestroy() {
        // Remove event listeners
        GameManagers.Instance.EventManager.PlayTermClipEvent -= PlayTermClip;
    }



    public void Play() { audioSource.Play(); }
    public void Stop() { audioSource.Stop(); }
    public bool IsPlaying() { return audioSource.isPlaying; }
    public AudioClip GetClip() { return audioSource.clip; }
    public void SetClip(AudioClip clip) { audioSource.clip = clip; }

    public void PlayTermClip(Term term) {
        // NOTE: For now, just load the clip EVERY time for safety. We want it definitely trimmed/normalized.
        //// Is it a new clip? Load a new clip!
        //if (currClipGuid != term.audio0Guid) {
            currClipGuid = term.audio0Guid;
            string clipPath = SaveKeys.TermAudioClip0(term.audio0Guid);
            LoadClipFromPath(clipPath, true);
        //}
        //// Otherwise, play what's loaded; it's already the correct clip.
        //else {
        //    Play();
        //}
    }



    public void LoadClipFromPath(string clipPath, bool doPlayImmediately) {
        StartCoroutine(LoadAudioClipCoroutine(clipPath, doPlayImmediately));
    }

    IEnumerator LoadAudioClipCoroutine(string fullPath, bool doPlayImmediately) {
        fullPath = "file:///" + fullPath; // HACK for Mac? We can't find the audio file without this prefix, but *only* for loading, not saving.
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.WAV)) {
            ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;

            audioSource.clip = null; // default this to null, in case we fail.

            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError) {
                AppDebugLog.LogError(uwr.error);
                yield break;
            }

            DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
            yield return dlHandler;

            if (dlHandler.isDone) {
                audioSource.clip = dlHandler.audioClip;

                if (audioSource.clip != null) {
                    WWW download = new WWW(fullPath); // HACKED, using WWW! I think DownloadHandlerAudioClip doesn't support getting the file samples. Not sure tho.
                    yield return download;
                    bool threeD = false;
                    bool stream = false;
                    AudioClip clip = download.GetAudioClip(threeD, stream, AudioType.WAV);
                    //float[] samples = new float[clip.samples * clip.channels];
                    //clip.GetData(samples, 0);
                    //for (int s = 0; s < 100; s++) Debug.Log(samples[s]);


                    //AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    if (GameManagers.Instance.SettingsManager.DoTrimAudioClips) {
                        clip = GetTrimmed(clip);
                    }
                    if (GameManagers.Instance.SettingsManager.DoNormalizeAudioClips) {
                        clip = GetNormalized(clip);
                    }
                    audioSource.clip = clip;
                    GameManagers.Instance.EventManager.OnClipLoadSuccess(clip);
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

    const float SampleQuietThreshold = 0.17f;//TODO: Make a slider for this in the App Options.
    //const float NormalizeMin = 0.1f;
    const float NormalizeMax = 0.9f;
    const int InitialSamplesToIgnore = 20000; // in case there's a bump in the beginning of the recording, just don't register it.
    const int SamplesToIncludeBeforeFirstLoudIndex = 15000; // include just a moment before the threshold point, to make it less potentially abrupt.

    AudioClip GetTrimmed(AudioClip clip) {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // TRIM.
        int firstLoudIndex = 0;
        for (int i=Mathf.Min(samples.Length,InitialSamplesToIgnore); i<samples.Length; ++i) {
            if (samples[i] > SampleQuietThreshold) {
                firstLoudIndex = i;
                break;
            }
        }
        firstLoudIndex = Mathf.Max(0, firstLoudIndex - SamplesToIncludeBeforeFirstLoudIndex);
        float[] newSamples = new float[samples.Length - firstLoudIndex];
        for (int i=0; i<newSamples.Length; i++) {
            newSamples[i] = samples[i+firstLoudIndex];
        }

        Debug.Log("TRIM. samples.Length: " + samples.Length + ",   firstLoudIndex: " + firstLoudIndex);

        clip = AudioClip.Create(clip.name, newSamples.Length, clip.channels, clip.frequency, false);
        clip.SetData(newSamples, 0);
        return clip;
    }
    AudioClip GetNormalized(AudioClip clip) {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float highestPeak = 0;
        for (int i=0; i<samples.Length; ++i) {
            highestPeak = Mathf.Max(highestPeak, Mathf.Abs(samples[i]));
        }
        float normalizeFactor = NormalizeMax / highestPeak;
        normalizeFactor = Mathf.Clamp(normalizeFactor, 1f, 3f); // Don't normalize softer, or get TOO loud.
        for (int i=0; i<samples.Length; ++i) {
            samples[i] *= normalizeFactor;
        }

        Debug.Log("NORMALIZE. normalizeFactor: " + normalizeFactor + ",  highestPeak: " + highestPeak);

        clip = AudioClip.Create(clip.name, samples.Length, clip.channels, clip.frequency, false);
        clip.SetData(samples, 0);
        return clip;
    }


}
