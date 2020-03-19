using System;
using System.Collections.Generic;
using DG.DemiEditor;
using DG.DemiLib;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using DG.DOTweenEditor;
using RedApple.Tweening;
using Object = UnityEngine.Object;

namespace RedApple.TweenEditor
{
    public static class CTweenPreviewManager
    {
        private static bool previewOnlyIfSetToAutoPlay = true;
        private static readonly Dictionary<CTweenAnimation, TweenInfo> animationToTween = new Dictionary<CTweenAnimation, TweenInfo>();
        private static readonly List<CTweenAnimation> tmpKeys = new List<CTweenAnimation>();

        #region Public Methods & GUI

        /// <summary>
        /// Returns TRUE if its actually previewing animations
        /// </summary>
        public static bool PreviewGUI(CTweenAnimation a_Src)
        {
            if (EditorApplication.isPlaying) return false;

            Styles.Init();

            bool t_IsPreviewing = animationToTween.Count > 0;
            bool t_IsPreviewingThis = t_IsPreviewing && animationToTween.ContainsKey(a_Src);

            // Preview in editor
            UnityEngine.GUI.backgroundColor = t_IsPreviewing
                ? new DeSkinColor(new Color(0.49f, 0.8f, 0.86f), new Color(0.15f, 0.26f, 0.35f))
                : new DeSkinColor(Color.white, new Color(0.13f, 0.13f, 0.13f));
            GUILayout.BeginVertical(Styles.PreviewBox);
            DeGUI.ResetGUIColors();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Preview Mode - Experimental", Styles.PreviewLabel);
            previewOnlyIfSetToAutoPlay = DeGUILayout.ToggleButton(
                previewOnlyIfSetToAutoPlay,
                new GUIContent("AutoPlay only", "If toggled only previews animations that have AutoPlay turned ON"),
                Styles.BtnOption
            );
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
            // Preview - Play
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(
                t_IsPreviewingThis || a_Src.AnimationType == CTweenAnimation.EAnimationType.None
                || !a_Src.IsActive || previewOnlyIfSetToAutoPlay && !a_Src.AutoPlay
            );
            if (GUILayout.Button("► Play", Styles.BtnPreview))
            {
                if (!t_IsPreviewing) startupGlobalPreview();
                addAnimationToGlobalPreview(a_Src);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(t_IsPreviewing);
            if (GUILayout.Button("► Play All <i>on GameObject</i>", Styles.BtnPreview))
            {
                if (!t_IsPreviewing) startupGlobalPreview();
                CTweenAnimation[] t_Anims = a_Src.gameObject.GetComponents<CTweenAnimation>();
                foreach (CTweenAnimation anim in t_Anims) addAnimationToGlobalPreview(anim);
            }
            if (GUILayout.Button("► Play All <i>in Scene</i>", Styles.BtnPreview))
            {
                if (!t_IsPreviewing) startupGlobalPreview();
                CTweenAnimation[] anims = Object.FindObjectsOfType<CTweenAnimation>();
                foreach (CTweenAnimation anim in anims) addAnimationToGlobalPreview(anim);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            // Preview - Stop
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!t_IsPreviewingThis);
            if (GUILayout.Button("■ Stop", Styles.BtnPreview))
            {
                if (animationToTween.ContainsKey(a_Src)) stopPreview(animationToTween[a_Src].TweenData);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!t_IsPreviewing);
            if (GUILayout.Button("■ Stop All <i>on GameObject</i>", Styles.BtnPreview))
            {
                stopPreview(a_Src.gameObject);
            }
            if (GUILayout.Button("■ Stop All <i>in Scene</i>", Styles.BtnPreview))
            {
                StopAllPreviews();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            if (t_IsPreviewing)
            {
                int t_PlayingTweens = 0;
                int t_CompletedTweens = 0;
                int t_PausedTweens = 0;
                foreach (KeyValuePair<CTweenAnimation, TweenInfo> element in animationToTween)
                {
                    Tween t_Tween = element.Value.TweenData;
                    if (t_Tween.IsPlaying()) t_PlayingTweens++;
                    else if (t_Tween.IsComplete()) t_CompletedTweens++;
                    else t_PausedTweens++;
                }
                GUILayout.Label("Playing Tweens: " + t_PlayingTweens, Styles.PreviewStatusLabel);
                GUILayout.Label("Completed Tweens: " + t_CompletedTweens, Styles.PreviewStatusLabel);
                //                GUILayout.Label("Paused Tweens: " + playingTweens);
            }
            GUILayout.EndVertical();

            return t_IsPreviewing;
        }

#if !(UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5)
        public static void StopAllPreviews(PlayModeStateChange state)
        {
            StopAllPreviews();
        }
#endif

        public static void StopAllPreviews()
        {
            tmpKeys.Clear();
            foreach (KeyValuePair<CTweenAnimation, TweenInfo> element in animationToTween)
            {
                tmpKeys.Add(element.Key);
            }
            stopPreview(tmpKeys);
            tmpKeys.Clear();
            animationToTween.Clear();

            DOTweenEditorPreview.Stop();
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5
            UnityEditor.EditorApplication.playmodeStateChanged -= StopAllPreviews;
#else
            UnityEditor.EditorApplication.playModeStateChanged -= StopAllPreviews;
#endif
            //            EditorApplication.playmodeStateChanged -= StopAllPreviews;

            InternalEditorUtility.RepaintAllViews();
        }

        #endregion

        #region Methods

        private static void startupGlobalPreview()
        {
            DOTweenEditorPreview.Start();
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5
            UnityEditor.EditorApplication.playmodeStateChanged += StopAllPreviews;
#else
            UnityEditor.EditorApplication.playModeStateChanged += StopAllPreviews;
#endif
            //            EditorApplication.playmodeStateChanged += StopAllPreviews;
        }

        private static void addAnimationToGlobalPreview(CTweenAnimation a_Src)
        {
            if (!a_Src.IsActive) return; // Ignore sources whose tweens have been set to inactive
            if (previewOnlyIfSetToAutoPlay && !a_Src.AutoPlay) return;

            Tween t_Tween = a_Src.CreateEditorPreview();
            animationToTween.Add(a_Src, new TweenInfo(a_Src, t_Tween, a_Src.IsFrom));
            // Tween setup
            DOTweenEditorPreview.PrepareTweenForPreview(t_Tween);
        }

        private static void stopPreview(GameObject a_Go)
        {
            tmpKeys.Clear();
            foreach (KeyValuePair<CTweenAnimation, TweenInfo> kvp in animationToTween)
            {
                if (kvp.Key.gameObject != a_Go) continue;
                tmpKeys.Add(kvp.Key);
            }
            stopPreview(tmpKeys);
            tmpKeys.Clear();

            if (animationToTween.Count == 0) StopAllPreviews();
            else InternalEditorUtility.RepaintAllViews();
        }

        private static void stopPreview(Tween a_Tween)
        {
            TweenInfo t_Info = null;
            foreach (KeyValuePair<CTweenAnimation, TweenInfo> element in animationToTween)
            {
                if (element.Value.TweenData != a_Tween) continue;
                t_Info = element.Value;
                animationToTween.Remove(element.Key);
                break;
            }
            if (t_Info == null)
            {
                Debug.LogWarning("DOTween Preview ► Couldn't find tween to stop");
                return;
            }
            if (t_Info.IsFrom)
            {
                int totLoops = t_Info.TweenData.Loops();
                if (totLoops < 0 || totLoops > 1)
                {
                    t_Info.TweenData.Goto(t_Info.TweenData.Duration(false));
                }
                else t_Info.TweenData.Complete();
            }
            else t_Info.TweenData.Rewind();
            t_Info.TweenData.Kill();
            EditorUtility.SetDirty(t_Info.TweenAnimation); // Refresh views

            if (animationToTween.Count == 0) StopAllPreviews();
            else InternalEditorUtility.RepaintAllViews();
        }

        // Stops while iterating inversely, which deals better with tweens that overwrite each other
        private static void stopPreview(List<CTweenAnimation> t_Keys)
        {
            for (int i = t_Keys.Count - 1; i > -1; --i)
            {
                CTweenAnimation anim = t_Keys[i];
                TweenInfo tInfo = animationToTween[anim];
                if (tInfo.IsFrom)
                {
                    int totLoops = tInfo.TweenData.Loops();
                    if (totLoops < 0 || totLoops > 1)
                    {
                        tInfo.TweenData.Goto(tInfo.TweenData.Duration(false));
                    }
                    else tInfo.TweenData.Complete();
                }
                else tInfo.TweenData.Rewind();
                tInfo.TweenData.Kill();
                EditorUtility.SetDirty(anim); // Refresh views
                animationToTween.Remove(anim);
            }
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        private class TweenInfo
        {
            public CTweenAnimation TweenAnimation;
            public Tween TweenData;
            public bool IsFrom;
            public TweenInfo(CTweenAnimation a_Animation, Tween a_Tween, bool a_IsFrom)
            {
                TweenAnimation = a_Animation;
                TweenData = a_Tween;
                IsFrom = a_IsFrom;
            }
        }

        private static class Styles
        {
            private static bool initialized;

            public static GUIStyle PreviewBox, PreviewLabel, BtnOption, BtnPreview, PreviewStatusLabel;

            public static void Init()
            {
                if (initialized) return;

                initialized = true;

                PreviewBox = new GUIStyle(UnityEngine.GUI.skin.box).Clone().Padding(1, 1, 0, 3)
                    .Background(DeStylePalette.squareBorderCurved_darkBorders).Border(7, 7, 7, 7);
                PreviewLabel = new GUIStyle(UnityEngine.GUI.skin.label).Clone(10, FontStyle.Bold).Padding(1, 0, 3, 0).Margin(3, 6, 0, 0).StretchWidth(false);
                BtnOption = DeGUI.styles.button.bBlankBorderCompact.MarginBottom(2).MarginRight(4);
                BtnPreview = EditorStyles.miniButton.Clone(Format.RichText);
                PreviewStatusLabel = EditorStyles.miniLabel.Clone().Padding(4, 0, 0, 0).Margin(0);
            }
        }
    }
}
