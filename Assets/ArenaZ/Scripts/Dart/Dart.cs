using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaZ.ShootingObject.Interface;
using DG.Tweening;

namespace ArenaZ.ShootingObject
{
    public class Dart : MonoBehaviour
    {
        public Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private Vector3 BallisticVelocity(Vector3 thisPosition, float angle)
        {
            rb.useGravity = true;
            Vector3 direction = thisPosition - transform.position; // Need to change this transform position
            float height = direction.y;
            direction.y = 0;
            float dist = direction.magnitude;
            float radianValue = angle * Mathf.Deg2Rad;
            direction.y = dist * Mathf.Tan(radianValue);
            dist += height / Mathf.Tan(radianValue);
            float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * radianValue));
            return vel * direction.normalized;
        }

        public void MoveInCurvePath(Vector3 endPosition)
        {
            Vector3 middlePos = (transform.position + endPosition) / 2;
            middlePos.y = middlePos.y + 4f;
            Vector3[] pos = { transform.position, middlePos, endPosition };
            transform.DOPath(pos, 0.5f, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(1, Vector3.up);
        }

        public void MoveInProjectilePath(Vector3 endPosition, float angle)
        {
            rb.velocity = BallisticVelocity(endPosition, angle);
        }
    }
}
