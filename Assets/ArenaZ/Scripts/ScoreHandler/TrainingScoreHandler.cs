using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaZ.Manager;
using DevCommons.Utility;

namespace ArenaZ
{
    public class TrainingScoreHandler : MonoBehaviour
    {
        [SerializeField] private int targetScore = 199;
        private int remainingScore = 0;
        private int activeRoundScore = 0;
        private int activeHitCount = 0;

        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private ScoreGraphic scoreGraphic;

        public void Initialize()
        {
            remainingScore = targetScore;
            activeRoundScore = 0;
            activeHitCount = 0;
            scoreHandler.Initialize(targetScore);
        }

        public void AddHitPoint(int a_HitPoint, int a_Multiplier)
        {
            bool t_IsBust = false;
            int t_TotalPoint = a_HitPoint;
            if (a_Multiplier > 1)
                t_TotalPoint *= a_Multiplier;

            if ((remainingScore - t_TotalPoint) >= 0)
            {
                t_IsBust = false;
                onNonBust(t_TotalPoint, a_HitPoint, a_Multiplier);
                if ((remainingScore - t_TotalPoint) == 0)
                {
                    GameManager.Instance.OnLeaveTraining();
                }
            }
            else
            {
                t_IsBust = true;
                onBust();
            }

            scoreHandler.SetRoundScoreData(GameManager.Player.Self, activeRoundScore);
            scoreHandler.SetRemainingScoreData(GameManager.Player.Self, remainingScore);

            updateHitCount(t_IsBust);
        }

        private void onNonBust(int a_TotalPoint, int a_HitPoint, int a_Multiplier)
        {
            activeRoundScore += a_TotalPoint;
            remainingScore -= a_TotalPoint;

            scoreGraphic.ShowScore(a_HitPoint, a_Multiplier);
            if (a_Multiplier > 1)
                scoreGraphic.ShowScore(a_TotalPoint, 0);
        }

        private void onBust()
        {
            scoreGraphic.ShowScore(activeRoundScore, 0, ScoreGraphic.EMoveTowards.None, true);
            activeRoundScore = 0;
            activeHitCount = 0;
        }

        private void updateHitCount(bool a_IsBust)
        {
            if (!a_IsBust)
            {
                activeHitCount++;
                if (activeHitCount == 3)
                {
                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.AudienceCheering).Clip, oneShot = true, volume = SettingData.SFXVolume });
                    scoreGraphic.ShowScore(activeRoundScore, 0, ScoreGraphic.EMoveTowards.User, false, () => scoreHandler.UpdateScoreText(GameManager.Player.Self));

                    activeRoundScore = 0;
                    activeHitCount = 0;
                }
            }
        }
    }
}