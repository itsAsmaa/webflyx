using UnityEngine;

namespace CalmExplorer
{
    public static class ToneGenerator
    {
        public const int SampleRate = 44100;

        // C major pentatonic (C D E G A) - no two notes clash, so any two orbs
        // played together stay pleasant instead of turning dissonant.
        public static readonly float[] PentatonicScale = { 261.63f, 293.66f, 329.63f, 392.00f, 440.00f };

        public static AudioClip CreateSineTone(float frequency, float duration, float fadeSeconds = 0.05f)
        {
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(SampleRate * duration));
            var samples = new float[sampleCount];
            int fadeSamples = Mathf.Min(sampleCount / 2, Mathf.RoundToInt(SampleRate * fadeSeconds));

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)SampleRate;
                float value = Mathf.Sin(2f * Mathf.PI * frequency * t);

                float envelope = 1f;
                if (fadeSamples > 0)
                {
                    if (i < fadeSamples) envelope = i / (float)fadeSamples;
                    else if (i > sampleCount - fadeSamples) envelope = (sampleCount - i) / (float)fadeSamples;
                }

                samples[i] = value * envelope * 0.35f;
            }

            var clip = AudioClip.Create($"Tone_{frequency:0}Hz", sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
