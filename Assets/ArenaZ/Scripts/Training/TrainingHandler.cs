using System.Collections;
using System.Collections.Generic;
using ArenaZ.Behaviour;
using ArenaZ.ShootingObject;
using DevCommons.Utility;
using UnityEngine;

namespace ArenaZ
{
    public class TrainingHandler : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;

        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private ScoreGraphic scoreGraphic;

        private AudioPlayer mainMenuBGAudioPlayer;
        private AudioPlayer gameplayBGAudioPlayer;

        private GameObject userDart;
        private Dart currentDart;
        private TouchBehaviour touchBehaviour;

        [Header("Hit Particle")]
        [SerializeField] private GameObject hitParticleParent;
        [SerializeField] private ParticleSystem hitParticleSystem;

        [Header("Others")]
        [SerializeField] private float dartDragForce = 0.0f;

        public void StrartTraining()
        {

        }

        public void StopTraining()
        {

        }
    }
}