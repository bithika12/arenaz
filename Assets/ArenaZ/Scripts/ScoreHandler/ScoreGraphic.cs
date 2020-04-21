using DevCommons.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

namespace ArenaZ
{
    public class ScoreGraphic : MonoBehaviour
    {
        public enum EMoveTowards
        {
            None = 0,
            User,
            Opponent,
        }

        [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField] private ScoreElement whiteCircleBg;
        [SerializeField] private ScoreElement cross;
        [SerializeField] private ScoreElement bust;
        [SerializeField] private Sprite numberCross;

        [SerializeField] private GameObject numberPrototype;
        [SerializeField] private Transform contentHolder;

        [SerializeField] private Transform userTarget;
        [SerializeField] private Transform opponentTarget;

        [SerializeField] private List<ScoreGraphicData> scoreGraphicDatas = new List<ScoreGraphicData>();

        private List<GameObject> activeScoreGraphics = new List<GameObject>();
        private Queue<GraphicScoreData> graphicScoreDatas = new Queue<GraphicScoreData>();
        private GraphicScoreData activeGraphicScoreData = null;

        public void ShowScore(int a_HitPointScore, int a_ScoreMultiplier, EMoveTowards a_MoveTowards = EMoveTowards.None, bool a_ScoreIsDenied = false, Action a_OnStepComplete = null, Action a_OnComplete = null)
        {
            graphicScoreDatas.Enqueue(new GraphicScoreData()
            {
                HitPointScore = a_HitPointScore,
                ScoreMultiplier = a_ScoreMultiplier,
                ScoreMoveTowards = a_MoveTowards,
                ScoreIsDenied = a_ScoreIsDenied,
                OnStepComplete = a_OnStepComplete,
                OnComplete = a_OnComplete,
            });
            if (activeGraphicScoreData == null)
                peekFromQueue();
        }

        private void peekFromQueue()
        {
            activeGraphicScoreData = graphicScoreDatas.Peek();
            showScoreOnDisplay(activeGraphicScoreData);
        }

        private void showScoreOnDisplay(GraphicScoreData a_GraphicScoreData)
        {
            //whiteCircleBg.FadeInOutAnimation();
            if (a_GraphicScoreData.HitPointScore > 0)
            {
                if (a_GraphicScoreData.ScoreMultiplier > 1)
                {
                    horizontalLayoutGroup.spacing = -50;
                    instantiateHelper(a_GraphicScoreData.HitPointScore, a_GraphicScoreData, 2);
                    instantiateSprites(numberCross, a_GraphicScoreData, 2.5f);
                    instantiateHelper(a_GraphicScoreData.ScoreMultiplier, a_GraphicScoreData, 2.5f);
                }
                else
                {
                    horizontalLayoutGroup.spacing = -100;
                    instantiateHelper(a_GraphicScoreData.HitPointScore, a_GraphicScoreData, 1);
                }
            }
            else
            {
                horizontalLayoutGroup.spacing = -100;
                instantiateHelper(0, a_GraphicScoreData, 1);
            }
            if (a_GraphicScoreData.ScoreIsDenied)
                scoreDenied();

            AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.NumberDisplay).Clip, oneShot = true, volume = SettingData.SFXVolume });
        }

        private void instantiateHelper(int a_Value, GraphicScoreData a_GraphicScoreData, float a_SizeFactor)
        {
            List<int> t_Digits = getDigits(a_Value);
            for (int i = 0; i < t_Digits.Count; i++)
            {
                ScoreGraphicData t_ScoreGraphicData = scoreGraphicDatas.Where(x => x.GraphicValue == t_Digits[i]).First();
                Debug.Log("Score Sprite Value: " + t_ScoreGraphicData.GraphicValue);
                instantiateSprites(t_ScoreGraphicData.GraphicSprite, a_GraphicScoreData, a_SizeFactor);
            }
        }

        private void instantiateSprites(Sprite a_GraphicSprite, GraphicScoreData a_GraphicScoreData, float a_SizeFactor)
        {
            activeGraphicScoreData.CallbackCount++;
            GameObject t_Go = Instantiate(numberPrototype, contentHolder);
            t_Go.GetComponent<Image>().sprite = a_GraphicSprite;
            t_Go.SetActive(true);
            t_Go.GetComponent<RectTransform>().sizeDelta /= a_SizeFactor;

            ScoreElement t_ScoreElement = t_Go.GetComponent<ScoreElement>();
            if (t_ScoreElement != null)
            {
                if (a_GraphicScoreData.ScoreMoveTowards == EMoveTowards.None)
                    t_ScoreElement.FadeInOutAnimation(true, a_GraphicScoreData.OnComplete, onCompleteAnimationInternalCallback);
                else if (a_GraphicScoreData.ScoreMoveTowards == EMoveTowards.User)
                    t_ScoreElement.MoveFadeInOutAnimation(userTarget, a_GraphicScoreData.OnStepComplete, a_GraphicScoreData.OnComplete, onCompleteAnimationInternalCallback);
                else if (a_GraphicScoreData.ScoreMoveTowards == EMoveTowards.Opponent)
                    t_ScoreElement.MoveFadeInOutAnimation(opponentTarget, a_GraphicScoreData.OnStepComplete, a_GraphicScoreData.OnComplete, onCompleteAnimationInternalCallback);
            }

            activeScoreGraphics.Add(t_Go);
        }

        private void scoreDenied()
        {
            cross.FadeInOutAnimation(false, () =>
            {
                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Lost).Clip, oneShot = true, volume = SettingData.SFXVolume });
                bust.FadeInOutAnimation();
            });
        }

        private void onCompleteAnimationInternalCallback()
        {
            activeGraphicScoreData.CallbackCount--;
            if (activeGraphicScoreData.CallbackCount == 0)
            {
                graphicScoreDatas.Dequeue();
                if (graphicScoreDatas.Any())
                    peekFromQueue();
                else
                    activeGraphicScoreData = null;
            }
        }

        private List<int> getDigits(int a_Value)
        {
            string t_Numbers = a_Value.ToString();
            List<int> t_IntArray = new List<int>();
            for (int i = 0; i < t_Numbers.Length; i++)
            {
                t_IntArray.Add(int.Parse(t_Numbers[i].ToString()));
            }
            return t_IntArray;
        }
    }

    [System.Serializable]
    public class ScoreGraphicData
    {
        public int GraphicValue = 0;
        public Sprite GraphicSprite;
    }

    [System.Serializable]
    public class GraphicScoreData
    {
        public int HitPointScore = 0;
        public int ScoreMultiplier = 0;
        public ScoreGraphic.EMoveTowards ScoreMoveTowards = ScoreGraphic.EMoveTowards.None;
        public int CallbackCount = 0;
        public bool ScoreIsDenied = false;

        public Action OnStepComplete = null;
        public Action OnComplete = null;
    }
}