using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ.Behaviour
{
    public class TouchBehaviour : MonoBehaviour
    {
        //Public Fields
        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float touchDelay = 0.2f;
        [SerializeField]
        [Range(5.0f, 10.0f)]
        private float releaseDelay = 5.5f;
        [SerializeField]
        LayerMask objectMask = 5;
        // Private Fields
        private RaycastHit hit;
        private float startTouchTime, endTouchTime;
        private Vector3 FirstTouchPos, LastTouchPos;
        private Vector3 initialDartPosition = new Vector3(-5,0,0);
        private Vector3[] positions = new Vector3[4];
        private bool isDartSelected = false;
        private int counter = -1;
        private Vector3 lowestValue = Vector3.zero;

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
                else if(touch.phase==TouchPhase.Ended && isDartSelected)
                {
                    DartShoot(touch.position);
                }
            }
#endif
        }

        private void DartMove(Vector3 inputPosition)
        {
            startTouchTime = Time.deltaTime;
            if (startTouchTime > touchDelay)
            {
                ResetTimer();
            }
            if (Physics.Raycast(Camera.main.ScreenPointToRay(inputPosition), out hit)&& !isDartSelected)
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    Debug.Log("Hit Player");
                    isDartSelected = true;
                    lowestValue = GetWorldPosFromMousePos(inputPosition);
                    OnDartMove?.Invoke(FirstTouchPos);
                }
            }
            if (isDartSelected)
            {
                Vector3 nextPosition = GetWorldPosFromMousePos(inputPosition);
                FirstTouchPos = firstPositionOfY(GetWorldPosFromMousePos(inputPosition));
               // Debug.Log("First Touch Position: " + FirstTouchPos);
                OnDartMove?.Invoke(nextPosition);
            }
        }

        private Vector3 firstPositionOfY(Vector3 position)
        {
            Vector3 tempValue = Vector3.zero;
            counter++;
            positions[counter] = position;            
           // Debug.Log("Counter Value:  " + counter);
            if (counter == 0)
            {
                lowestValue = positions[counter];
            }
            else if(counter > 0)
            { 
                if (screenpositionOfY(positions[counter].y) < screenpositionOfY(positions[counter-1].y))
                {
                    tempValue = positions[counter];
                }
                else
                {
                    tempValue = positions[counter];
                }
                if(screenpositionOfY(tempValue.y) < screenpositionOfY(lowestValue.y))
                {
                    lowestValue = tempValue;
                }
            }
            if(counter==3)
            {
                counter = 0;
                positions[0] = positions[3];
            }
            return lowestValue;
        }

        private void DartShoot(Vector3 InputPosition)
        {

            LastTouchPos = GetWorldPosFromMousePos(InputPosition);
             Debug.Log("Mouse Last Touch Position:  " + LastTouchPos); 
            //Dart Only Move In Upward Direction
            if (screenpositionOfY(LastTouchPos.y) - screenpositionOfY(FirstTouchPos.y) > 3f)
            {
                float distance = Vector3.Distance(LastTouchPos, FirstTouchPos);
                endTouchTime = Time.deltaTime;
                Debug.Log("Last Mouse Final Position of Y:  " + screenpositionOfY(LastTouchPos.y));
                Debug.Log("first Mouse Final Position of Y:  " + screenpositionOfY(FirstTouchPos.y));
                Debug.Log("StartTouchTime: " + startTouchTime + "  EndTouchTime: " + endTouchTime + "  Distance: " + distance + "Touch delay: "+touchDelay + "  ReleaseDelay: " + releaseDelay);
                 Debug.Log("Substraction of start and end: " + (startTouchTime - endTouchTime) + "Vagfol of touch delay: " + distance / releaseDelay);
                if ((startTouchTime - endTouchTime) < distance / releaseDelay)
                {
                   // Debug.Log("Ya im successfully entered in if condition");
                   // RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(InputPosition), out hit, Mathf.Infinity, objectMask))
                    {
                       // Debug.Log("Gameobject er nam holo vai: " + hit.transform.name);
                        if (hit.transform.tag == "DartBoard")
                        {
                           //Debug.Log("Hit Object:  " + hit.transform.gameObject.name);
                            float shootAngle = 45; // This angle should change
                            OnDartThrow?.Invoke( hit.point, shootAngle);
                        }
                    }
                }
            }
            ResetTimer();
        }

        private Vector3 GetWorldPosFromMousePos(Vector3 position)
        {
            // Value set in z for getting proper value
            position.z = 10;
            return Camera.main.ScreenToWorldPoint(position);
        }

        private float screenpositionOfY(float position)
        {
            // Max Y value in screen is 6.4f
            float ScreenYOffset = 6.4f;
            return ScreenYOffset + position;
        }

        private void ResetTimer()
        {
            isDartSelected = false;
            startTouchTime = 0;
            endTouchTime = 0;
        }
    }
}
