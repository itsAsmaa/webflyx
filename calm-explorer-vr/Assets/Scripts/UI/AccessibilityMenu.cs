using UnityEngine;
using UnityEngine.UI;

namespace CalmExplorer
{
    public class AccessibilityMenu : MonoBehaviour
    {
        public Slider volumeSlider;
        public Slider brightnessSlider;
        public Slider particleSlider;
        public Toggle gazeToggle;
        public Toggle guidedModeToggle;
        public Toggle teleportLocomotionToggle;

        void Start()
        {
            var settings = ComfortSettings.Instance;
            if (settings == null) return;

            if (volumeSlider != null)
            {
                volumeSlider.SetValueWithoutNotify(settings.masterVolume);
                volumeSlider.onValueChanged.AddListener(settings.SetVolume);
            }

            if (brightnessSlider != null)
            {
                brightnessSlider.SetValueWithoutNotify(settings.brightness);
                brightnessSlider.onValueChanged.AddListener(settings.SetBrightness);
            }

            if (particleSlider != null)
            {
                particleSlider.SetValueWithoutNotify(settings.particleIntensity);
                particleSlider.onValueChanged.AddListener(settings.SetParticleIntensity);
            }

            if (gazeToggle != null)
            {
                gazeToggle.SetIsOnWithoutNotify(settings.gazeInteractionEnabled);
                gazeToggle.onValueChanged.AddListener(settings.SetGazeEnabled);
            }

            if (guidedModeToggle != null)
            {
                guidedModeToggle.SetIsOnWithoutNotify(settings.guidedModeEnabled);
                guidedModeToggle.onValueChanged.AddListener(settings.SetGuidedMode);
            }

            if (teleportLocomotionToggle != null)
            {
                teleportLocomotionToggle.SetIsOnWithoutNotify(settings.locomotion == LocomotionMode.Teleport);
                teleportLocomotionToggle.onValueChanged.AddListener(
                    isOn => settings.SetLocomotion(isOn ? LocomotionMode.Teleport : LocomotionMode.RoomScaleOnly));
            }
        }
    }
}
