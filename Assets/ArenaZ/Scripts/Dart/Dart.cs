using UnityEngine;
using DG.Tweening;
using System;
using ArenaZ.Manager;

namespace ArenaZ.ShootingObject
{
    public class Dart : MonoBehaviour
    {
        private Rigidbody dartRB;

        private float time = 0;

        private Vector3[] points = new Vector3[ConstantInteger.totalDartPointsForProjectileMove];

        private readonly float _screenMiddleOffset = 4.5f; // Y axis

        public static Action GetGameObj;

        private void Awake()
        {
            dartRB = GetComponent<Rigidbody>();
        }

        private Vector3 BallisticVelocity(Vector3 hitPosition, float angle)
        {
            Debug.Log("Moving in projectile path");
            dartRB.useGravity = true;
            Vector3 direction = hitPosition - transform.position; // Need to change this transform position
            float height = direction.y;
            direction.y = 0;
            float dist = direction.magnitude;
            float radianValue = angle * Mathf.Deg2Rad;
            direction.y = dist * Mathf.Tan(radianValue);
            dist += height / Mathf.Tan(radianValue);
            float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * radianValue));
            return vel * direction.normalized;
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
            AddPointsToArray(endPosition);
            transform.DOPath(points, .6f, PathType.CatmullRom)
                     .SetEase(Ease.Linear).SetLookAt(1, Vector3.forward)
                     .OnComplete(() => GameManager.Instance.OnCompletionDartHit());
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

        public void MoveInProjectilePathWithPhysics(Vector3 endPosition, float angle)
        {
            dartRB.velocity = BallisticVelocity(endPosition, angle);
        }
    }
}
