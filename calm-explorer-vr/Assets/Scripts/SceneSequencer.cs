using System.Collections.Generic;
using UnityEngine;

namespace CalmExplorer
{
    // Optional gentle nudge toward the next unexplored orb - a soft point
    // light, never sound or motion - shown only when a caregiver has opted
    // into "guided mode". Off by default: open-ended exploration is the norm.
    public class SceneSequencer : MonoBehaviour
    {
        public List<GameObject> sequenceOrder = new List<GameObject>();
        public float idleSecondsBeforeHint = 12f;
        public Color hintColor = new Color(1f, 1f, 0.6f, 0.6f);

        float _idleTimer;
        int _hintedIndex = -1;
        GameObject _activeHintMarker;

        void Update()
        {
            if (ComfortSettings.Instance == null || !ComfortSettings.Instance.guidedModeEnabled)
            {
                ClearHint();
                return;
            }

            _idleTimer += Time.deltaTime;
            if (_idleTimer >= idleSecondsBeforeHint)
                ShowNextHint();
        }

        public void NotifyPlayerActed()
        {
            _idleTimer = 0f;
            ClearHint();
        }

        void ShowNextHint()
        {
            if (sequenceOrder.Count == 0) return;
            _hintedIndex = (_hintedIndex + 1) % sequenceOrder.Count;
            var target = sequenceOrder[_hintedIndex];
            if (target == null) return;

            ClearHint();
            _activeHintMarker = new GameObject("HintMarker");
            _activeHintMarker.transform.SetParent(target.transform, false);
            _activeHintMarker.transform.localPosition = Vector3.up * 0.3f;
            var light = _activeHintMarker.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = hintColor;
            light.range = 1.5f;
            light.intensity = 1.2f;
            _idleTimer = 0f;
        }

        void ClearHint()
        {
            if (_activeHintMarker != null) Destroy(_activeHintMarker);
        }
    }
}
