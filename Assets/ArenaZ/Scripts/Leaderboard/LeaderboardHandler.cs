using ArenaZ.Manager;
using Newtonsoft.Json;
using RedApple;
using RedApple.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArenaZ
{
    public class LeaderboardHandler : MonoBehaviour
    {
        [SerializeField] private LeaderboardCell leaderboardCellPrototype;
        [SerializeField] private Transform container;

        [SerializeField] private LeaderboardCell selfLeaderboardCell;

        private List<LeaderboardCell> activeLeaderboardCells = new List<LeaderboardCell>();

        private void OnEnable()
        {
            ClearWindow();
            RestManager.FetchLeaderboard(User.UserEmailId, OnRequestSuccess, OnRequestFailure);
        }

        private void OnRequestSuccess(LeaderboardDetails a_LeaderboardDetails)
        {
            Debug.Log($"LeaderboardDetails: {JsonConvert.SerializeObject(a_LeaderboardDetails)}");
            if (a_LeaderboardDetails != null && a_LeaderboardDetails.leaderboardDatas.Any())
            {
                Debug.Log("LBCount: " + a_LeaderboardDetails.leaderboardDatas.Count);
                a_LeaderboardDetails.leaderboardDatas.ForEach(x =>
                {
                    Debug.Log("LBUserName: " + x.Name);
                    GameObject t_Go = Instantiate(leaderboardCellPrototype.gameObject, container);
                    t_Go.SetActive(true);
                    LeaderboardCell t_Cell = t_Go.GetComponent<LeaderboardCell>();
                    t_Cell.InitializeCell(x);
                    activeLeaderboardCells.Add(t_Cell);

                    if (x.Name.Equals(User.UserName))
                    {
                        selfLeaderboardCell.gameObject.SetActive(true);
                        selfLeaderboardCell.InitializeCell(x);
                    }
                });
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError a_Error)
        {
            Debug.LogError($"LeaderboardDetails: {a_Error.Description}");
        }

        private void OnDisable()
        {
            ClearWindow();
        }

        private void ClearWindow()
        {
            selfLeaderboardCell.gameObject.SetActive(false);
            if (activeLeaderboardCells.Any())
            {
                activeLeaderboardCells.ForEach(x => Destroy(x.gameObject));
                activeLeaderboardCells.Clear();
            }
        }

        public void CloseWindow()
        {
            UIManager.Instance.HideScreen(Page.LeaderBoardPanel.ToString());
            UIManager.Instance.ShowDefaultScreens();
        }
    }
}
