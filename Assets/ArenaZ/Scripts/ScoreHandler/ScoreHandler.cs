using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaZ.Manager;
using DG.Tweening;
using TMPro;

namespace ArenaZ
{
    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI selfRemainingScoreText;
        [SerializeField] private TextMeshProUGUI opponentRemainingScoreText;

        private SoreData selfScoreData;
        private SoreData opponentScoreData;

        public SoreData SelfScoreData { get => selfScoreData; }
        public SoreData OpponentScoreData { get => opponentScoreData; }

        public void Initialize(int a_InitialRemainingScore)
        {
            selfScoreData = new SoreData() { PlayerType = GameManager.Player.Self, CurrentRemainingScore = a_InitialRemainingScore, PreviousRemainingScore = a_InitialRemainingScore, ActiveRoundScore = 0 };
            opponentScoreData = new SoreData() { PlayerType = GameManager.Player.Opponent, CurrentRemainingScore = a_InitialRemainingScore, PreviousRemainingScore = a_InitialRemainingScore, ActiveRoundScore = 0 };

            selfRemainingScoreText.text = a_InitialRemainingScore.ToString();
            opponentRemainingScoreText.text = a_InitialRemainingScore.ToString();
        }

        public void SetRemainingScoreData(GameManager.Player a_PlayerType, int a_RemainingScore)
        {
            if (a_PlayerType == GameManager.Player.Self)
                selfScoreData.CurrentRemainingScore = a_RemainingScore;
            else if (a_PlayerType == GameManager.Player.Opponent)
                opponentScoreData.CurrentRemainingScore = a_RemainingScore;
        }

        public void SetRoundScoreData(GameManager.Player a_PlayerType, int a_ActiveRoundScore)
        {
            if (a_PlayerType == GameManager.Player.Self)
                selfScoreData.ActiveRoundScore = a_ActiveRoundScore;
            else if (a_PlayerType == GameManager.Player.Opponent)
                opponentScoreData.ActiveRoundScore = a_ActiveRoundScore;
        }

        public void UpdateScoreText(GameManager.Player a_PlayerType)
        {
            if (a_PlayerType == GameManager.Player.Self)
                animateScoreText(selfRemainingScoreText, selfScoreData);
            else if (a_PlayerType == GameManager.Player.Opponent)
                animateScoreText(opponentRemainingScoreText, opponentScoreData);
        }

        public void UpdateScoreImmediately(GameManager.Player a_PlayerType, int a_RemainingScore, bool a_AllTurnFinished)
        {
            if (a_PlayerType == GameManager.Player.Self)
            {
                selfRemainingScoreText.text = a_RemainingScore.ToString();
                selfScoreData.CurrentRemainingScore = a_RemainingScore;

                if (a_AllTurnFinished)
                    selfScoreData.PreviousRemainingScore = a_RemainingScore;
            }
            else if (a_PlayerType == GameManager.Player.Opponent)
            {
                opponentRemainingScoreText.text = a_RemainingScore.ToString();
                opponentScoreData.CurrentRemainingScore = a_RemainingScore;

                if (a_AllTurnFinished)
                    opponentScoreData.PreviousRemainingScore = a_RemainingScore;
            }
        }

        private void animateScoreText(TextMeshProUGUI a_TextMeshPro, SoreData a_ScoreData)
        {
            // Tween a float called myFloat to 52 in 1 second
            //DOTween.To(() => myFloat, x => myFloat = x, 52, 1);

            DOTween.To(() => a_ScoreData.PreviousRemainingScore, x => a_TextMeshPro.text = x.ToString(), a_ScoreData.CurrentRemainingScore, 1.0f)
                .OnComplete(() => a_ScoreData.PreviousRemainingScore = a_ScoreData.CurrentRemainingScore);
        }
    }

    [System.Serializable]
    public class SoreData
    {
        public GameManager.Player PlayerType;
        public int CurrentRemainingScore = 0;
        public int PreviousRemainingScore = 0;
        public int ActiveRoundScore = 0;
    }
}