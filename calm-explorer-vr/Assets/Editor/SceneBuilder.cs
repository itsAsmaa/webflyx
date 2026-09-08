using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CalmExplorer.EditorTools
{
    public static class SceneBuilder
    {
        [MenuItem("Calm Explorer/Build Demo Scene")]
        public static void BuildDemoScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.85f, 0.9f, 0.95f);

            BuildLighting();
            BuildFloor();
            GameObject player = FindOrWarnForXROrigin();
            BuildComfortSettings();
            BuildRewardSystem();
            BuildSafeZone();
            List<GameObject> sensoryObjects = BuildSensoryObjects();
            BuildGazeInteractor(player);
            BuildAccessibilityMenu();

            var sequencerGO = new GameObject("SceneSequencer");
            var sequencer = sequencerGO.AddComponent<SceneSequencer>();
            sequencer.sequenceOrder = sensoryObjects;

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/CalmExplorer.unity");
            Debug.Log("Calm Explorer demo scene built. If no XR Origin was found, drag in the 'XR Origin (VR)' " +
                      "prefab from the XR Interaction Toolkit Starter Assets sample and tag its root 'Player', " +
                      "then re-run this tool.");
        }

        static void BuildLighting()
        {
            var lightGO = new GameObject("Soft Directional Light");
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.9f);
            light.intensity = 0.8f;
            lightGO.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }

        static void BuildFloor()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = Vector3.one * 5f;

            var renderer = floor.GetComponent<Renderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            renderer.sharedMaterial = new Material(shader) { color = new Color(0.75f, 0.8f, 0.85f) };
        }

        static GameObject FindOrWarnForXROrigin()
        {
            GameObject existing = GameObject.Find("XR Origin (XR Rig)") ?? GameObject.Find("XR Origin");
            if (existing != null)
            {
                existing.tag = "Player";
                return existing;
            }

            Debug.LogWarning("No XR Origin found in scene. Import the 'XR Origin (VR)' prefab from the XR " +
                              "Interaction Toolkit Starter Assets sample and re-run this tool, or drag it in " +
                              "manually and tag its root 'Player'. A placeholder camera rig was created instead.");

            var stub = new GameObject("XR Origin (placeholder - replace with real rig)");
            var cam = new GameObject("Main Camera");
            cam.transform.SetParent(stub.transform);
            cam.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            cam.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.AddComponent<AudioListener>();
            stub.tag = "Player";
            return stub;
        }

        static void BuildComfortSettings()
        {
            new GameObject("ComfortSettings").AddComponent<ComfortSettings>();
        }

        static void BuildRewardSystem()
        {
            new GameObject("RewardSystem").AddComponent<RewardSystem>();
        }

        static void BuildSafeZone()
        {
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "Calm Down Safe Zone";
            Object.DestroyImmediate(zone.GetComponent<Collider>());

            var trigger = zone.AddComponent<CapsuleCollider>();
            trigger.isTrigger = true;
            trigger.radius = 1.2f;
            trigger.height = 2.5f;

            zone.transform.localScale = new Vector3(1.5f, 0.02f, 1.5f);
            zone.transform.position = new Vector3(0f, 0.01f, -3f);

            var renderer = zone.GetComponent<Renderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            renderer.sharedMaterial = new Material(shader) { color = new Color(0.7f, 0.9f, 0.75f, 0.6f) };

            zone.AddComponent<CalmDownSafeZone>();
        }

        static List<GameObject> BuildSensoryObjects()
        {
            var results = new List<GameObject>();
            int count = ToneGenerator.PentatonicScale.Length;
            const float radius = 1.8f;

            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.Lerp(-60f, 60f, i / (float)(count - 1)) * Mathf.Deg2Rad;
                var position = new Vector3(Mathf.Sin(angle) * radius, 1.2f, Mathf.Cos(angle) * radius);

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = $"Sensory Orb {i + 1}";
                sphere.transform.position = position;
                sphere.transform.localScale = Vector3.one * 0.25f;

                var renderer = sphere.GetComponent<Renderer>();
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                float hue = i / (float)count;
                var idleColor = Color.HSVToRGB(hue, 0.35f, 0.95f);
                renderer.sharedMaterial = new Material(shader) { color = idleColor };

                var col = sphere.GetComponent<SphereCollider>();
                col.isTrigger = true;

                var sensory = sphere.AddComponent<SensoryObject>();
                sensory.toneFrequency = ToneGenerator.PentatonicScale[i];
                sensory.idleColor = idleColor;
                sensory.activeColor = Color.HSVToRGB(hue, 0.6f, 1f);

                results.Add(sphere);
            }

            return results;
        }

        static void BuildGazeInteractor(GameObject player)
        {
            Camera camera = player.GetComponentInChildren<Camera>();
            if (camera == null) return;

            var gaze = camera.gameObject.AddComponent<GazeInteractor>();

            GameObject reticle = GameObject.CreatePrimitive(PrimitiveType.Quad);
            reticle.name = "Gaze Reticle";
            Object.DestroyImmediate(reticle.GetComponent<Collider>());
            reticle.transform.localScale = Vector3.one * 0.05f;
            gaze.reticle = reticle.transform;
        }

        static void BuildAccessibilityMenu()
        {
            var holder = new GameObject("Accessibility Menu (placeholder)");
            holder.transform.position = new Vector3(0f, 1.4f, 1.5f);
            holder.AddComponent<AccessibilityMenu>();

            Debug.LogWarning("Accessibility Menu placeholder created. Add a world-space Canvas with an " +
                              "EventSystem (Tracked Device Graphic Raycaster + XR UI Input Module) and " +
                              "Slider/Toggle widgets, then assign them on the AccessibilityMenu component " +
                              "- see README 'Building the settings panel'.");
        }
    }
}
