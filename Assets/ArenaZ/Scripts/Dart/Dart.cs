using UnityEngine;
using DG.Tweening;
using System;
using ArenaZ.Manager;
using System.Collections;
using DG.Tweening;

namespace ArenaZ.ShootingObject
{
    public class Dart : MonoBehaviour
    {
        public enum EFlashState
        {
            Idle,
            Flashing,
        }

        public enum ERotationAxis
        {
            X,
            Y,
            Z
        }

        [SerializeField] private Transform dartObj;
        [SerializeField] private ERotationAxis rotationAxis;

        [SerializeField] private Material bodyMaterial;
        [SerializeField] private ParticleSystem rippleParticle;

        [SerializeField] private AudioClip dartThrownHit;
        [SerializeField] private AudioClip dartThrownMiss;

        private EFlashState flashState = EFlashState.Idle;
        private Rigidbody dartRB;
        private float time = 0;
        private Vector3[] points = new Vector3[ConstantInteger.totalDartPointsForProjectileMove];
        private readonly float _screenMiddleOffset = 4.5f; // Y axis

        public static Action GetGameObj;

        private void Awake()
        {
            dartRB = GetComponent<Rigidbody>();
        }

        private void AddPointsToArray(Vector3 point)
        {
            int pointsNum = points.Length;
            for (int i = 1; i < pointsNum + 1; i++)
            {
                time = i / (float)pointsNum;
                points[i - 1] = CalculateQuadraticBeizerCurve(time, point);
            }
        }

        public void TweenthroughPoints(Vector3 endPosition)
        {
            if (endPosition != Vector3.zero)
            {
                Vector3 t_RotDirection = new Vector3(0.0f, 0.0f, 0.0f);
                if (rotationAxis == ERotationAxis.X)
                    t_RotDirection.x = -360.0f;
                else if (rotationAxis == ERotationAxis.Z)
                    t_RotDirection.z = -360.0f;

                dartObj.DOLocalRotate(t_RotDirection, 0.7f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                AddPointsToArray(endPosition);
                transform.DOPath(points, 0.6f, PathType.CatmullRom)
                         .SetEase(Ease.Linear).SetLookAt(1, Vector3.forward)
                         .OnComplete(() =>
                         {
                             GameManager.Instance.OnCompletionDartHit(this.gameObject);
                             dartObj.DOKill();
                         });
            }
        }

        private Vector3 CalculateQuadraticBeizerCurve(float time, Vector3 pointThree)
        {
            Vector3 pointTwo = (transform.position + pointThree) / 2;
            pointTwo.y += _screenMiddleOffset;
            // B(t) =i (1 - t)2P0 + 2(1 - t)tP1 + t2P2 , 0 < t < 1
            float initialV = 1 - time;
            float squareOfTime = time * time;
            float squareOfInitialV = initialV * initialV;
            Vector3 calculation = (squareOfInitialV * transform.position) + (2 * initialV * time * pointTwo) + (squareOfTime * pointThree);
            return calculation;
        }

        Tween tween;
        public void DoFlash()
        {
            InvokeRepeating(nameof(playParticle), 1.5f, 1.5f);
            //float t_Intensity = 0;
            //tween = DOTween.To(() => t_Intensity, x => 
            //{ 
            //    t_Intensity = x;
            //    SetMaterialEmissionIntensity(bodyMaterial, t_Intensity);
            //}, 1.0f, 1.0f).SetLoops(-1, LoopType.Yoyo);
        }

        public void StopFlash()
        {
            rippleParticle.Stop();
            CancelInvoke(nameof(playParticle));
            //if (tween != null)
            //{
            //    tween.Kill();
            //    SetMaterialEmissionIntensity(bodyMaterial, 0.0f);
            //}
        }

        private void playParticle()
        {
            rippleParticle.Stop();
            rippleParticle.Play();
        }

        public void ChangeColor(Color color)
        {

        }

        private void SetMaterialEmissionIntensity(Material mat, float intensity)
        {
            // get the material at this path
            Color color = mat.GetColor("_Color");

            // for some reason, the desired intensity value (set in the UI slider) needs to be modified slightly for proper internal consumption
            float adjustedIntensity = intensity - (0.4169F);

            // redefine the color with intensity factored in - this should result in the UI slider matching the desired value
            color *= Mathf.Pow(2.0F, adjustedIntensity);
            mat.SetColor("_EmissionColor", color);
        }
    } 
}
