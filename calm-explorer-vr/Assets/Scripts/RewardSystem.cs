using UnityEngine;

namespace CalmExplorer
{
    public class RewardSystem : MonoBehaviour
    {
        public static RewardSystem Instance { get; private set; }

        public ParticleSystem celebrationPrefab;

        AudioSource _audioSource;

        void Awake()
        {
            Instance = this;
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
        }

        public void Celebrate(Vector3 worldPosition)
        {
            float volume = ComfortSettings.Instance != null ? ComfortSettings.Instance.masterVolume : 0.5f;
            float particleScale = ComfortSettings.Instance != null ? ComfortSettings.Instance.particleIntensity : 0.6f;

            if (celebrationPrefab != null)
            {
                var fx = Instantiate(celebrationPrefab, worldPosition, Quaternion.identity);
                fx.transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 1.2f, particleScale);
                fx.Play();
                Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
            }

            var note = ToneGenerator.PentatonicScale[Random.Range(0, ToneGenerator.PentatonicScale.Length)];
            var clip = ToneGenerator.CreateSineTone(note * 2f, 0.6f, 0.1f);
            _audioSource.transform.position = worldPosition;
            _audioSource.PlayOneShot(clip, volume);
        }
    }
}
