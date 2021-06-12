using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GoodEnough.TextToSpeech;

public class TermAudioClipPlayer : MonoBehaviour {
    // Components
    [SerializeField] private AudioSource audioSource;
    // Properties
    private string currClipGuid; // so we know if we need to load a new clip
    private int numTimesTTSSpokeCurrTermForeign; // so we can say it slowly every other play.
    // References
    private Term prevTermTTSForeign; // every time we speak foreign, we update this. So we can know if it has changed.


    // Getters
    //public bool IsClip() { return audioSource.clip != null; }
    private SettingsManager sm { get { return SettingsManager.Instance; } }



    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    void Start() {
        // Add event listeners
        GameManagers.Instance.EventManager.PlayTermClipEvent += PlayTermClip;
        GameManagers.Instance.EventManager.SpeakTTSNativeEvent += SpeakTTSNative;
        GameManagers.Instance.EventManager.SpeakTTSForeignEvent += SpeakTTSForeign;
    }
    private void OnDestroy() {
        // Remove event listeners
        GameManagers.Instance.EventManager.PlayTermClipEvent -= PlayTermClip;
        GameManagers.Instance.EventManager.SpeakTTSNativeEvent -= SpeakTTSNative;
        GameManagers.Instance.EventManager.SpeakTTSForeignEvent -= SpeakTTSForeign;
    }



    // ----------------------------------------------------------------
    //  Text to Speech
    // ----------------------------------------------------------------
    private void SpeakTTSNative(Term term) {
        if (TTS.IsSpeaking) { return; } // Already talking? Don't queue anything up.
        SpeechUtteranceParameters parameters = new SpeechUtteranceParameters();
        parameters.Voice = TTS.GetVoiceForLanguage(sm.NativeLanguageCode);
        parameters.PitchMultiplier = UnityEngine.Random.Range(0.3f, 2f);
        parameters.SpeechRate = sm.TTSSpeechRate / 2f; // note: divide by 2: 0.5 is actually normal-speed.
        TTS.Speak(term.native, parameters);
    }
    private void SpeakTTSForeign(Term term) {
        if (TTS.IsSpeaking) { return; } // Already talking? Don't queue anything up.
        // Update our tracking of what's been said recently.
        if (term == prevTermTTSForeign) {
            numTimesTTSSpokeCurrTermForeign ++; // increment this here.
        }
        else {
            numTimesTTSSpokeCurrTermForeign = 0;
            prevTermTTSForeign = term;
        }
        SpeechUtteranceParameters parameters = new SpeechUtteranceParameters();
        parameters.Voice = TTS.GetVoiceForLanguage(sm.CurrForeignCode);
        parameters.PitchMultiplier = UnityEngine.Random.Range(0.9f, 1.1f);
        //parameters.SpeechRate = sm.TTSSpeechRate / 2f; // note: divide by 2: 0.5 is actually normal-speed.
        parameters.SpeechRate = (numTimesTTSSpokeCurrTermForeign%2==0) ? 0.5f : 0.3f; // speak SLOWLY every OTHER play!
        TTS.Speak(term.foreign, parameters);
        Debug.Log("SpeechRate: " + parameters.SpeechRate);
    }



    // ----------------------------------------------------------------
    //  Audio Files
    // ----------------------------------------------------------------
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
                    if (SettingsManager.Instance.DoTrimAudioClips) {
                        clip = AudioEditor.GetQuietTrimmed(clip);
                    }
                    if (SettingsManager.Instance.DoNormalizeAudioClips) {
                        clip = AudioEditor.GetNormalized(clip);
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



}
