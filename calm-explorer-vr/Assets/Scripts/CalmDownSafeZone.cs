using System.Collections.Generic;
using UnityEngine;

namespace CalmExplorer
{
    // An always-available retreat: stepping in dims the lights and ducks
    // ambient sound, giving the child a way to de-escalate on their own
    // terms, never gated behind anything else in the scene.
    public class CalmDownSafeZone : MonoBehaviour
    {
        public Light[] dimOnEnter;
        public float dimmedIntensityScale = 0.4f;
        public AudioSource ambientToDuck;
        public float duckedVolumeScale = 0.3f;

        readonly Dictionary<Light, float> _originalIntensities = new Dictionary<Light, float>();
        float _originalAmbientVolume;
        int _occupants;

        void Awake()
        {
            foreach (var light in dimOnEnter)
                if (light != null) _originalIntensities[light] = light.intensity;

            if (ambientToDuck != null)
                _originalAmbientVolume = ambientToDuck.volume;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _occupants++;
            if (_occupants == 1) Enter();
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _occupants = Mathf.Max(0, _occupants - 1);
            if (_occupants == 0) Exit();
        }

        void Enter()
        {
            foreach (var light in dimOnEnter)
                if (light != null) light.intensity = _originalIntensities[light] * dimmedIntensityScale;

            if (ambientToDuck != null)
                ambientToDuck.volume = _originalAmbientVolume * duckedVolumeScale;
        }

        void Exit()
        {
            foreach (var light in dimOnEnter)
                if (light != null) light.intensity = _originalIntensities[light];

            if (ambientToDuck != null)
                ambientToDuck.volume = _originalAmbientVolume;
        }
    }
}
