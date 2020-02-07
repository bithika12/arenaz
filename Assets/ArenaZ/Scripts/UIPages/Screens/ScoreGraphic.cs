using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArenaZ
{
    public class ScoreGraphic : MonoBehaviour
    {
        [SerializeField] private GameObject bg;
        [SerializeField] private GameObject cross;
        [SerializeField] private GameObject bust;
        [SerializeField] private Sprite numberCross;

        [SerializeField] private GameObject numberPrototype;
        [SerializeField] private Transform contentHolder;

        [SerializeField] private List<ScoreGraphicData> scoreGraphicDatas = new List<ScoreGraphicData>();

        private List<GameObject> activeScoreGraphics = new List<GameObject>();

        public void ShowScore(int a_HitPointScore, int a_ScoreMultiplier)
        {
            bg.SetActive(true);

            if (a_HitPointScore > 0)
            {
                if (a_ScoreMultiplier > 0)
                {
                    InstantiateHelper(a_HitPointScore, false);
                    Instantiate(numberCross);
                    InstantiateHelper(a_ScoreMultiplier, true);
                }
                else
                {
                    InstantiateHelper(a_HitPointScore, false);
                }
            }
            else
            {
                InstantiateHelper(0, false);
            }

            UpdateRect(contentHolder, (activeScoreGraphics.Count * 3.5f), 3.5f);
            StartCoroutine(ClearScoreboard(1.5f));
        }

        public void ScoreDenied()
        {
            StartCoroutine(ShowCrossAndBust());
        }

        private IEnumerator ShowCrossAndBust()
        {
            cross.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            cross.SetActive(false);
            bust.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            bust.SetActive(false);
        }

        private void InstantiateHelper(int a_Value, bool a_UpdateRect)
        {
            List<int> t_Digits = GetDigits(a_Value);
            for (int i = 0; i < t_Digits.Count; i++)
            {
                Debug.Log($"Score Digit: {t_Digits[i]}");
                ScoreGraphicData t_ScoreGraphicData = scoreGraphicDatas.Where(x => x.GraphicValue == t_Digits[i]).FirstOrDefault();
                Debug.Log($"Score Fetched Digit: {t_ScoreGraphicData.GraphicValue}");
                Instantiate(t_ScoreGraphicData.GraphicSprite, a_UpdateRect);
            }
        }

        private void Instantiate(Sprite a_GraphicSprite, bool a_UpdateRect)
        {
            GameObject t_Go = Instantiate(numberPrototype, contentHolder);
            t_Go.SetActive(true);

            if (a_UpdateRect)
            {
                UpdateRect(t_Go.transform, 3, 3);
            }

            activeScoreGraphics.Add(t_Go);
        }

        private void UpdateRect(Transform a_ObjTransform, float a_Width, float a_Height)
        {
            RectTransform t_RectTransform = a_ObjTransform.GetComponent<RectTransform>();
            Rect t_Rect = t_RectTransform.rect;
            t_RectTransform.rect.Set(t_Rect.x, t_Rect.y, a_Width, a_Height);
        }

        private IEnumerator ClearScoreboard(float a_Delay = 0.0f)
        {
            yield return new WaitForSeconds(a_Delay);
            if (activeScoreGraphics.Any())
            {
                activeScoreGraphics.ForEach(x => Destroy(x));
                activeScoreGraphics.Clear();
            }
            cross.SetActive(false);
            bg.SetActive(false);
        }

        private List<int> GetDigits(int a_Value)
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