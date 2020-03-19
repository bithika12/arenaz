using DevCommons.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

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
        [SerializeField] private GameObject bg;
        [SerializeField] private GameObject cross;
        [SerializeField] private GameObject bust;
        [SerializeField] private Sprite numberCross;

        [SerializeField] private GameObject numberPrototype;
        [SerializeField] private Transform contentHolder;

        [SerializeField] private GameObject userTarget;
        [SerializeField] private GameObject opponentTarget;

        [SerializeField] private List<ScoreGraphicData> scoreGraphicDatas = new List<ScoreGraphicData>();

        private List<GameObject> activeScoreGraphics = new List<GameObject>();

        public void ShowScore(int a_HitPointScore, int a_ScoreMultiplier, EMoveTowards a_MoveTowards)
        {
            Debug.Log($"Score HitPointScore: {a_HitPointScore}, ScoreMultiplier: {a_ScoreMultiplier}");
            bg.SetActive(true);

            if (a_HitPointScore > 0)
            {
                if (a_ScoreMultiplier > 1)
                {
                    instantiateHelper(a_HitPointScore, false, a_MoveTowards);
                    instantiateSprites(numberCross, false, a_MoveTowards);
                    instantiateHelper(a_ScoreMultiplier, true, a_MoveTowards);
                }
                else
                {
                    instantiateHelper(a_HitPointScore, false, a_MoveTowards);
                }
            }
            else
            {
                instantiateHelper(0, false, a_MoveTowards);
            }

            updateRect(contentHolder, (activeScoreGraphics.Count * 300.0f), 300.0f);
            AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.NumberDisplay).Clip, oneShot = true });
            StartCoroutine(ClearScoreboard(0.75f));
        }
        
        public void ScoreDenied()
        {
            AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Lost).Clip, oneShot = true });
            StartCoroutine(showCrossAndBust());
        }

        private IEnumerator showCrossAndBust()
        {
            cross.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            cross.SetActive(false);
            StartCoroutine(ClearScoreboard());
            bust.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            bust.SetActive(false);
        }

        private void instantiateHelper(int a_Value, bool a_UpdateRect, EMoveTowards a_MoveTowards)
        {
            List<int> t_Digits = getDigits(a_Value);
            for (int i = 0; i < t_Digits.Count; i++)
            {
                ScoreGraphicData t_ScoreGraphicData = scoreGraphicDatas.Where(x => x.GraphicValue == t_Digits[i]).FirstOrDefault();
                Debug.Log("Score Sprite Value: " + t_ScoreGraphicData.GraphicValue);
                instantiateSprites(t_ScoreGraphicData.GraphicSprite, a_UpdateRect, a_MoveTowards);
            }
        }

        private void instantiateSprites(Sprite a_GraphicSprite, bool a_UpdateRect, EMoveTowards a_MoveTowards)
        {
            GameObject t_Go = Instantiate(numberPrototype, contentHolder);
            t_Go.name = a_MoveTowards.ToString();
            t_Go.GetComponent<Image>().sprite = a_GraphicSprite;
            t_Go.SetActive(true);

            if (a_UpdateRect)
            {
                updateRect(t_Go.transform, 260.0f, 260.0f);
            }
            t_Go.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
            activeScoreGraphics.Add(t_Go);
        }

        private void updateRect(Transform a_ObjTransform, float a_Width, float a_Height)
        {
            Debug.Log("UpdateRect");
            a_ObjTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(a_Width, a_Height);
        }

        public void HideCrossAndBust()
        {
            StopAllCoroutines();
            cross.SetActive(false);
            bust.SetActive(false);
        }

        public IEnumerator ClearScoreboard(float a_Delay = 0.0f)
        {
            yield return new WaitForSeconds(a_Delay);
            if (activeScoreGraphics.Any())
            {
                for (int i = activeScoreGraphics.Count - 1; i >= 0; i--)
                {
                    EMoveTowards t_MoveTowards = EMoveTowards.None;
                    Enum.TryParse(activeScoreGraphics[i].name, out t_MoveTowards);

                    ScoreElement t_ScoreElement = activeScoreGraphics[i].GetComponent<ScoreElement>();
                    if (t_ScoreElement != null)
                    {
                        if (t_MoveTowards == EMoveTowards.None)
                            t_ScoreElement.PlayCloseAnimation(onCompleteCloseAnimation);
                        else if (t_MoveTowards == EMoveTowards.User)
                            t_ScoreElement.PlayMoveTowardsAnimation(userTarget, EMoveTowards.User, onCompleteCloseAnimation);
                        else if (t_MoveTowards == EMoveTowards.Opponent)
                            t_ScoreElement.PlayMoveTowardsAnimation(opponentTarget, EMoveTowards.Opponent, onCompleteCloseAnimation);
                    }
                }
            }
            cross.SetActive(false);
            bg.SetActive(false);
        }

        private void onCompleteCloseAnimation(GameObject t_Obj)
        {
            Destroy(t_Obj);
            activeScoreGraphics.Remove(t_Obj);
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
}