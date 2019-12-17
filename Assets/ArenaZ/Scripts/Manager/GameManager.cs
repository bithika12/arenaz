using ArenaZ.Behaviour;
using ArenaZ.ShootingObject;
using UnityEngine;
using RedApple;

namespace ArenaZ.Manager
{
    [RequireComponent(typeof(TouchBehaviour))]
    public class GameManager : Singleton<GameManager>
    {
        // Private Variables
        [SerializeField] private bool projectileMove;       
        private TouchBehaviour touchBehaviour;
        private Dart currentDart;

        // Public Variables

        protected override void Awake()
        {
            currentDart = GameObject.FindGameObjectWithTag("Player").GetComponent<Dart>();
            touchBehaviour = GetComponent<TouchBehaviour>();
        }

        private void Start()
        {           
            touchBehaviour.OnDartMove += DartMove;
            touchBehaviour.OnDartThrow += DartThrow;
        }

        private void DartMove(Vector3 dartPosition)
        {
            currentDart.transform.position = dartPosition;
        }

        private void DartThrow(Vector3 hitPoint, float angle)
        {
            if (!projectileMove)
            {
                currentDart.TweenthroughPoints(hitPoint);
            }
            else
            {
                currentDart.MoveInProjectilePathWithPhysics(hitPoint, angle);
            }
        }       
    }
}
