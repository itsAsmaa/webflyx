using UnityEngine;

namespace CalmExplorer
{
    // Dwell-based gaze selection - an alternative to controller grab/trigger
    // for children who have difficulty holding or aiming hand controllers.
    [RequireComponent(typeof(Camera))]
    public class GazeInteractor : MonoBehaviour
    {
        public float maxDistance = 10f;
        public float dwellSeconds = 1.5f;
        public LayerMask interactableLayers = ~0;
        public Transform reticle;

        Camera _camera;
        IGazeSelectable _current;
        float _dwellTimer;
        bool _selectedThisDwell;

        void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        void Update()
        {
            if (ComfortSettings.Instance != null && !ComfortSettings.Instance.gazeInteractionEnabled)
            {
                ClearCurrent();
                if (reticle) reticle.gameObject.SetActive(false);
                return;
            }

            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            if (Physics.Raycast(ray, out var hit, maxDistance, interactableLayers))
            {
                var selectable = hit.collider.GetComponentInParent<IGazeSelectable>();
                if (selectable != _current)
                {
                    ClearCurrent();
                    _current = selectable;
                    _dwellTimer = 0f;
                    _selectedThisDwell = false;
                    _current?.OnGazeEnter();
                }

                if (reticle)
                {
                    reticle.gameObject.SetActive(true);
                    reticle.position = hit.point;
                    reticle.localScale = Vector3.one * (_current != null ? Mathf.Clamp01(_dwellTimer / dwellSeconds) : 0.01f);
                }

                if (_current != null && !_selectedThisDwell)
                {
                    _dwellTimer += Time.deltaTime;
                    if (_dwellTimer >= dwellSeconds)
                    {
                        _selectedThisDwell = true;
                        _current.OnGazeSelect();
                    }
                }
            }
            else
            {
                ClearCurrent();
                if (reticle) reticle.gameObject.SetActive(false);
            }
        }

        void ClearCurrent()
        {
            _current?.OnGazeExit();
            _current = null;
            _dwellTimer = 0f;
            _selectedThisDwell = false;
        }
    }
}
