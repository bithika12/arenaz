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

        private int selfRemainingScore = 0;
        private int opponentRemainingScore = 0;

        public void Initialize(int a_InitialRemainingScore)
        {
            selfRemainingScore = a_InitialRemainingScore;
            opponentRemainingScore = a_InitialRemainingScore;

            selfRemainingScoreText.text = a_InitialRemainingScore.ToString();
            opponentRemainingScoreText.text = a_InitialRemainingScore.ToString();
        }

        public void UpdateScoreText(GameManager.Player a_PlayerType, int a_RemainingScore, bool a_Animate)
        {
            if (a_Animate)
                animatedUpdateScoreText(a_PlayerType, a_RemainingScore);
            else
                updateScoreTextImmediately(a_PlayerType, a_RemainingScore);
        }

        private void updateScoreTextImmediately(GameManager.Player a_PlayerType, int a_RemainingScore)
        {
            if (a_PlayerType == GameManager.Player.Self)
            {
                selfRemainingScoreText.text = a_RemainingScore.ToString();
                selfRemainingScore = a_RemainingScore;
            }
            else if (a_PlayerType == GameManager.Player.Opponent)
            {
                opponentRemainingScoreText.text = a_RemainingScore.ToString();
                opponentRemainingScore = a_RemainingScore;
            }
        }

        private void animatedUpdateScoreText(GameManager.Player a_PlayerType, int a_RemainingScore)
        {
            // Tween a float called myFloat to 52 in 1 second
            // Before myFloat == 40;
            // DOTween.To(() => myFloat, x => myFloat = x, 52, 1);
            // After myFloat == 52;

            if (a_PlayerType == GameManager.Player.Self)
                DOTween.To(() => selfRemainingScore, x => selfRemainingScoreText.text = x.ToString(), a_RemainingScore, 1.0f).OnComplete(() => selfRemainingScore = a_RemainingScore);
            else if (a_PlayerType == GameManager.Player.Opponent)
                DOTween.To(() => opponentRemainingScore, x => opponentRemainingScoreText.text = x.ToString(), a_RemainingScore, 1.0f).OnComplete(() => opponentRemainingScore = a_RemainingScore);
        }
    }
}