using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace CalmExplorer
{
    // An interactive orb: hover plays a soft tone, select toggles a calm
    // color change and triggers a reward. Always reversible, never a "wrong"
    // input, so there's no fail state to react to.
    [RequireComponent(typeof(AudioSource))]
    public class SensoryObject : XRBaseInteractable, IGazeSelectable
    {
        public float toneFrequency = 261.63f;
        public Color idleColor = new Color(0.6f, 0.8f, 0.9f);
        public Color activeColor = new Color(1f, 0.85f, 0.6f);
        public float pulseSpeed = 2f;

        AudioSource _audioSource;
        Renderer _renderer;
        MaterialPropertyBlock _propBlock;
        bool _activated;
        bool _hovering;
        Vector3 _baseScale;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
            _audioSource.clip = ToneGenerator.CreateSineTone(toneFrequency, 2f, 0.3f);

            _renderer = GetComponentInChildren<Renderer>();
            _propBlock = new MaterialPropertyBlock();
            _baseScale = transform.localScale;
            SetColor(idleColor);
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            BeginHover();
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            EndHover();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            Activate();
        }

        public void OnGazeEnter() => BeginHover();
        public void OnGazeExit() => EndHover();
        public void OnGazeSelect() => Activate();

        void BeginHover()
        {
            _hovering = true;
            float volume = ComfortSettings.Instance != null ? ComfortSettings.Instance.masterVolume : 0.5f;
            _audioSource.volume = volume * 0.5f;
            if (!_audioSource.isPlaying) _audioSource.Play();
        }

        void EndHover()
        {
            _hovering = false;
            _audioSource.Stop();
        }

        void Activate()
        {
            _activated = !_activated;
            SetColor(_activated ? activeColor : idleColor);
            if (_activated) RewardSystem.Instance?.Celebrate(transform.position);
        }

        void Update()
        {
            float pulse = _hovering ? 1f + 0.08f * Mathf.Sin(Time.time * pulseSpeed) : 1f;
            transform.localScale = _baseScale * pulse;
        }

        void SetColor(Color color)
        {
            if (_renderer == null) return;
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", color);
            _propBlock.SetColor("_Color", color);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}
