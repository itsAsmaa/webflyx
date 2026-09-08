# Calm Explorer — a sensory-friendly VR experience

A small, low-stimulation VR "room" built for autistic children to explore at
their own pace. There's no score, no timer, no fail state, and no forced
interaction — just a handful of gently-responsive objects, a safe retreat
space, and settings that let a caregiver (or the child, if they're able) tune
sound, brightness, and interaction style before or during play.

This is a **Unity project scaffold**, not a finished, tested product. It was
built without access to the Unity Editor, so every visual and audio element
is generated procedurally in code (primitive shapes + runtime materials,
synthesized tones) rather than relying on imported art/audio assets — the
project opens and runs without anything missing. Before using this with real
children, it needs real playtesting with occupational therapists, autism
specialists, and the children's caregivers — the design choices below are
general accessibility best practices, not a clinical prescription, and every
child's sensory preferences differ.

## Design principles

- **No time pressure, no failure.** Every interactive object toggles back and
  forth; nothing punishes a "wrong" input because there isn't one.
- **Predictable, repeatable feedback.** The same action always produces the
  same gentle audio/visual response — important for children who rely on
  predictability to feel safe.
- **A safe retreat, always available.** The "Calm Down Safe Zone" dims lights
  and ducks ambient sound the moment the player steps into it — an
  always-on option to de-escalate, never gated behind anything.
- **Non-dissonant sound.** All tones are drawn from a pentatonic scale, so any
  combination that plays together stays pleasant instead of clashing —
  meaningful for children with sound sensitivity.
- **More than one way to interact.** Hand controllers (grab/trigger select via
  XR Interaction Toolkit) work, but so does gaze-and-dwell selection for
  children who have difficulty with controllers.
- **Everything is adjustable, nothing is a surprise.** Master volume,
  brightness, particle intensity, locomotion style (teleport vs. room-scale
  only), and whether gentle visual hints ("guided mode") appear are all
  exposed as settings and persisted between sessions.
- **Calm palette.** Soft pastel colors, warm low-intensity lighting, no
  flashing or sudden motion anywhere in the scene.

## Requirements

- Unity **2022.3 LTS** (any recent patch release)
- Packages (already listed in `Packages/manifest.json`, Unity will fetch them
  on first open):
  - XR Interaction Toolkit
  - XR Plugin Management + OpenXR
  - Input System
  - Universal Render Pipeline
  - TextMeshPro

## Setup

1. Open this folder (`calm-explorer-vr/`) as a Unity project (Unity Hub →
   Add → select this folder). Let it import packages.
2. **Window → Package Manager → XR Interaction Toolkit → Samples** and import
   the **Starter Assets** sample. This gives you the `XR Origin (VR)` prefab
   used for headset/controller tracking (kept out of this repo since it
   ships with the package).
3. **Edit → Project Settings → XR Plug-in Management** → enable **OpenXR**
   for your target platform (PC or Android/Meta Quest).
4. Create a new empty scene, drag the `XR Origin (VR)` prefab into it, then
   run **Calm Explorer → Build Demo Scene** from the menu bar. This editor
   tool (`Assets/Editor/SceneBuilder.cs`) assembles the rest of the scene
   around your XR rig: lighting, floor, the safe zone, five sensory orbs,
   the gaze interactor, and the settings objects — and saves it to
   `Assets/Scenes/CalmExplorer.unity`.
   - If no XR Origin is found, the tool creates a plain placeholder camera
     so you can preview the scene layout, and logs a warning reminding you
     to swap in the real rig before testing on a headset.
5. **Building the settings panel:** the tool creates an empty
   `AccessibilityMenu` placeholder object and logs a reminder — add a
   world-space `Canvas` with an `EventSystem` (using a
   `Tracked Device Graphic Raycaster` on the canvas and an
   `XR UI Input Module` on the event system, both added by XR Interaction
   Toolkit) plus `Slider`/`Toggle` widgets for volume, brightness, particle
   intensity, gaze toggle, guided-mode toggle, and locomotion toggle. Assign
   those widgets to the matching fields on the `AccessibilityMenu`
   component. This part is genuinely easier to lay out visually in the
   Editor than to generate blind.
6. Press Play. Without a headset connected, install the XR Interaction
   Toolkit's **XR Device Simulator** sample to test interactions with mouse
   and keyboard.

## What's in `Assets/Scripts`

| Script | Purpose |
|---|---|
| `Comfort/ComfortSettings.cs` | Persisted sensory preferences (volume, brightness, particle intensity, locomotion mode, vignette, gaze on/off, guided mode) |
| `Interaction/SensoryObject.cs` | The interactive orbs — hover plays a soft tone, select toggles a calm color change and a reward |
| `Interaction/GazeInteractor.cs` | Dwell-based gaze selection, an alternative to controller grab/select |
| `Interaction/IGazeSelectable.cs` | Interface implemented by anything gaze can select |
| `Audio/ToneGenerator.cs` | Generates soft sine-wave tones at runtime (pentatonic scale) — no audio assets needed |
| `RewardSystem.cs` | Plays a gentle celebratory particle burst + chime; scaled by the player's comfort settings |
| `CalmDownSafeZone.cs` | Dims lighting and ducks ambient sound while the player stands in the safe zone |
| `UI/AccessibilityMenu.cs` | Binds Slider/Toggle UI widgets to `ComfortSettings` |
| `SceneSequencer.cs` | Optional gentle visual hint (a soft point light) toward the next unexplored orb, only when "guided mode" is on |
| `Editor/SceneBuilder.cs` | One-click scene assembly (Editor-only, excluded from builds) |

## Known limitations / next steps

- No real 3D models, textures, or recorded audio — everything is primitives
  and synthesized tone by design (see above), but a real art pass would help
  engagement.
- The accessibility settings UI needs to be laid out in-editor (step 5).
- This has not been tested on a headset or with actual users. Before any
  real-world use, get it in front of an occupational therapist and the
  children's caregivers, and expect to change defaults based on what you
  learn.
