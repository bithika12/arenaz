using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ
{
    public class GlowAnimation : MonoBehaviour
    {
        [SerializeField] private bool playOnAwake = false;
        [SerializeField] private float maxIntensity = 1.0f;
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private Material bodyMaterial;
        private Tween tween;

        private void OnEnable()
        {
            if (playOnAwake)
            {
                DoFlash(maxIntensity, duration);
            }
        }

        private void OnDisable()
        {
            StopFlash();
        }

        public void DoFlash(float a_MaxIntensity = 1.0f, float a_Duration = 1.0f)
        {
            StopFlash();
            float t_Intensity = 0;
            tween = DOTween.To(() => t_Intensity, x =>
            {
                t_Intensity = x;
                SetMaterialEmissionIntensity(bodyMaterial, t_Intensity);
            }, a_MaxIntensity, a_Duration).SetLoops(-1, LoopType.Yoyo);
        }

        public void StopFlash()
        {
            if (tween != null)
            {
                tween.Kill();
                tween = null;
                SetMaterialEmissionIntensity(bodyMaterial, 0.0f);
            }
        }

        private void SetMaterialEmissionIntensity(Material mat, float intensity)
        {
            // get the material at this path
            Color color = mat.GetColor("_Color");

            // for some reason, the desired intensity value (set in the UI slider) needs to be modified slightly for proper internal consumption
            float adjustedIntensity = intensity - (0.4169F);

            // redefine the color with intensity factored in - this should result in the UI slider matching the desired value
            color *= Mathf.Pow(2.0f, adjustedIntensity);
            mat.SetColor("_EmissionColor", color);
        }
    }
}
