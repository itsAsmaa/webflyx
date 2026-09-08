using System;
using UnityEngine;

namespace CalmExplorer
{
    public enum LocomotionMode { Teleport, RoomScaleOnly }

    public class ComfortSettings : MonoBehaviour
    {
        public static ComfortSettings Instance { get; private set; }

        [Range(0f, 1f)] public float masterVolume = 0.5f;
        [Range(0f, 1f)] public float brightness = 0.7f;
        [Range(0f, 1f)] public float particleIntensity = 0.6f;
        public LocomotionMode locomotion = LocomotionMode.Teleport;
        public bool vignetteDuringMovement = true;
        public bool gazeInteractionEnabled = true;
        public bool guidedModeEnabled = false;

        public event Action Changed;

        const string KeyVolume = "calm_volume";
        const string KeyBrightness = "calm_brightness";
        const string KeyParticles = "calm_particles";
        const string KeyLocomotion = "calm_locomotion";
        const string KeyVignette = "calm_vignette";
        const string KeyGaze = "calm_gaze";
        const string KeyGuided = "calm_guided";

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        public void Load()
        {
            masterVolume = PlayerPrefs.GetFloat(KeyVolume, masterVolume);
            brightness = PlayerPrefs.GetFloat(KeyBrightness, brightness);
            particleIntensity = PlayerPrefs.GetFloat(KeyParticles, particleIntensity);
            locomotion = (LocomotionMode)PlayerPrefs.GetInt(KeyLocomotion, (int)locomotion);
            vignetteDuringMovement = PlayerPrefs.GetInt(KeyVignette, vignetteDuringMovement ? 1 : 0) == 1;
            gazeInteractionEnabled = PlayerPrefs.GetInt(KeyGaze, gazeInteractionEnabled ? 1 : 0) == 1;
            guidedModeEnabled = PlayerPrefs.GetInt(KeyGuided, guidedModeEnabled ? 1 : 0) == 1;
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(KeyVolume, masterVolume);
            PlayerPrefs.SetFloat(KeyBrightness, brightness);
            PlayerPrefs.SetFloat(KeyParticles, particleIntensity);
            PlayerPrefs.SetInt(KeyLocomotion, (int)locomotion);
            PlayerPrefs.SetInt(KeyVignette, vignetteDuringMovement ? 1 : 0);
            PlayerPrefs.SetInt(KeyGaze, gazeInteractionEnabled ? 1 : 0);
            PlayerPrefs.SetInt(KeyGuided, guidedModeEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetVolume(float v) { masterVolume = v; Save(); Changed?.Invoke(); }
        public void SetBrightness(float v) { brightness = v; Save(); Changed?.Invoke(); }
        public void SetParticleIntensity(float v) { particleIntensity = v; Save(); Changed?.Invoke(); }
        public void SetLocomotion(LocomotionMode m) { locomotion = m; Save(); Changed?.Invoke(); }
        public void SetVignette(bool on) { vignetteDuringMovement = on; Save(); Changed?.Invoke(); }
        public void SetGazeEnabled(bool on) { gazeInteractionEnabled = on; Save(); Changed?.Invoke(); }
        public void SetGuidedMode(bool on) { guidedModeEnabled = on; Save(); Changed?.Invoke(); }
    }
}
