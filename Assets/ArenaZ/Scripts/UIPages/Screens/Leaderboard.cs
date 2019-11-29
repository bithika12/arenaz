using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;

namespace ArenaZ.LeaderBoard
{
    public class Leaderboard : RedAppleSingleton<Leaderboard>
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

        public void OnClickLeaderBoardBack()
        {
            UIManager.Instance.HideScreenNormalWithAnim(Page.LeaderBoardPanel.ToString());
        }
    }
}
