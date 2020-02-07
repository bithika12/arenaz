using RedApple;
using RedApple.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace ArenaZ
{
    public class GameHistoryHandler : MonoBehaviour
    {
        [SerializeField] private MatchDetailsCell matchDetailsCellPrototype;
        [SerializeField] private Transform container;

        private List<MatchDetailsCell> activeMatchDetailsCells = new List<MatchDetailsCell>();

        private void OnEnable()
        {
            ClearWindow();
            RestManager.GetGameHistory(User.UserEmailId, OnRequestSuccess, OnRequestFailure);
        }

        private void OnRequestSuccess(GameHistoryMatchDetails a_GameHistoryMatchDetails)
        {
            Debug.Log($"GameHistoryMatchDetails: {JsonConvert.SerializeObject(a_GameHistoryMatchDetails)}");
            if (a_GameHistoryMatchDetails != null && a_GameHistoryMatchDetails.gameHistoryGameDetails.Any())
            {
                a_GameHistoryMatchDetails.gameHistoryGameDetails.ForEach(x =>
                {
                    GameObject t_Go = Instantiate(matchDetailsCellPrototype.gameObject, container);
                    t_Go.SetActive(true);
                    MatchDetailsCell t_Cell = t_Go.GetComponent<MatchDetailsCell>();
                    t_Cell.InitializeCell(x);
                    activeMatchDetailsCells.Add(t_Cell);
                });
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError a_Error)
        {
            Debug.LogError($"GameHistoryMatchDetails: {a_Error.Description}");
        }

        private void ClearWindow()
        {
            if (activeMatchDetailsCells.Any())
            {
                activeMatchDetailsCells.ForEach(x => Destroy(x.gameObject));
                activeMatchDetailsCells.Clear();
            }
        }
    }
}