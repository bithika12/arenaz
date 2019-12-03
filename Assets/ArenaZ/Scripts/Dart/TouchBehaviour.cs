using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ.Behaviour
{
    public class TouchBehaviour : MonoBehaviour
    {
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
        private Vector3 FirstTouchPos;
        private Vector3 LastTouchPos;
        private bool isDartSelected = false;

        public Action<Vector3> OnDartMove;
        public Action<Vector3, float> OnDartThrow;

        private void Update()
        {
            TouchToShoot();
        }

        private void TouchToShoot()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                DartMove(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0) && isDartSelected)
            {
                DartShoot(Input.mousePosition);
            }
#elif UNITY_ANDROID
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase==TouchPhase.Stationary || touch.phase==TouchPhase.Moved)
                {
                    DartMove(touch.position);
                }
                else if(touch.phase==TouchPhase.Moved && isDartSelected)
                {
                    DartShoot(touch.position);
                }
            }
#endif
        }

        private void DartMove(Vector3 InputPosition)
        {
            startTouchTime += Time.deltaTime;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(InputPosition);
            if (startTouchTime > TouchDelay)
            {
                ResetTimer();
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)&& !isDartSelected)
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    isDartSelected = true;
                    FirstTouchPos = GetWorldPosFromMousePos(InputPosition);
                    Debug.Log("Mouse First Touch Pos:   " + FirstTouchPos);
                    OnDartMove?.Invoke(FirstTouchPos);
                }
            }
            if (isDartSelected)
            {
                Vector3 nextPosition = GetWorldPosFromMousePos(InputPosition);
                OnDartMove?.Invoke(nextPosition);
            }
        }

        private void DartShoot(Vector3 InputPosition)
        {
            endTouchTime += Time.deltaTime;
            LastTouchPos = GetWorldPosFromMousePos(InputPosition);
            // Debug.Log("Mouse Last Touch Position:  " + mouseLastTouchPos);
            float distance = Vector3.Distance(LastTouchPos, FirstTouchPos);
           // Debug.Log("Last Mouse Final Position of Y:  " + FinalPositionOfY(mouseLastTouchPos.y));
           // Debug.Log("First Mouse Final Position of Y:  " + FinalPositionOfY(mouseFirstTouchPos.y));
            if (FinalPositionOfY(LastTouchPos.y) - FinalPositionOfY(FirstTouchPos.y) > 0)
            {
               // Debug.Log("StartTouchTime: " + startTouchTime + "  EndTouchTime: " + endTouchTime + "  Distance: " + distance + "  ReleaseDelay: " + releaseDelay);
              //  Debug.Log("Substraction of start and end: " + (startTouchTime - endTouchTime) + "Vagfol of touch delay: " + distance / releaseDelay);
                if ((startTouchTime - endTouchTime) < distance / releaseDelay)
                {
                   // Debug.Log("Ya im successfully entered in if condition");
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(InputPosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, objectMask))
                    {
                       // Debug.Log("Gameobject er nam holo vai: " + hit.transform.name);
                        if (hit.transform.tag == "DartBoard")
                        {
                          //  Debug.Log("Hit Object:  " + hit.transform.gameObject.name);
                            var shootAngle = 45; // This angle should change
                            OnDartThrow?.Invoke( hit.point, shootAngle);
                        }
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
           // Debug.Log("Camera Pixelrect Values:  "+Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelRect.yMax, 10)).y);
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
