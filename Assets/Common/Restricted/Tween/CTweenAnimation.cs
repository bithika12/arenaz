using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using DG.Tweening;
using UnityEngine;
#if true // UI_MARKER
using UnityEngine.UI;
#endif
#if false // TEXTMESHPRO_MARKER
using TMPro;
#endif

#pragma warning disable 1591
namespace RedApple.Tweening
{
    /// <summary>
    /// Attach this to a GameObject to create a tween
    /// </summary>
    [AddComponentMenu("Red Apple/Tween/CTween Animation")]
    public sealed class CTweenAnimation : ABSAnimationComponent
    {
        #region Enums
        public enum EAnimationMode
        {
            None = 0,
            OnEntry,
            OnExit,
        }

        public enum EAnimationType
        {
            None = 0,
            Move, LocalMove,
            Rotate, LocalRotate,
            Scale,
            Color, Fade,
            Text,
            PunchPosition, PunchRotation, PunchScale,
            ShakePosition, ShakeRotation, ShakeScale,
            CameraAspect, CameraBackgroundColor, CameraFieldOfView, CameraOrthoSize, CameraPixelRect, CameraRect,
            UIWidthHeight
        }

        public enum ETargetType
        {
            Unset = 0,

            Camera,
            CanvasGroup,
            Image,
            Light,
            RectTransform,
            Renderer, SpriteRenderer,
            Rigidbody, Rigidbody2D,
            Text,
            Transform,

            tk2dBaseSprite,
            tk2dTextMesh,

            TextMeshPro,
            TextMeshProUGUI
        }
        #endregion

        #region EVENTS - EDITOR-ONLY
        /// <summary>Used internally by the editor</summary>
        public static event Action<CTweenAnimation> OnReset;
        private static void DispatchOnReset(CTweenAnimation a_Anim) { if (OnReset != null) OnReset(a_Anim); }
        #endregion

        #region Public Variables
        public EAnimationMode AnimationMode;

        public bool TargetIsSelf = true; // If FALSE allows to set the target manually
        public GameObject TargetGO = null; // Used in case targetIsSelf is FALSE
        // If TRUE always uses the GO containing this DOTweenAnimation (and not the one containing the target) as DOTween's SetTarget target
        public bool TweenTargetIsTargetGO = true;

        public float Delay;
        public float Duration = 1;
        public Ease EaseType = Ease.OutQuad;
        public AnimationCurve EaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public LoopType Loop = LoopType.Restart;
        public int Loops = 1;
        public string Id = "";
        public bool IsRelative;
        public bool IsFrom;
        public bool IsIndependentUpdate = false;
        public bool AutoKill = false;

        public bool IsActive = true;
        public bool IsValid;
        public Component Target;
        public EAnimationType AnimationType;
        public ETargetType TargetType;
        public ETargetType ForcedTargetType; // Used when choosing between multiple targets
        public bool AutoPlay = false;
        public bool UseTargetAsVectorThree;

        public float EndValueFloat;
        public Vector3 EndValueVectorThree;
        public Vector2 EndValueVectorTwo;
        public Color EndValueColor = new Color(1, 1, 1, 1);
        public string EndValueString = "";
        public Rect EndValueRect = new Rect(0, 0, 0, 0);
        public Transform EndValueTransform;

        public bool OptionalBoolZero;
        public float OptionalFloatZero;
        public int OptionalIntZero;
        public RotateMode OptionalRotationMode = RotateMode.Fast;
        public ScrambleMode OptionalScrambleMode = ScrambleMode.None;
        public string OptionalString;

        [HideInInspector]
        public bool AllowResetId = true;
        #endregion

        #region Private Variables
        private bool _tweenCreated; // TRUE after the tweens have been created
        private int _playCount = -1; // Used when calling DOPlayNext
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (!IsActive || !IsValid) return;

            if (AnimationType != EAnimationType.Move || !UseTargetAsVectorThree)
            {
                // Don't create tweens if we're using a RectTransform as a Move target,
                // because that will work only inside Start
                CreateTween();
                _tweenCreated = true;
            }
        }

        private void Start()
        {
            if (_tweenCreated || !IsActive || !IsValid) return;

            CreateTween();
            _tweenCreated = true;
        }

        private void Reset()
        {
            DispatchOnReset(this);
        }

        private void OnDestroy()
        {
            if (tween != null && tween.IsActive()) tween.Kill();
            tween = null;
        }
        #endregion

        #region Public Methods

        // Used also by CTweenAnimationInspector when applying runtime changes and restarting
        public void CreateTween()
        {
            GameObject t_TweenGO = GetTweenGO();
            if (Target == null || t_TweenGO == null)
            {
                if (TargetIsSelf && Target == null)
                {
                    // Old error caused during upgrade from DOTween Pro 0.9.255
                    Debug.LogWarning(string.Format("{0} :: This CTweenAnimation's target is NULL, because the animation was created with a DOTween Pro version older than 0.9.255. To fix this, exit Play mode then simply select this object, and it will update automatically", this.gameObject.name), this.gameObject);
                }
                else
                {
                    // Missing non-self target
                    Debug.LogWarning(string.Format("{0} :: This CTweenAnimation's target/GameObject is unset: the tween will not be created.", this.gameObject.name), this.gameObject);
                }
                return;
            }

            if (ForcedTargetType != ETargetType.Unset) TargetType = ForcedTargetType;
            if (TargetType == ETargetType.Unset)
            {
                // Legacy CTweenAnimation (made with a version older than 0.9.450) without stored TargetType > assign it now
                TargetType = TypeToDOTargetType(Target.GetType());
            }

            switch (AnimationType)
            {
                case EAnimationType.None:
                    break;
                case EAnimationType.Move:
                    if (UseTargetAsVectorThree)
                        OnMoveUseTargetAsVectorThree();
                    AnimationTypeMoveState();
                    break;
                case EAnimationType.LocalMove:
                    tween = t_TweenGO.transform.DOLocalMove(EndValueVectorThree, Duration, OptionalBoolZero);
                    break;
                case EAnimationType.Rotate:
                    AnimationTypeRotateState();
                    break;
                case EAnimationType.LocalRotate:
                    tween = t_TweenGO.transform.DOLocalRotate(EndValueVectorThree, Duration, OptionalRotationMode);
                    break;
                case EAnimationType.Scale:
                    AnimationTypeScaleState(t_TweenGO);
                    break;
#if true // UI_MARKER
                case EAnimationType.UIWidthHeight:
                    tween = ((RectTransform)Target).DOSizeDelta(OptionalBoolZero ? new Vector2(EndValueFloat, EndValueFloat) : EndValueVectorTwo, Duration);
                    break;
#endif
                case EAnimationType.Color:
                    AnimationTypeColorState();
                    break;
                case EAnimationType.Fade:
                    AnimationTypeFadeState();
                    break;
                case EAnimationType.Text:
                    AnimationTypeTextState();
                    break;
                case EAnimationType.PunchPosition:
                    AnimationTypePunchPositionState();
                    break;
                case EAnimationType.PunchScale:
                    tween = t_TweenGO.transform.DOPunchScale(EndValueVectorThree, Duration, OptionalIntZero, OptionalFloatZero);
                    break;
                case EAnimationType.PunchRotation:
                    tween = t_TweenGO.transform.DOPunchRotation(EndValueVectorThree, Duration, OptionalIntZero, OptionalFloatZero);
                    break;
                case EAnimationType.ShakePosition:
                    AnimationTypeShakePositionState();
                    break;
                case EAnimationType.ShakeScale:
                    tween = t_TweenGO.transform.DOShakeScale(Duration, EndValueVectorThree, OptionalIntZero, OptionalFloatZero);
                    break;
                case EAnimationType.ShakeRotation:
                    tween = t_TweenGO.transform.DOShakeRotation(Duration, EndValueVectorThree, OptionalIntZero, OptionalFloatZero);
                    break;
                case EAnimationType.CameraAspect:
                    tween = ((Camera)Target).DOAspect(EndValueFloat, Duration);
                    break;
                case EAnimationType.CameraBackgroundColor:
                    tween = ((Camera)Target).DOColor(EndValueColor, Duration);
                    break;
                case EAnimationType.CameraFieldOfView:
                    tween = ((Camera)Target).DOFieldOfView(EndValueFloat, Duration);
                    break;
                case EAnimationType.CameraOrthoSize:
                    tween = ((Camera)Target).DOOrthoSize(EndValueFloat, Duration);
                    break;
                case EAnimationType.CameraPixelRect:
                    tween = ((Camera)Target).DOPixelRect(EndValueRect, Duration);
                    break;
                case EAnimationType.CameraRect:
                    tween = ((Camera)Target).DORect(EndValueRect, Duration);
                    break;
            }

            if (tween == null) return;

            if (IsFrom)
            {
                ((Tweener)tween).From(IsRelative);
            }
            else
            {
                tween.SetRelative(IsRelative);
            }

            GameObject setTarget = TargetIsSelf || !TweenTargetIsTargetGO ? this.gameObject : TargetGO;
            tween.SetTarget(setTarget).SetDelay(Delay).SetLoops(Loops, Loop).SetAutoKill(AutoKill).OnKill(() => tween = null);

            if (isSpeedBased)
                tween.SetSpeedBased();

            if (EaseType == Ease.INTERNAL_Custom)
                tween.SetEase(EaseCurve);
            else
                tween.SetEase(EaseType);

            if (!string.IsNullOrEmpty(Id))
                tween.SetId(Id);

            tween.SetUpdate(IsIndependentUpdate);
            HandleCallbackEvents();
        }

        // Below methods are here so they can be called directly via Unity's UGUI event system

        public override void DOPlay() => DOTween.Play(this.gameObject);

        public override void DOPlayBackwards() => DOTween.PlayBackwards(this.gameObject);

        public override void DOPlayForward() => DOTween.PlayForward(this.gameObject);

        public override void DOPause() => DOTween.Pause(this.gameObject);

        public override void DOTogglePause() => DOTween.TogglePause(this.gameObject);

        public override void DORewind()
        {
            _playCount = -1;
            // Rewind using Components order (in case there are multiple animations on the same property)
            CTweenAnimation[] anims = this.gameObject.GetComponents<CTweenAnimation>();
            for (int i = anims.Length - 1; i > -1; --i)
            {
                Tween t = anims[i].tween;
                if (t != null && t.IsInitialized()) 
                    anims[i].tween.Rewind();
            }
            // DOTween.Rewind(this.gameObject);
        }

        /// <summary>
        /// Restarts the tween
        /// </summary>
        public override void DORestart() => DORestart(false);

        /// <summary>
        /// Restarts the tween
        /// </summary>
        /// <param name="fromHere">If TRUE, re-evaluates the tween's start and end values from its current position.
        /// Set it to TRUE when spawning the same DOTweenAnimation in different positions (like when using a pooling system)</param>
        public override void DORestart(bool fromHere)
        {
            _playCount = -1;
            if (tween == null)
            {
                if (Debugger.logPriority > 1) 
                    Debugger.LogNullTween(tween); return;
            }
            if (fromHere && IsRelative) 
                ReEvaluateRelativeTween();
            DOTween.Restart(this.gameObject);
        }

        public override void DOComplete() => DOTween.Complete(this.gameObject);

        public override void DOKill()
        {
            DOTween.Kill(this.gameObject);
            tween = null;
        }

        #region Specifics
        public void DOPlayById(string id) => DOTween.Play(this.gameObject, id);

        public void DOPlayAllById(string id) => DOTween.Play(id);

        public void DOPauseAllById(string id) => DOTween.Pause(id);

        public void DOPlayBackwardsById(string id) => DOTween.PlayBackwards(this.gameObject, id);

        public void DOPlayBackwardsAllById(string id) => DOTween.PlayBackwards(id);

        public void DOPlayForwardById(string id) => DOTween.PlayForward(this.gameObject, id);

        public void DOPlayForwardAllById(string id) => DOTween.PlayForward(id);

        public void DOPlayNext()
        {
            CTweenAnimation[] anims = this.GetComponents<CTweenAnimation>();
            while (_playCount < anims.Length - 1)
            {
                _playCount++;
                CTweenAnimation anim = anims[_playCount];
                if (anim != null && anim.tween != null && !anim.tween.IsPlaying() && !anim.tween.IsComplete())
                {
                    anim.tween.Play();
                    break;
                }
            }
        }

        public void DORewindAndPlayNext()
        {
            _playCount = -1;
            DOTween.Rewind(this.gameObject);
            DOPlayNext();
        }

        public void DORewindAllById(string id)
        {
            _playCount = -1;
            DOTween.Rewind(id);
        }

        public void DORestartById(string id)
        {
            _playCount = -1;
            DOTween.Restart(this.gameObject, id);
        }

        public void DORestartAllById(string id)
        {
            _playCount = -1;
            DOTween.Restart(id);
        }

        /// <summary>
        /// Returns the tweens created by this DOTweenAnimation, in the same order as they appear in the Inspector (top to bottom)
        /// </summary>
        public List<Tween> GetTweens()
        {
            //return DOTween.TweensByTarget(this.gameObject);

            List<Tween> result = new List<Tween>();
            CTweenAnimation[] anims = this.GetComponents<CTweenAnimation>();
            foreach (CTweenAnimation anim in anims) 
                result.Add(anim.tween);
            return result;
        }
        #endregion

        #region Internal (Also used by Inspector)
        public static ETargetType TypeToDOTargetType(Type t)
        {
            string str = t.ToString();
            int dotIndex = str.LastIndexOf(".");

            if (dotIndex != -1) 
                str = str.Substring(dotIndex + 1);
            if (str.IndexOf("Renderer") != -1 && (str != "SpriteRenderer")) 
                str = "Renderer";

            //#if true // PHYSICS_MARKER
            //            if (str == "Rigidbody") str = "Transform";
            //#endif
            //#if true // PHYSICS2D_MARKER
            //            if (str == "Rigidbody2D") str = "Transform";
            //#endif
#if true // UI_MARKER
            //            if (str == "RectTransform") str = "Transform";
            if (str == "RawImage") 
                str = "Image"; // RawImages are managed like Images for DOTweenAnimation (color and fade use Graphic target anyway)
#endif
            return (ETargetType)Enum.Parse(typeof(ETargetType), str);
        }

        // Editor preview system
        /// <summary>
        /// Previews the tween in the editor. Only for DOTween internal usage: don't use otherwise.
        /// </summary>
        public Tween CreateEditorPreview()
        {
            if (Application.isPlaying) 
                return null;

            CreateTween();
            return tween;
        }
        #endregion

        #endregion

        #region Private Methods
        // Returns the gameObject whose target component should be animated
        private GameObject GetTweenGO()
        {
            return TargetIsSelf ? this.gameObject : TargetGO;
        }

        // Re-evaluate relative position of path
        private void ReEvaluateRelativeTween()
        {
            GameObject tweenGO = GetTweenGO();
            if (tweenGO == null)
            {
                Debug.LogWarning(string.Format("{0} :: This CTweenAnimation's target/GameObject is unset: the tween will not be created.", this.gameObject.name), this.gameObject);
                return;
            }
            if (AnimationType == EAnimationType.Move)
            {
                ((Tweener)tween).ChangeEndValue(tweenGO.transform.position + EndValueVectorThree, true);
            }
            else if (AnimationType == EAnimationType.LocalMove)
            {
                ((Tweener)tween).ChangeEndValue(tweenGO.transform.localPosition + EndValueVectorThree, true);
            }
        }

        private void OnMoveUseTargetAsVectorThree()
        {
            IsRelative = false;
            if (EndValueTransform == null)
            {
                Debug.LogWarning(string.Format("{0} :: This tween's TO target is NULL, a Vector3 of (0,0,0) will be used instead", this.gameObject.name), this.gameObject);
                EndValueVectorThree = Vector3.zero;
            }
            else
            {
#if true // UI_MARKER
                if (TargetType == ETargetType.RectTransform)
                {
                    RectTransform endValueT = EndValueTransform as RectTransform;
                    if (endValueT == null)
                    {
                        Debug.LogWarning(string.Format("{0} :: This tween's TO target should be a RectTransform, a Vector3 of (0,0,0) will be used instead", this.gameObject.name), this.gameObject);
                        EndValueVectorThree = Vector3.zero;
                    }
                    else
                    {
                        RectTransform rTarget = Target as RectTransform;
                        if (rTarget == null)
                        {
                            Debug.LogWarning(string.Format("{0} :: This tween's target and TO target are not of the same type. Please reassign the values", this.gameObject.name), this.gameObject);
                        }
                        else
                        {
                            // Problem: doesn't work inside Awake (ararargh!)
                            EndValueVectorThree = DOTweenModuleUI.Utils.SwitchToRectTransform(endValueT, rTarget);
                        }
                    }
                }
                else
#endif
                    EndValueVectorThree = EndValueTransform.position;
            }
        }

        private void AnimationTypeMoveState()
        {
            switch (TargetType)
            {
                case ETargetType.Transform:
                    tween = ((Transform)Target).DOMove(EndValueVectorThree, Duration, OptionalBoolZero);
                    break;
                case ETargetType.RectTransform:
#if true // UI_MARKER
                    tween = ((RectTransform)Target).DOAnchorPos3D(EndValueVectorThree, Duration, OptionalBoolZero);
#else
                    tween = ((Transform)Target).DOMove(EndValueVectorThree, Duration, OptionalBoolZero);
#endif
                    break;
                case ETargetType.Rigidbody:
#if true // PHYSICS_MARKER
                    tween = ((Rigidbody)Target).DOMove(EndValueVectorThree, Duration, OptionalBoolZero);
#else
                    tween = ((Transform)Target).DOMove(EndValueVectorThree, Duration, OptionalBoolZero);
#endif
                    break;
                case ETargetType.Rigidbody2D:
#if true // PHYSICS2D_MARKER
                    tween = ((Rigidbody2D)Target).DOMove(EndValueVectorThree, Duration, OptionalBoolZero);
#else
                    tween = ((Transform)Target).DOMove(EndValueVectorThree, Duration, OptionalBoolZero);
#endif
                    break;
            }
        }

        private void AnimationTypeRotateState()
        {
            switch (TargetType)
            {
                case ETargetType.Transform:
                    tween = ((Transform)Target).DORotate(EndValueVectorThree, Duration, OptionalRotationMode);
                    break;
                case ETargetType.Rigidbody:
#if true // PHYSICS_MARKER
                    tween = ((Rigidbody)Target).DORotate(EndValueVectorThree, Duration, OptionalRotationMode);
#else
                    tween = ((Transform)Target).DORotate(EndValueVectorThree, Duration, OptionalRotationMode);
#endif
                    break;
                case ETargetType.Rigidbody2D:
#if true // PHYSICS2D_MARKER
                    tween = ((Rigidbody2D)Target).DORotate(EndValueFloat, Duration);
#else
                    tween = ((Transform)Target).DORotate(EndValueVectorThree, Duration, OptionalRotationMode);
#endif
                    break;
            }
        }

        private void AnimationTypeScaleState(GameObject a_TweenGO)
        {
            switch (TargetType)
            {
#if false // TK2D_MARKER
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)Target).DOScale(OptionalBoolZero ? new Vector3(EndValueFloat, EndValueFloat, EndValueFloat) : EndValueVectorThree, Duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)Target).DOScale(OptionalBoolZero ? new Vector3(EndValueFloat, EndValueFloat, EndValueFloat) : EndValueVectorThree, Duration);
                    break;
#endif
                default:
                    tween = a_TweenGO.transform.DOScale(OptionalBoolZero ? new Vector3(EndValueFloat, EndValueFloat, EndValueFloat) : EndValueVectorThree, Duration);
                    break;
            }
        }

        private void AnimationTypeColorState()
        {
            IsRelative = false;
            switch (TargetType)
            {
                case ETargetType.Renderer:
                    tween = ((Renderer)Target).material.DOColor(EndValueColor, Duration);
                    break;
                case ETargetType.Light:
                    tween = ((Light)Target).DOColor(EndValueColor, Duration);
                    break;
#if true // SPRITE_MARKER
                case ETargetType.SpriteRenderer:
                    tween = ((SpriteRenderer)Target).DOColor(EndValueColor, Duration);
                    break;
#endif
#if true // UI_MARKER
                case ETargetType.Image:
                    tween = ((Graphic)Target).DOColor(EndValueColor, Duration);
                    break;
                case ETargetType.Text:
                    tween = ((Text)Target).DOColor(EndValueColor, Duration);
                    break;
#endif
#if false // TK2D_MARKER
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)Target).DOColor(EndValueColor, Duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)Target).DOColor(EndValueColor, Duration);
                    break;
#endif
#if false // TEXTMESHPRO_MARKER
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)Target).DOColor(EndValueColor, Duration);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)Target).DOColor(EndValueColor, Duration);
                    break;
#endif
            }
        }

        private void AnimationTypeFadeState()
        {
            IsRelative = false;
            switch (TargetType)
            {
                case ETargetType.Renderer:
                    tween = ((Renderer)Target).material.DOFade(EndValueFloat, Duration);
                    break;
                case ETargetType.Light:
                    tween = ((Light)Target).DOIntensity(EndValueFloat, Duration);
                    break;
#if true // SPRITE_MARKER
                case ETargetType.SpriteRenderer:
                    tween = ((SpriteRenderer)Target).DOFade(EndValueFloat, Duration);
                    break;
#endif
#if true // UI_MARKER
                case ETargetType.Image:
                    tween = ((Graphic)Target).DOFade(EndValueFloat, Duration);
                    break;
                case ETargetType.Text:
                    tween = ((Text)Target).DOFade(EndValueFloat, Duration);
                    break;
                case ETargetType.CanvasGroup:
                    tween = ((CanvasGroup)Target).DOFade(EndValueFloat, Duration);
                    break;
#endif
#if false // TK2D_MARKER
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)Target).DOFade(EndValueFloat, Duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)Target).DOFade(EndValueFloat, Duration);
                    break;
#endif
#if false // TEXTMESHPRO_MARKER
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)Target).DOFade(EndValueFloat, Duration);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)Target).DOFade(EndValueFloat, Duration);
                    break;
#endif
            }
        }

        private void AnimationTypeTextState()
        {
#if true // UI_MARKER
            switch (TargetType)
            {
                case ETargetType.Text:
                    tween = ((Text)Target).DOText(EndValueString, Duration, OptionalBoolZero, OptionalScrambleMode, OptionalString);
                    break;
            }
#endif
#if false // TK2D_MARKER
            switch (TargetType)
            {
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)Target).DOText(EndValueString, Duration, OptionalBoolZero, OptionalScrambleMode, OptionalString);
                    break;
            }
#endif
#if false // TEXTMESHPRO_MARKER
            switch (TargetType)
            {
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)Target).DOText(EndValueString, Duration, OptionalBoolZero, OptionalScrambleMode, OptionalString);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)Target).DOText(EndValueString, Duration, OptionalBoolZero, OptionalScrambleMode, OptionalString);
                    break;
            }
#endif
        }

        private void AnimationTypePunchPositionState()
        {
            switch (TargetType)
            {
                case ETargetType.Transform:
                    tween = ((Transform)Target).DOPunchPosition(EndValueVectorThree, Duration, OptionalIntZero, OptionalFloatZero, OptionalBoolZero);
                    break;
#if true // UI_MARKER
                case ETargetType.RectTransform:
                    tween = ((RectTransform)Target).DOPunchAnchorPos(EndValueVectorThree, Duration, OptionalIntZero, OptionalFloatZero, OptionalBoolZero);
                    break;
#endif
            }
        }

        private void AnimationTypeShakePositionState()
        {
            switch (TargetType)
            {
                case ETargetType.Transform:
                    tween = ((Transform)Target).DOShakePosition(Duration, EndValueVectorThree, OptionalIntZero, OptionalFloatZero, OptionalBoolZero);
                    break;
#if true // UI_MARKER
                case ETargetType.RectTransform:
                    tween = ((RectTransform)Target).DOShakeAnchorPos(Duration, EndValueVectorThree, OptionalIntZero, OptionalFloatZero, OptionalBoolZero);
                    break;
#endif
            }
        }

        private void HandleCallbackEvents()
        {
            if (hasOnStart)
            {
                if (onStart != null)
                    tween.OnStart(onStart.Invoke);
            }
            else
                onStart = null;

            if (hasOnPlay)
            {
                if (onPlay != null)
                    tween.OnPlay(onPlay.Invoke);
            }
            else
                onPlay = null;

            if (hasOnUpdate)
            {
                if (onUpdate != null)
                    tween.OnUpdate(onUpdate.Invoke);
            }
            else
                onUpdate = null;

            if (hasOnStepComplete)
            {
                if (onStepComplete != null)
                    tween.OnStepComplete(onStepComplete.Invoke);
            }
            else
                onStepComplete = null;

            if (hasOnComplete)
            {
                if (onComplete != null)
                    tween.OnComplete(onComplete.Invoke);
            }
            else
                onComplete = null;

            if (hasOnRewind)
            {
                if (onRewind != null)
                    tween.OnRewind(onRewind.Invoke);
            }
            else
                onRewind = null;

            if (AutoPlay)
                tween.Play();
            else
                tween.Pause();

            if (hasOnTweenCreated && onTweenCreated != null)
                onTweenCreated.Invoke();
        }
        #endregion
    }

    public static class CTweenAnimationExtensions
    {
        /*// Doesn't work on Win 8.1
        public static bool IsSameOrSubclassOf(this Type t, Type tBase)
        {
            return t.IsSubclassOf(tBase) || t == tBase;
        }*/

        public static bool IsSameOrSubclassOf<T>(this Component t)
        {
            return t is T;
        }
    }
}
