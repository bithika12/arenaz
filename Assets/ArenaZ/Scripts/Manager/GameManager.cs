using System;
using ArenaZ.Behaviour;
using ArenaZ.ShootingObject;
using UnityEngine;

namespace ArenaZ.Manager
{
    [RequireComponent(typeof(TouchBehaviour))]
    public class GameManager : RedAppleSingleton<GameManager>
    {
        // Private Variables
        [SerializeField] private bool projectileMovement;
        private TouchBehaviour touchBehaviour;
        private Dart currentDart;

        // Public Variables

        private void Start()
        {
            touchBehaviour = GetComponent<TouchBehaviour>();
            touchBehaviour.OnDartMove += DartMove;
            touchBehaviour.OnDartThrow += DartThrow;
        }

        private void DartMove(Vector3 dartPostion) {
            currentDart.transform.position = dartPostion;
        }

        private void DartThrow(Vector3 hitPoint, float angle)
        {
            if (!projectileMovement)
            {
                currentDart.MoveInCurvePath(hitPoint);
            }
            else
            {
                currentDart.MoveInProjectilePath(hitPoint, angle);
            }
        }
    }
}
