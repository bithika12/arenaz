using System;
using System.Collections.Generic;
using UnityEngine;
using ArenaZ.Manager;

namespace ArenaZ.Behaviour
{
    public class TouchBehaviour : MonoBehaviour
    {
        //Public Fields
        [SerializeField]
        [Range(1f, 10f)]
        private float touchDelay = 5.0f;
        [SerializeField]
        [Range(5.0f, 10.0f)]
        private float releaseDelay = 5.5f;
        [SerializeField]
        [Range(5.0f, 10.0f)]
        private float initialRotationForDart = 5;
        [SerializeField]
        LayerMask objectMask = 5;

        // Private Fields
        private GameObject dartHitObj;
        private Vector3 dartHitPos;
        private RaycastHit hit;
        private Vector3 firstTouchPos;
        private Vector3[] inputPositions = new Vector3[ConstantInteger.inputPosNo];
        private bool isDartSelected = false;
        private bool isShooted;
        private int firstpositioncounter = -1;
        private Vector3 lowestValue = Vector3.zero;
        private LinkedList<Vector3> lastPositions = new LinkedList<Vector3>();

        public GameObject DartHitGameObj { get { return dartHitObj; } }
        public Vector3 LastTouchPosition { get { return dartHitPos; } }

        public bool IsShooted { get => isShooted; set => isShooted = value; }

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
                shootIfNotStayedInSamePosForLong(Input.mousePosition);
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
                    shootIfNotStayedInSamePosForLong(touch.position);
                }
            }
#endif
        }

        private void DartMove(Vector3 inputPosition)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(inputPosition), out hit)&& !isDartSelected && !isShooted)
            {
                if (hit.transform.tag == GameobjectTag.Player.ToString())
                {                 
                    isDartSelected = true;
                    Vector3 dartRotation = Vector3.zero;
                    dartRotation.x = initialRotationForDart;
                   // hit.transform.eulerAngles = dartRotation;
                   // lowestValue = GetWorldPosFromMousePos(inputPosition);
                    OnDartMove?.Invoke(firstTouchPos);
                }
            }
            if (isDartSelected && !isShooted)
            {
                Vector3 nextPosition = GetWorldPosFromMousePos(inputPosition);
                // FirstTouchPos = firstPositionOfY(nextPosition);
                //Debug.Log("Next position: " + nextPosition);
                storeLastFewInputpositions(nextPosition);
                OnDartMove?.Invoke(nextPosition);
            }
        }

        private void storeLastFewInputpositions(Vector3 position)
        {
            lastPositions.AddFirst(position);
            //Debug.Log("linklist count:  " + lastPositions.Count);
            if(lastPositions.Count > ConstantInteger.fewPosNo)
            {
                lastPositions.RemoveLast();
            }
        }

        private void shootIfNotStayedInSamePosForLong(Vector3 lastPosition)
        {
            if (lastPositions.Count >= ConstantInteger.fewPosNo)
            {
                if (screenpositionOfY(lastPositions.First.Value.y) - screenpositionOfY(lastPositions.Last.Value.y) > 1f)
                {
                    DartShoot(lastPosition);
                }
            }
        }

        private Vector3 firstPositionOfY(Vector3 position)
        {
            Vector3 tempValue = Vector3.zero;
            firstpositioncounter++;
            inputPositions[firstpositioncounter] = position;            
            if (firstpositioncounter == 0)
            {
                lowestValue = inputPositions[firstpositioncounter];
            }
            else if(firstpositioncounter > 0)
            { 
                if (screenpositionOfY(inputPositions[firstpositioncounter].y) < screenpositionOfY(inputPositions[firstpositioncounter-1].y))
                {
                    tempValue = inputPositions[firstpositioncounter];
                }
                else
                {
                    tempValue = inputPositions[firstpositioncounter];
                }
                if(screenpositionOfY(tempValue.y) < screenpositionOfY(lowestValue.y))
                {
                    lowestValue = tempValue;
                }
            }
            if(firstpositioncounter==inputPositions.Length-1)
            {
                firstpositioncounter = 0;
                inputPositions[0] = inputPositions[inputPositions.Length-1];
            }
            return lowestValue;
        }

        private void DartShoot(Vector3 lastPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(lastPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, objectMask))
            {
                Debug.Log("Dart Shoot");
                if (hit.transform.tag == GameobjectTag.DartBoard.ToString())
                {
                    Debug.Log("Dart Hit GameObject Name: " + hit.transform.gameObject.name);
                    OnDartThrow?.Invoke(hit.point, ConstantInteger.shootingAngle);
                    dartHitObj = hit.transform.gameObject as GameObject;
                    dartHitPos = hit.point;
                }
            }
            ResetTouch();
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

        private void ResetTouch()
        {
            isDartSelected = false;
        }
    }
}
