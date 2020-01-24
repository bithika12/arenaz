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
        public enum ERotationAxis
        {
            X,
            Y,
            Z
        }

        [SerializeField] private Transform dartObj;
        [SerializeField] private ERotationAxis rotationAxis;

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

                dartObj.DOLocalRotate(t_RotDirection, 1.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                AddPointsToArray(endPosition);
                transform.DOPath(points, .6f, PathType.CatmullRom)
                         .SetEase(Ease.Linear).SetLookAt(1, Vector3.forward)
                         .OnComplete(()=> 
                         {
                             GameManager.Instance.OnCompletionDartHit(this.gameObject);
                             dartObj.DOKill();
                         });
            }
        } 

        private Vector3 CalculateQuadraticBeizerCurve(float time,Vector3 pointThree)
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
    }
}
