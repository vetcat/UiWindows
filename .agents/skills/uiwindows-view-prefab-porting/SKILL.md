---
name: uiwindows-view-prefab-porting
description: Project-specific workflow for porting or adjusting Unity UI view prefabs into UI.Windows-compatible view assets in the UiWindows project. Use when creating, reviewing, or fixing OpenUI visual prefab ports, UI.Windows view prefab assets, serialized Unity UI reference components, non-code asset placement, UIDevelopScene layout previews, CanvasScaler/RectTransform/font readability issues, or prefab/scene verification for UI.Windows view assets.
---

# UI.Windows View Prefab Porting

## Purpose

Use this skill for visual Unity UI view asset work: prefab hierarchy, RectTransform layout, sprites, legacy `Text` readability, serialized view references, and static layout preview scenes.

This skill does not replace `.agents/skills/uiwindows-mvp-architecture/SKILL.md`. Use both skills when the task touches presenter binding, model/read-model ports, R3 subscriptions, UI.Windows show/hide lifecycle, or runtime behavior.

## Load First

Before editing, read:

- `AGENTS.md`
- `_bmad-output/project-context.md`
- `.agents/skills/uiwindows-mvp-architecture/SKILL.md` when the prefab has a UI.Windows view component or the task mentions presenters/MVP/OpenUI ports
- The relevant Linear issue and comments when the work is issue-tracked
- The source prefab and source view scripts named by the task

Always check `git status --short --branch` before editing.

## Scope Rules

- Keep view assets, prefabs, sprites, scenes, settings, and other non-code Unity assets outside `Assets/Scripts`.
- Put project-owned C# only under `Assets/Scripts`.
- Add only minimal C# view components needed for serialized Unity UI references.
- Do not add presenters, model/service bindings, R3 subscriptions, scene launchers, installers, localization/effects infrastructure, or OpenUI runtime unless the issue explicitly requires that runtime slice.
- Do not import OpenUI infrastructure, Zenject, UniRx, schemes, installers, or `SetActive` lifecycle patterns.
- UI.Windows lifecycle verification belongs to a runtime slice; `UIDevelopScene` is only a static layout preview scene.

## Porting Workflow

1. Identify source and destination.

- Source OpenUI prefabs normally come from `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/...`.
- Project view prefabs normally live under `Assets/Prefabs/UiWindowsMvp/...`.
- Supporting visual assets normally live under `Assets/Content/UiWindowsMvp/...`.
- Static layout preview scene is `Assets/Scenes/Develop/UIDevelopScene.unity`.

2. Inspect the source prefab and source view scripts.

- Use the source scripts only to identify serialized UI references and useful view boundaries.
- Preserve visual hierarchy, layout groups, sprites, colors, anchors, pivots, sizes, and text settings where they are part of the visual asset.
- Copy only required visual assets. Do not copy source runtime infrastructure.

3. Build or adjust the project prefab.

- The root should be the UI.Windows-compatible view object, usually with a `RectTransform` and the concrete view component.
- If the prefab is a UI.Windows view component asset, the root component should be compatible with `WindowComponent` or the existing project view pattern.
- Apply intended placement to the prefab asset, not only to a scene instance.
- Check for scene instance overrides. If a scene instance looks correct but the prefab asset differs, the source asset is still wrong.
- Avoid broad `PrefabUtility.ApplyObjectOverride` or "Apply All" operations for large prefab instances. Prefer targeted prefab-stage edits or manual YAML edits after identifying the exact `fileID`. Verify the diff immediately.

4. Handle root RectTransform deliberately.

- Top-left HUD style views should use top-left anchors and pivot on the prefab root, not a scene-only override.
- The `UiTopLeftView` baseline learned in UIW-15 is:
  - `m_AnchorMin: {x: 0, y: 1}`
  - `m_AnchorMax: {x: 0, y: 1}`
  - `m_Pivot: {x: 0, y: 1}`
  - `m_AnchoredPosition: {x: 32, y: -32}`
  - `m_SizeDelta: {x: 420, y: 520}`
- Keep the inner `Root` child stretched if it is the layout container. Do not confuse the root `UiTopLeftView` RectTransform with the child named `Root`.

5. Keep serialized view refs minimal.

- Add fields only for UI elements the future presenter needs to bind or update.
- Do not add behavior to the view component beyond local view helpers and serialized reference ownership.
- If C# changes, use Rider checks when available and Unity compile/Console verification.

## UIDevelopScene Layout Preview

Use `Assets/Scenes/Develop/UIDevelopScene.unity` to visually inspect view prefabs without running OpenUI or the future UI.Windows vertical slice.

Expected scene purpose:

- Static layout check only.
- Contains Main Camera, Directional Light, Canvas with `CanvasScaler` and `GraphicRaycaster`, EventSystem, and one or more prefab instances under the Canvas.
- Does not prove `WindowSystem.Show`, presenter binding, R3 lifetime, pooling, or model integration.

Canvas settings learned from UIW-15:

- `CanvasScaler.uiScaleMode`: `Scale With Screen Size`
- `Reference Resolution`: `1280 x 720`
- `Match Width Or Height`: `0.5`
- `Reference Pixels Per Unit`: `100`

The OpenUI develop scene used `1280 x 720`. A `1920 x 1080` reference resolution made the small editor Game View scale legacy text down too aggressively and reduced readability.

Scene instance rules:

- Keep preview-only scene placement explicit when needed.
- Remove prefab instance overrides once the correct value belongs in the prefab asset.
- Re-check prefab asset and scene instance RectTransform values after visual tweaks.

## Text And Readability Checks

- Many OpenUI samples use legacy `UnityEngine.UI.Text` and built-in dynamic fonts.
- `Best Fit` on legacy `Text` can look poor when the Canvas is scaled down.
- Fix preview scale first: use the `1280 x 720` CanvasScaler baseline and inspect in a fixed Game View resolution such as `1280 x 720` or `1920 x 1080`.
- Do not migrate to TextMeshPro just to solve preview readability unless the issue explicitly broadens the scope.

## Unity MCP Workflow

Before using Unity MCP tools, read these resources when available:

- `mcpforunity://custom-tools`
- `mcpforunity://instances`
- `mcpforunity://editor/state`

Then run only the tools needed for the asset change:

- `refresh_unity` with `scope: assets` and `compile: none` for prefab/scene YAML-only changes.
- `manage_scene validate` on `Assets/Scenes/Develop/UIDevelopScene.unity`.
- `read_console` for errors and warnings.
- `manage_prefabs get_info` and `manage_prefabs get_hierarchy` for the changed prefab.

If C# changed, request compile and run the usual Rider/Unity checks from `AGENTS.md`.

## Static Scans

Run focused scans after a prefab port:

```bash
rg -n "UniRx|Zenject|Libs\\.OpenUI|OpenUI|SetActive\\(" Assets/Scripts Assets/Prefabs/UiWindowsMvp Assets/Scenes/Develop || true
```

If the scan reports expected historical docs or unrelated files, separate those from task changes. New view-prefab work must not introduce those dependencies or lifecycle shortcuts.

## Generated Unity Noise

Separate generated/editor artifacts from task changes in the final status:

- `Assets/EditorResources/UI.Windows/WindowObjectRegistry.asset` can be regenerated by the UI.Windows editor tooling and may contain `items: []`. Do not commit it unless the task intentionally needs that registry asset.
- `Assets/_Recovery/` can appear after a Unity crash. Do not include it in task changes.
- After removing generated noise, run a short Unity refresh and check `git status --short --branch` again.

## Final Report Checklist

Report:

- Branch name.
- Changed prefab, scene, C# files, and visual assets.
- Whether `UIDevelopScene` was used for inspection.
- Unity refresh, scene validation, prefab info/hierarchy, and Console results.
- Rider results or why Rider was skipped.
- Static scan result for UniRx/Zenject/OpenUI/SetActive.
- Generated Unity noise separated from task changes.
- Remaining visual risks, such as legacy `Text` quality or unresolved scene-instance overrides.
