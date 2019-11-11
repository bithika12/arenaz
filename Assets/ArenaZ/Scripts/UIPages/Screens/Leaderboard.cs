using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;

namespace ArenaZ.LeaderBoard
{
    [RequireComponent(typeof(UIScreen))]
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] private Button leaderBoardBackButton;

        private void Start()
        {
            GettingButtonReferences();
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        private void GettingButtonReferences()
        {
            leaderBoardBackButton.onClick.AddListener(OnClickLeaderBoardBack);
        }

        private void ReleaseButtonReferences()
        {
            leaderBoardBackButton.onClick.RemoveListener(OnClickLeaderBoardBack);
        }

        private void OnClickLeaderBoardBack()
        {
            UIManager.Instance.HideScreen(Page.LeaderBoard.ToString());
        }
    }
}
