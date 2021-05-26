using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioEditor {
    // Properties
    const float SampleQuietThreshold = 0.17f;//TODO: Make a slider for this in the App Options.
    //const float NormalizeMin = 0.1f;
    const float NormalizeMax = 0.9f;
    const int InitialSamplesToIgnore = 20000; // in case there's a bump in the beginning of the recording, just don't register it.
    const int SamplesToIncludeBeforeFirstLoudIndex = 15000; // include just a moment before the threshold point, to make it less potentially abrupt.


    public static AudioClip GetQuietTrimmed(AudioClip clip) {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // TRIM.
        int firstLoudIndex = 0;
        for (int i = Mathf.Min(samples.Length, InitialSamplesToIgnore); i < samples.Length; ++i) {
            if (samples[i] > SampleQuietThreshold) {
                firstLoudIndex = i;
                break;
            }
        }
        firstLoudIndex = Mathf.Max(0, firstLoudIndex - SamplesToIncludeBeforeFirstLoudIndex);
        float[] newSamples = new float[samples.Length - firstLoudIndex];
        for (int i = 0; i < newSamples.Length; i++) {
            newSamples[i] = samples[i + firstLoudIndex];
        }

        Debug.Log("TRIM. samples.Length: " + samples.Length + ",   firstLoudIndex: " + firstLoudIndex);

        clip = AudioClip.Create(clip.name, newSamples.Length, clip.channels, clip.frequency, false);
        clip.SetData(newSamples, 0);
        return clip;
    }
    public static AudioClip GetNormalized(AudioClip clip) {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float highestPeak = 0;
        for (int i = 0; i < samples.Length; ++i) {
            highestPeak = Mathf.Max(highestPeak, Mathf.Abs(samples[i]));
        }
        float normalizeFactor = NormalizeMax / highestPeak;
        normalizeFactor = Mathf.Clamp(normalizeFactor, 1f, 3f); // Don't normalize softer, or get TOO loud.
        for (int i = 0; i < samples.Length; ++i) {
            samples[i] *= normalizeFactor;
        }

        Debug.Log("NORMALIZE. normalizeFactor: " + normalizeFactor + ",  highestPeak: " + highestPeak);

        clip = AudioClip.Create(clip.name, samples.Length, clip.channels, clip.frequency, false);
        clip.SetData(samples, 0);
        return clip;
    }


    public static AudioClip GetTrailingSilenceTrimmed(AudioClip clip) {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // Determine how many new samples there are.
        int numNewSamples = samples.Length; // default to all of 'em.
        for (int i=samples.Length-1; i>=0; --i) {
            if (samples[i] != 0) {
                numNewSamples = i;
                break;
            }
        }
        Debug.Log("TRAILING SILENCE TRIMMED. original samples: " + clip.samples + ",  new samples: " + numNewSamples);
        // Make/return the new clip!
        clip = AudioClip.Create(clip.name, numNewSamples, clip.channels, clip.frequency, false);
        clip.SetData(samples, 0);
        return clip;
    }

}
