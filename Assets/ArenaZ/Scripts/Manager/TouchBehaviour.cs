using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ.Behaviour
{
    public class TouchBehaviour : MonoBehaviour
    {
        [SerializeField] private bool dragBallToShoot;
        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float TouchDelay;
        [SerializeField]
        [Range(5.0f, 10.0f)]
        private float releaseDelay;
        [SerializeField]
        LayerMask objectMask = 5;

        private float startTouchTime;
        private float endTouchTime;
        private Vector3 mouseFirstTouchPos;
        private Vector3 mouseLastTouchPos;
        private bool isDartSelected = false;

        public Action<Vector3> OnDartMove; 
        public Action<Vector3, float> OnDartThrow;

        private void Update()
        {
            TouchToShoot();
        }

        private void TouchToShoot()
        {
            if (Input.GetMouseButton(0))
            {
                DartMove();
            }
            if (Input.GetMouseButtonUp(0) && isDartSelected)
            {
                DartShoot();
            }
        }

        private void DartMove()
        {
            startTouchTime += Time.deltaTime;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (startTouchTime > TouchDelay)
            {
                ResetTimer();
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !isDartSelected)
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    isDartSelected = true;
                    mouseFirstTouchPos = GetWorldPosFromMousePos(Input.mousePosition);
                    OnDartMove?.Invoke(mouseFirstTouchPos);
                }
            }

            if (isDartSelected)
            {
                Vector3 nextPosition = GetWorldPosFromMousePos(Input.mousePosition);
                OnDartMove?.Invoke(nextPosition);
            }
        }

        private void DartShoot()
        {
            endTouchTime += Time.deltaTime;
            mouseLastTouchPos = GetWorldPosFromMousePos(Input.mousePosition);
            float distance = Vector3.Distance(mouseLastTouchPos, mouseFirstTouchPos);
            if (FinalPositionOfY(mouseLastTouchPos.y) - FinalPositionOfY(mouseFirstTouchPos.y) > 0)
            {
                if ((startTouchTime - endTouchTime) < distance / releaseDelay)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, objectMask))
                    {
                        var shootAngle = 45; // This angle should change
                        OnDartThrow?.Invoke(hit.point, shootAngle);
                    }
                }
            }
            ResetTimer();
        }

        private Vector3 GetWorldPosFromMousePos(Vector3 pos)
        {
            // Value set in z for getting proper value
            pos.z = 10;
            return Camera.main.ScreenToWorldPoint(pos);
        }

        private float FinalPositionOfY(float position)
        {
            return YmaxPos() + position;
        }

        private float YmaxPos()
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelRect.yMax, 10)).y;
        }

        private void ResetTimer()
        {
            isDartSelected = false;
            startTouchTime = 0;
            endTouchTime = 0;
        }

    }
}
