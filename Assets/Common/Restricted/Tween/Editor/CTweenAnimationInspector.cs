using System;
using System.Collections.Generic;
using System.IO;
using DG.DemiEditor;
using DG.DOTweenEditor.Core;
using DG.DOTweenEditor.UI;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;
using DG.DOTweenEditor;
using RedApple.Tweening;
using DOTweenSettings = DG.Tweening.Core.DOTweenSettings;
#if true // UI_MARKER
using UnityEngine.UI;
#endif
#if false // TEXTMESHPRO_MARKER
    using TMPro;
#endif

namespace RedApple.TweenEditor
{
    [CustomEditor(typeof(CTweenAnimation))]
    public sealed class CTweenAnimationInspector : ABSAnimationInspector
    {
        enum EFadeTargetType
        {
            CanvasGroup,
            Image
        }

        enum EChooseTargetMode
        {
            None,
            BetweenCanvasGroupAndImage
        }

        static readonly Dictionary<CTweenAnimation.EAnimationType, Type[]> _AnimationTypeToComponent = new Dictionary<CTweenAnimation.EAnimationType, Type[]>() {
            { CTweenAnimation.EAnimationType.Move, new[] {
#if true // PHYSICS_MARKER
                typeof(Rigidbody),
#endif
#if true // PHYSICS2D_MARKER
                typeof(Rigidbody2D),
#endif
#if true // UI_MARKER
                typeof(RectTransform),
#endif
                typeof(Transform)
            }},
            { CTweenAnimation.EAnimationType.Rotate, new[] {
#if true // PHYSICS_MARKER
                typeof(Rigidbody),
#endif
#if true // PHYSICS2D_MARKER
                typeof(Rigidbody2D),
#endif
                typeof(Transform)
            }},
            { CTweenAnimation.EAnimationType.LocalMove, new[] { typeof(Transform) } },
            { CTweenAnimation.EAnimationType.LocalRotate, new[] { typeof(Transform) } },
            { CTweenAnimation.EAnimationType.Scale, new[] { typeof(Transform) } },
            { CTweenAnimation.EAnimationType.Color, new[] {
                typeof(Light),
#if true // SPRITE_MARKER
                typeof(SpriteRenderer),
#endif
#if true // UI_MARKER
                typeof(Image), typeof(Text), typeof(RawImage),
#endif
                typeof(Renderer),
            }},
            { CTweenAnimation.EAnimationType.Fade, new[] {
                typeof(Light),
#if true // SPRITE_MARKER
                typeof(SpriteRenderer),
#endif
#if true // UI_MARKER
                typeof(Image), typeof(Text), typeof(CanvasGroup), typeof(RawImage),
#endif
                typeof(Renderer),
            }},
#if true // UI_MARKER
            { CTweenAnimation.EAnimationType.Text, new[] { typeof(Text) } },
#endif
            { CTweenAnimation.EAnimationType.PunchPosition, new[] {
#if true // UI_MARKER
                typeof(RectTransform),
#endif
                typeof(Transform)
            }},
            { CTweenAnimation.EAnimationType.PunchRotation, new[] { typeof(Transform) } },
            { CTweenAnimation.EAnimationType.PunchScale, new[] { typeof(Transform) } },
            { CTweenAnimation.EAnimationType.ShakePosition, new[] {
#if true // UI_MARKER
                typeof(RectTransform),
#endif
                typeof(Transform)
            }},
            { CTweenAnimation.EAnimationType.ShakeRotation, new[] { typeof(Transform) } },
            { CTweenAnimation.EAnimationType.ShakeScale, new[] { typeof(Transform) } },
            { CTweenAnimation.EAnimationType.CameraAspect, new[] { typeof(Camera) } },
            { CTweenAnimation.EAnimationType.CameraBackgroundColor, new[] { typeof(Camera) } },
            { CTweenAnimation.EAnimationType.CameraFieldOfView, new[] { typeof(Camera) } },
            { CTweenAnimation.EAnimationType.CameraOrthoSize, new[] { typeof(Camera) } },
            { CTweenAnimation.EAnimationType.CameraPixelRect, new[] { typeof(Camera) } },
            { CTweenAnimation.EAnimationType.CameraRect, new[] { typeof(Camera) } },
#if true // UI_MARKER
            { CTweenAnimation.EAnimationType.UIWidthHeight, new[] { typeof(RectTransform) } },
#endif
        };

#if false // TK2D_MARKER
        static readonly Dictionary<CTweenAnimation.AnimationType, Type[]> _Tk2dAnimationTypeToComponent = new Dictionary<CTweenAnimation.AnimationType, Type[]>() {
            { CTweenAnimation.AnimationType.Scale, new[] { typeof(tk2dBaseSprite), typeof(tk2dTextMesh) } },
            { CTweenAnimation.AnimationType.Color, new[] { typeof(tk2dBaseSprite), typeof(tk2dTextMesh) } },
            { CTweenAnimation.AnimationType.Fade, new[] { typeof(tk2dBaseSprite), typeof(tk2dTextMesh) } },
            { CTweenAnimation.AnimationType.Text, new[] { typeof(tk2dTextMesh) } }
        };
#endif
#if false // TEXTMESHPRO_MARKER
        static readonly Dictionary<CTweenAnimation.AnimationType, Type[]> _TMPAnimationTypeToComponent = new Dictionary<CTweenAnimation.AnimationType, Type[]>() {
            { CTweenAnimation.AnimationType.Color, new[] { typeof(TextMeshPro), typeof(TextMeshProUGUI) } },
            { CTweenAnimation.AnimationType.Fade, new[] { typeof(TextMeshPro), typeof(TextMeshProUGUI) } },
            { CTweenAnimation.AnimationType.Text, new[] { typeof(TextMeshPro), typeof(TextMeshProUGUI) } }
        };
#endif

        static readonly string[] _AnimationType = new[] {
            "None",
            "Move", "LocalMove",
            "Rotate", "LocalRotate",
            "Scale",
            "Color", "Fade",
#if true // UI_MARKER
            "Text",
#endif
#if false // TK2D_MARKER
            "Text",
#endif
#if false // TEXTMESHPRO_MARKER
            "Text",
#endif
#if true // UI_MARKER
            "UIWidthHeight",
#endif
            "Punch/Position", "Punch/Rotation", "Punch/Scale",
            "Shake/Position", "Shake/Rotation", "Shake/Scale",
            "Camera/Aspect", "Camera/BackgroundColor", "Camera/FieldOfView", "Camera/OrthoSize", "Camera/PixelRect", "Camera/Rect"
        };
        static string[] _animationTypeNoSlashes; // _AnimationType list without slashes in values
        static string[] _datString; // String representation of CTweenAnimation enum (here for caching reasons)

        CTweenAnimation _src;
        DOTweenSettings _settings;
        bool _runtimeEditMode; // If TRUE allows to change and save stuff at runtime
        bool _refreshRequired; // If TRUE refreshes components data
        int _totComponentsOnSrc; // Used to determine if a Component is added or removed from the source
        bool _isLightSrc; // Used to determine if we're tweening a Light, to set the max Fade value to more than 1
#pragma warning disable 414
        EChooseTargetMode _chooseTargetMode = EChooseTargetMode.None;
#pragma warning restore 414

        static readonly GUIContent _GuiC_selfTarget_true = new GUIContent(
            "SELF", "Will animate components on this gameObject"
        );
        static readonly GUIContent _GuiC_selfTarget_false = new GUIContent(
            "OTHER", "Will animate components on the given gameObject instead than on this one"
        );
        static readonly GUIContent _GuiC_tweenTargetIsTargetGO_true = new GUIContent(
            "Use As Tween Target", "Will set the tween target (via SetTarget, used to control a tween directly from a target) to the \"OTHER\" gameObject"
        );
        static readonly GUIContent _GuiC_tweenTargetIsTargetGO_false = new GUIContent(
            "Use As Tween Target", "Will set the tween target (via SetTarget, used to control a tween directly from a target) to the gameObject containing this animation, not the \"OTHER\" one"
        );

        #region MonoBehaviour Methods

        void OnEnable()
        {
            _src = target as CTweenAnimation;
            _settings = DOTweenUtilityWindow.GetDOTweenSettings();

            onStartProperty = base.serializedObject.FindProperty("onStart");
            onPlayProperty = base.serializedObject.FindProperty("onPlay");
            onUpdateProperty = base.serializedObject.FindProperty("onUpdate");
            onStepCompleteProperty = base.serializedObject.FindProperty("onStepComplete");
            onCompleteProperty = base.serializedObject.FindProperty("onComplete");
            onRewindProperty = base.serializedObject.FindProperty("onRewind");
            onTweenCreatedProperty = base.serializedObject.FindProperty("onTweenCreated");

            // Convert _AnimationType to _animationTypeNoSlashes
            int len = _AnimationType.Length;
            _animationTypeNoSlashes = new string[len];
            for (int i = 0; i < len; ++i)
            {
                string a = _AnimationType[i];
                a = a.Replace("/", "");
                _animationTypeNoSlashes[i] = a;
            }
        }

        void OnDisable()
        {
            CTweenPreviewManager.StopAllPreviews();
        }

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(3);
            EditorGUIUtils.SetGUIStyles();

            bool playMode = Application.isPlaying;
            _runtimeEditMode = _runtimeEditMode && playMode;

            GUILayout.BeginHorizontal();
            // EditorGUIUtils.InspectorLogo();

            GUILayout.Label(_src.AnimationType.ToString() + (string.IsNullOrEmpty(_src.Id) ? "" : " [" + _src.Id + "]"), EditorGUIUtils.sideLogoIconBoldLabelStyle);

            GUILayout.FlexibleSpace();
            _src.AnimationMode = (CTweenAnimation.EAnimationMode)EditorGUILayout.EnumPopup(_src.AnimationMode, GUILayout.Width(75));
            if (_src.AnimationMode.Equals(CTweenAnimation.EAnimationMode.OnEntry) || _src.AnimationMode.Equals(CTweenAnimation.EAnimationMode.OnExit))
            {
                _src.Id = _src.AnimationMode.ToString();
                _src.hasOnComplete = true;
                _src.AutoPlay = false;
                _src.AllowResetId = true;
            }
            else if (_src.AnimationMode.Equals(CTweenAnimation.EAnimationMode.None) && _src.AllowResetId)
            {
                _src.AllowResetId = false;
                _src.Id = "";
            }

            // Up-down buttons
            if (GUILayout.Button("▲", DeGUI.styles.button.toolIco)) UnityEditorInternal.ComponentUtility.MoveComponentUp(_src);
            if (GUILayout.Button("▼", DeGUI.styles.button.toolIco)) UnityEditorInternal.ComponentUtility.MoveComponentDown(_src);
            GUILayout.EndHorizontal();

            if (playMode)
            {
                if (_runtimeEditMode)
                {

                }
                else
                {
                    GUILayout.Space(8);
                    GUILayout.Label("Animation Editor disabled while in play mode", EditorGUIUtils.wordWrapLabelStyle);
                    if (!_src.IsActive)
                    {
                        GUILayout.Label("This animation has been toggled as inactive and won't be generated", EditorGUIUtils.wordWrapLabelStyle);
                        UnityEngine.GUI.enabled = false;
                    }
                    if (GUILayout.Button(new GUIContent("Activate Edit Mode", "Switches to Runtime Edit Mode, where you can change animations values and restart them")))
                    {
                        _runtimeEditMode = true;
                    }
                    GUILayout.Label("NOTE: when using DOPlayNext, the sequence is determined by the DOTweenAnimation Components order in the target GameObject's Inspector", EditorGUIUtils.wordWrapLabelStyle);
                    GUILayout.Space(10);
                    if (!_runtimeEditMode) return;
                }
            }

            Undo.RecordObject(_src, "DOTween Animation");
            Undo.RecordObject(_settings, "DOTween Animation");

            // _src.isValid = Validate(); // Moved down

            EditorGUIUtility.labelWidth = 110;

            if (playMode)
            {
                GUILayout.Space(4);
                DeGUILayout.Toolbar("Edit Mode Commands");
                DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("TogglePause")) _src.tween.TogglePause();
                if (GUILayout.Button("Rewind")) _src.tween.Rewind();
                if (GUILayout.Button("Restart")) _src.tween.Restart();
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Commit changes and restart"))
                {
                    _src.tween.Rewind();
                    _src.tween.Kill();
                    if (_src.IsValid)
                    {
                        _src.CreateTween();
                        _src.tween.Play();
                    }
                }
                GUILayout.Label("To apply your changes when exiting Play mode, use the Component's upper right menu and choose \"Copy Component\", then \"Paste Component Values\" after exiting Play mode", DeGUI.styles.label.wordwrap);
                DeGUILayout.EndVBox();
                GUILayout.EndHorizontal();
            }

            // Preview in editor
            bool isPreviewing = _settings.showPreviewPanel ? CTweenPreviewManager.PreviewGUI(_src) : false;

            EditorGUI.BeginDisabledGroup(isPreviewing);
            // Choose target
            GUILayout.BeginHorizontal();
            _src.IsActive = EditorGUILayout.Toggle(new GUIContent("", "If unchecked, this animation will not be created"), _src.IsActive, GUILayout.Width(14));
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginChangeCheck();
            _src.TargetIsSelf = DeGUILayout.ToggleButton(
                _src.TargetIsSelf, _src.TargetIsSelf ? _GuiC_selfTarget_true : _GuiC_selfTarget_false,
                new Color(1f, 0.78f, 0f), DeGUI.colors.bg.toggleOn, new Color(0.33f, 0.14f, 0.02f), DeGUI.colors.content.toggleOn,
                null, GUILayout.Width(47)
            );
            bool innerChanged = EditorGUI.EndChangeCheck();
            if (innerChanged)
            {
                _src.TargetGO = null;
                UnityEngine.GUI.changed = true;
            }
            if (_src.TargetIsSelf) GUILayout.Label(_GuiC_selfTarget_true.tooltip);
            else
            {
                using (new DeGUI.ColorScope(null, null, _src.TargetGO == null ? Color.red : Color.white))
                {
                    _src.TargetGO = (GameObject)EditorGUILayout.ObjectField(_src.TargetGO, typeof(GameObject), true);
                }
                _src.TweenTargetIsTargetGO = DeGUILayout.ToggleButton(
                    _src.TweenTargetIsTargetGO, _src.TweenTargetIsTargetGO ? _GuiC_tweenTargetIsTargetGO_true : _GuiC_tweenTargetIsTargetGO_false,
                    GUILayout.Width(131)
                );
            }
            bool check = EditorGUI.EndChangeCheck();
            if (check) _refreshRequired = true;
            GUILayout.EndHorizontal();

            GameObject targetGO = _src.TargetIsSelf ? _src.gameObject : _src.TargetGO;

            if (targetGO == null)
            {
                // Uses external target gameObject but it's not set
                if (_src.TargetGO != null || _src.Target != null)
                {
                    _src.TargetGO = null;
                    _src.Target = null;
                    UnityEngine.GUI.changed = true;
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                CTweenAnimation.EAnimationType prevAnimType = _src.AnimationType;
                // _src.animationType = (DOTweenAnimation.AnimationType)EditorGUILayout.EnumPopup(_src.animationType, EditorGUIUtils.popupButton);
                UnityEngine.GUI.enabled = UnityEngine.GUI.enabled && _src.IsActive;
                _src.AnimationType = AnimationToDOTweenAnimationType(_AnimationType[EditorGUILayout.Popup(DOTweenAnimationTypeToPopupId(_src.AnimationType), _AnimationType)]);
                _src.AutoPlay = DeGUILayout.ToggleButton(_src.AutoPlay, new GUIContent("AutoPlay", "If selected, the tween will play automatically"));
                _src.AutoKill = DeGUILayout.ToggleButton(_src.AutoKill, new GUIContent("AutoKill", "If selected, the tween will be killed when it completes, and won't be reusable"));
                GUILayout.EndHorizontal();
                if (prevAnimType != _src.AnimationType)
                {
                    // Set default optional values based on animation type
                    _src.EndValueTransform = null;
                    _src.UseTargetAsVectorThree = false;
                    switch (_src.AnimationType)
                    {
                        case CTweenAnimation.EAnimationType.Move:
                        case CTweenAnimation.EAnimationType.LocalMove:
                        case CTweenAnimation.EAnimationType.Rotate:
                        case CTweenAnimation.EAnimationType.LocalRotate:
                        case CTweenAnimation.EAnimationType.Scale:
                            _src.EndValueVectorThree = Vector3.zero;
                            _src.EndValueFloat = 0;
                            _src.OptionalBoolZero = _src.AnimationType == CTweenAnimation.EAnimationType.Scale;
                            break;
                        case CTweenAnimation.EAnimationType.UIWidthHeight:
                            _src.EndValueVectorThree = Vector3.zero;
                            _src.EndValueFloat = 0;
                            _src.OptionalBoolZero = _src.AnimationType == CTweenAnimation.EAnimationType.UIWidthHeight;
                            break;
                        case CTweenAnimation.EAnimationType.Color:
                        case CTweenAnimation.EAnimationType.Fade:
                            _isLightSrc = targetGO.GetComponent<Light>() != null;
                            _src.EndValueFloat = 0;
                            break;
                        case CTweenAnimation.EAnimationType.Text:
                            _src.OptionalBoolZero = true;
                            break;
                        case CTweenAnimation.EAnimationType.PunchPosition:
                        case CTweenAnimation.EAnimationType.PunchRotation:
                        case CTweenAnimation.EAnimationType.PunchScale:
                            _src.EndValueVectorThree = _src.AnimationType == CTweenAnimation.EAnimationType.PunchRotation ? new Vector3(0, 180, 0) : Vector3.one;
                            _src.OptionalFloatZero = 1;
                            _src.OptionalIntZero = 10;
                            _src.OptionalBoolZero = false;
                            break;
                        case CTweenAnimation.EAnimationType.ShakePosition:
                        case CTweenAnimation.EAnimationType.ShakeRotation:
                        case CTweenAnimation.EAnimationType.ShakeScale:
                            _src.EndValueVectorThree = _src.AnimationType == CTweenAnimation.EAnimationType.ShakeRotation ? new Vector3(90, 90, 90) : Vector3.one;
                            _src.OptionalIntZero = 10;
                            _src.OptionalFloatZero = 90;
                            _src.OptionalBoolZero = false;
                            break;
                        case CTweenAnimation.EAnimationType.CameraAspect:
                        case CTweenAnimation.EAnimationType.CameraFieldOfView:
                        case CTweenAnimation.EAnimationType.CameraOrthoSize:
                            _src.EndValueFloat = 0;
                            break;
                        case CTweenAnimation.EAnimationType.CameraPixelRect:
                        case CTweenAnimation.EAnimationType.CameraRect:
                            _src.EndValueRect = new Rect(0, 0, 0, 0);
                            break;
                    }
                }
                if (_src.AnimationType == CTweenAnimation.EAnimationType.None)
                {
                    _src.IsValid = false;
                    if (UnityEngine.GUI.changed) EditorUtility.SetDirty(_src);
                    return;
                }

                if (_refreshRequired || prevAnimType != _src.AnimationType || ComponentsChanged())
                {
                    _refreshRequired = false;
                    _src.IsValid = Validate(targetGO);
                    // See if we need to choose between multiple targets
#if true // UI_MARKER
                    if (_src.AnimationType == CTweenAnimation.EAnimationType.Fade && targetGO.GetComponent<CanvasGroup>() != null && targetGO.GetComponent<Image>() != null)
                    {
                        _chooseTargetMode = EChooseTargetMode.BetweenCanvasGroupAndImage;
                        // Reassign target and forcedTargetType if lost
                        if (_src.ForcedTargetType == CTweenAnimation.ETargetType.Unset) _src.ForcedTargetType = _src.TargetType;
                        switch (_src.ForcedTargetType)
                        {
                            case CTweenAnimation.ETargetType.CanvasGroup:
                                _src.Target = targetGO.GetComponent<CanvasGroup>();
                                break;
                            case CTweenAnimation.ETargetType.Image:
                                _src.Target = targetGO.GetComponent<Image>();
                                break;
                        }
                    }
                    else
                    {
#endif
                        _chooseTargetMode = EChooseTargetMode.None;
                        _src.ForcedTargetType = CTweenAnimation.ETargetType.Unset;
#if true // UI_MARKER
                    }
#endif
                }

                if (!_src.IsValid)
                {
                    UnityEngine.GUI.color = Color.red;
                    GUILayout.BeginVertical(UnityEngine.GUI.skin.box);
                    GUILayout.Label("No valid Component was found for the selected animation", EditorGUIUtils.wordWrapLabelStyle);
                    GUILayout.EndVertical();
                    UnityEngine.GUI.color = Color.white;
                    if (UnityEngine.GUI.changed) EditorUtility.SetDirty(_src);
                    return;
                }

#if true // UI_MARKER
                // Special cases in which multiple target types could be used (set after validation)
                if (_chooseTargetMode == EChooseTargetMode.BetweenCanvasGroupAndImage && _src.ForcedTargetType != CTweenAnimation.ETargetType.Unset)
                {
                    EFadeTargetType fadeTargetType = (EFadeTargetType)Enum.Parse(typeof(EFadeTargetType), _src.ForcedTargetType.ToString());
                    CTweenAnimation.ETargetType prevTargetType = _src.ForcedTargetType;
                    _src.ForcedTargetType = (CTweenAnimation.ETargetType)Enum.Parse(typeof(CTweenAnimation.ETargetType), EditorGUILayout.EnumPopup(_src.AnimationType + " Target", fadeTargetType).ToString());
                    if (_src.ForcedTargetType != prevTargetType)
                    {
                        // Target type change > assign correct target
                        switch (_src.ForcedTargetType)
                        {
                            case CTweenAnimation.ETargetType.CanvasGroup:
                                _src.Target = targetGO.GetComponent<CanvasGroup>();
                                break;
                            case CTweenAnimation.ETargetType.Image:
                                _src.Target = targetGO.GetComponent<Image>();
                                break;
                        }
                    }
                }
#endif

                GUILayout.BeginHorizontal();
                _src.Duration = EditorGUILayout.FloatField("Duration", _src.Duration);
                if (_src.Duration < 0) _src.Duration = 0;
                _src.isSpeedBased = DeGUILayout.ToggleButton(_src.isSpeedBased, new GUIContent("SpeedBased", "If selected, the duration will count as units/degree x second"), DeGUI.styles.button.tool, GUILayout.Width(75));
                GUILayout.EndHorizontal();
                _src.Delay = EditorGUILayout.FloatField("Delay", _src.Delay);
                if (_src.Delay < 0) _src.Delay = 0;
                _src.IsIndependentUpdate = EditorGUILayout.Toggle("Ignore TimeScale", _src.IsIndependentUpdate);
                _src.EaseType = EditorGUIUtils.FilteredEasePopup(_src.EaseType);
                if (_src.EaseType == Ease.INTERNAL_Custom)
                {
                    _src.EaseCurve = EditorGUILayout.CurveField("   Ease Curve", _src.EaseCurve);
                }
                _src.Loops = EditorGUILayout.IntField(new GUIContent("Loops", "Set to -1 for infinite loops"), _src.Loops);
                if (_src.Loops < -1) _src.Loops = -1;
                if (_src.Loops > 1 || _src.Loops == -1)
                    _src.Loop = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", _src.Loop);

                if (_src.AnimationMode.Equals(CTweenAnimation.EAnimationMode.None))
                    _src.Id = EditorGUILayout.TextField("ID", _src.Id);

                bool canBeRelative = true;
                // End value and eventual specific options
                switch (_src.AnimationType)
                {
                    case CTweenAnimation.EAnimationType.Move:
                    case CTweenAnimation.EAnimationType.LocalMove:
                        GUIEndValueV3(targetGO, _src.AnimationType == CTweenAnimation.EAnimationType.Move);
                        _src.OptionalBoolZero = EditorGUILayout.Toggle("    Snapping", _src.OptionalBoolZero);
                        canBeRelative = !_src.UseTargetAsVectorThree;
                        break;
                    case CTweenAnimation.EAnimationType.Rotate:
                    case CTweenAnimation.EAnimationType.LocalRotate:
                        bool isRigidbody2D = DOTweenModuleUtils.Physics.HasRigidbody2D(_src);
                        if (isRigidbody2D) GUIEndValueFloat();
                        else
                        {
                            GUIEndValueV3(targetGO);
                            _src.OptionalRotationMode = (RotateMode)EditorGUILayout.EnumPopup("    Rotation Mode", _src.OptionalRotationMode);
                        }
                        break;
                    case CTweenAnimation.EAnimationType.Scale:
                        if (_src.OptionalBoolZero) GUIEndValueFloat();
                        else GUIEndValueV3(targetGO);
                        _src.OptionalBoolZero = EditorGUILayout.Toggle("Uniform Scale", _src.OptionalBoolZero);
                        break;
                    case CTweenAnimation.EAnimationType.UIWidthHeight:
                        if (_src.OptionalBoolZero) GUIEndValueFloat();
                        else GUIEndValueV2();
                        _src.OptionalBoolZero = EditorGUILayout.Toggle("Uniform Scale", _src.OptionalBoolZero);
                        break;
                    case CTweenAnimation.EAnimationType.Color:
                        GUIEndValueColor();
                        canBeRelative = false;
                        break;
                    case CTweenAnimation.EAnimationType.Fade:
                        GUIEndValueFloat();
                        if (_src.EndValueFloat < 0) _src.EndValueFloat = 0;
                        if (!_isLightSrc && _src.EndValueFloat > 1) _src.EndValueFloat = 1;
                        canBeRelative = false;
                        break;
                    case CTweenAnimation.EAnimationType.Text:
                        GUIEndValueString();
                        _src.OptionalBoolZero = EditorGUILayout.Toggle("Rich Text Enabled", _src.OptionalBoolZero);
                        _src.OptionalScrambleMode = (ScrambleMode)EditorGUILayout.EnumPopup("Scramble Mode", _src.OptionalScrambleMode);
                        _src.OptionalString = EditorGUILayout.TextField(new GUIContent("Custom Scramble", "Custom characters to use in case of ScrambleMode.Custom"), _src.OptionalString);
                        break;
                    case CTweenAnimation.EAnimationType.PunchPosition:
                    case CTweenAnimation.EAnimationType.PunchRotation:
                    case CTweenAnimation.EAnimationType.PunchScale:
                        GUIEndValueV3(targetGO);
                        canBeRelative = false;
                        _src.OptionalIntZero = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the punch vibrate"), _src.OptionalIntZero, 1, 50);
                        _src.OptionalFloatZero = EditorGUILayout.Slider(new GUIContent("    Elasticity", "How much the vector will go beyond the starting position when bouncing backwards"), _src.OptionalFloatZero, 0, 1);
                        if (_src.AnimationType == CTweenAnimation.EAnimationType.PunchPosition) _src.OptionalBoolZero = EditorGUILayout.Toggle("    Snapping", _src.OptionalBoolZero);
                        break;
                    case CTweenAnimation.EAnimationType.ShakePosition:
                    case CTweenAnimation.EAnimationType.ShakeRotation:
                    case CTweenAnimation.EAnimationType.ShakeScale:
                        GUIEndValueV3(targetGO);
                        canBeRelative = false;
                        _src.OptionalIntZero = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the shake vibrate"), _src.OptionalIntZero, 1, 50);
                        _src.OptionalFloatZero = EditorGUILayout.Slider(new GUIContent("    Randomness", "The shake randomness"), _src.OptionalFloatZero, 0, 90);
                        if (_src.AnimationType == CTweenAnimation.EAnimationType.ShakePosition) _src.OptionalBoolZero = EditorGUILayout.Toggle("    Snapping", _src.OptionalBoolZero);
                        break;
                    case CTweenAnimation.EAnimationType.CameraAspect:
                    case CTweenAnimation.EAnimationType.CameraFieldOfView:
                    case CTweenAnimation.EAnimationType.CameraOrthoSize:
                        GUIEndValueFloat();
                        canBeRelative = false;
                        break;
                    case CTweenAnimation.EAnimationType.CameraBackgroundColor:
                        GUIEndValueColor();
                        canBeRelative = false;
                        break;
                    case CTweenAnimation.EAnimationType.CameraPixelRect:
                    case CTweenAnimation.EAnimationType.CameraRect:
                        GUIEndValueRect();
                        canBeRelative = false;
                        break;
                }

                // Final settings
                if (canBeRelative) _src.IsRelative = EditorGUILayout.Toggle("    Relative", _src.IsRelative);

                // Events
                AnimationInspectorGUI.AnimationEvents(this, _src);
            }
            EditorGUI.EndDisabledGroup();

            if (UnityEngine.GUI.changed) EditorUtility.SetDirty(_src);
        }

        #endregion

        #region Methods

        // Returns TRUE if the Component layout on the src gameObject changed (a Component was added or removed)
        bool ComponentsChanged()
        {
            int prevTotComponentsOnSrc = _totComponentsOnSrc;
            _totComponentsOnSrc = _src.gameObject.GetComponents<Component>().Length;
            return prevTotComponentsOnSrc != _totComponentsOnSrc;
        }

        // Checks if a Component that can be animated with the given animationType is attached to the src
        bool Validate(GameObject targetGO)
        {
            if (_src.AnimationType == CTweenAnimation.EAnimationType.None) return false;

            Component srcTarget;
            // First check for external plugins
#if false // TK2D_MARKER
            if (_Tk2dAnimationTypeToComponent.ContainsKey(_src.animationType)) {
                foreach (Type t in _Tk2dAnimationTypeToComponent[_src.animationType]) {
                    srcTarget = targetGO.GetComponent(t);
                    if (srcTarget != null) {
                        _src.target = srcTarget;
                        _src.targetType = CTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
#endif
#if false // TEXTMESHPRO_MARKER
            if (_TMPAnimationTypeToComponent.ContainsKey(_src.animationType)) {
                foreach (Type t in _TMPAnimationTypeToComponent[_src.animationType]) {
                    srcTarget = targetGO.GetComponent(t);
                    if (srcTarget != null) {
                        _src.target = srcTarget;
                        _src.targetType = CTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
#endif
            // Then check for regular stuff
            if (_AnimationTypeToComponent.ContainsKey(_src.AnimationType))
            {
                foreach (Type t in _AnimationTypeToComponent[_src.AnimationType])
                {
                    srcTarget = targetGO.GetComponent(t);
                    if (srcTarget != null)
                    {
                        _src.Target = srcTarget;
                        _src.TargetType = CTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
            return false;
        }

        CTweenAnimation.EAnimationType AnimationToDOTweenAnimationType(string animation)
        {
            if (_datString == null) _datString = Enum.GetNames(typeof(CTweenAnimation.EAnimationType));
            animation = animation.Replace("/", "");
            return (CTweenAnimation.EAnimationType)(Array.IndexOf(_datString, animation));
        }
        int DOTweenAnimationTypeToPopupId(CTweenAnimation.EAnimationType animation)
        {
            return Array.IndexOf(_animationTypeNoSlashes, animation.ToString());
        }

        #endregion

        #region GUI Draw Methods

        void GUIEndValueFloat()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.EndValueFloat = EditorGUILayout.FloatField(_src.EndValueFloat);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueColor()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.EndValueColor = EditorGUILayout.ColorField(_src.EndValueColor);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueV3(GameObject targetGO, bool optionalTransform = false)
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            if (_src.UseTargetAsVectorThree)
            {
                Transform prevT = _src.EndValueTransform;
                _src.EndValueTransform = EditorGUILayout.ObjectField(_src.EndValueTransform, typeof(Transform), true) as Transform;
                if (_src.EndValueTransform != prevT && _src.EndValueTransform != null)
                {
#if true // UI_MARKER
                    // Check that it's a Transform for a Transform or a RectTransform for a RectTransform
                    if (targetGO.GetComponent<RectTransform>() != null)
                    {
                        if (_src.EndValueTransform.GetComponent<RectTransform>() == null)
                        {
                            EditorUtility.DisplayDialog("DOTween Pro", "For Unity UI elements, the target must also be a UI element", "Ok");
                            _src.EndValueTransform = null;
                        }
                    }
                    else if (_src.EndValueTransform.GetComponent<RectTransform>() != null)
                    {
                        EditorUtility.DisplayDialog("DOTween Pro", "You can't use a UI target for a non UI object", "Ok");
                        _src.EndValueTransform = null;
                    }
#endif
                }
            }
            else
            {
                _src.EndValueVectorThree = EditorGUILayout.Vector3Field("", _src.EndValueVectorThree, GUILayout.Height(16));
            }
            if (optionalTransform)
            {
                if (GUILayout.Button(_src.UseTargetAsVectorThree ? "target" : "value", EditorGUIUtils.sideBtStyle, GUILayout.Width(44))) _src.UseTargetAsVectorThree = !_src.UseTargetAsVectorThree;
            }
            GUILayout.EndHorizontal();
#if true // UI_MARKER
            if (_src.UseTargetAsVectorThree && _src.EndValueTransform != null && _src.Target is RectTransform)
            {
                EditorGUILayout.HelpBox("NOTE: when using a UI target, the tween will be created during Start instead of Awake", MessageType.Info);
            }
#endif
        }

        void GUIEndValueV2()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.EndValueVectorTwo = EditorGUILayout.Vector2Field("", _src.EndValueVectorTwo, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        void GUIEndValueString()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.EndValueString = EditorGUILayout.TextArea(_src.EndValueString, EditorGUIUtils.wordWrapTextArea);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueRect()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.EndValueRect = EditorGUILayout.RectField(_src.EndValueRect);
            GUILayout.EndHorizontal();
        }

        void GUIToFromButton()
        {
            if (GUILayout.Button(_src.IsFrom ? "FROM" : "TO", EditorGUIUtils.sideBtStyle, GUILayout.Width(90))) _src.IsFrom = !_src.IsFrom;
            GUILayout.Space(16);
        }

        #endregion
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    [InitializeOnLoad]
    static class CInitializer
    {
        static CInitializer()
        {
            CTweenAnimation.OnReset += OnReset;
        }

        static void OnReset(CTweenAnimation src)
        {
            DOTweenSettings settings = DOTweenUtilityWindow.GetDOTweenSettings();
            if (settings == null) return;

            Undo.RecordObject(src, "CTweenAnimation");
            //src.autoPlay = settings.defaultAutoPlay == AutoPlay.All || settings.defaultAutoPlay == AutoPlay.AutoPlayTweeners;
            //src.autoKill = settings.defaultAutoKill;
            EditorUtility.SetDirty(src);
        }
    }
}
