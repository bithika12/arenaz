using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;

namespace ArenaZ.GameMode
{
    public class ShootingRange : Singleton<ShootingRange>
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button shootingRangeBackButton;
        [SerializeField] private Button coinButton10;
        [SerializeField] private Button coinButton50;
        [SerializeField] private Button coinButton100;
        [SerializeField] private Button coinButton250;
        [SerializeField] private Button coinButton500;

        [Header("Images")]
        [Space(5)]
        [SerializeField] private Image profileImage;

        [Header("Text")]
        [Space(5)]
        [SerializeField] private Text userName;

        private void Start()
        {
            GettingButtonReferences();
            ListenSocketEvents();
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetProfileImage;
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();        
        }

        #region ButtonReferences
        private void GettingButtonReferences()
        {
            shootingRangeBackButton.onClick.AddListener(OnClickShootingRangeBack);
            coinButton10.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton50.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton100.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton250.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton500.onClick.AddListener(OnClickStartGameWithCoinValue);
        }

        private void ReleaseButtonReferences()
        {
            shootingRangeBackButton.onClick.RemoveListener(OnClickShootingRangeBack);
            coinButton10.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton50.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton100.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton250.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton500.onClick.RemoveListener(OnClickStartGameWithCoinValue);
        }
        #endregion

        private void ListenSocketEvents()
        {
            SocketListener.Listen(SocketListenEvents.userJoin.ToString(), OnUserJoin);
            SocketListener.Listen(SocketListenEvents.gameStart.ToString(), OnGameStart);
        }

        private void OnGameStart(string data)
        {
            Debug.Log($"Game Start : {data}");
            var gameStartData = DataConverter.DeserializeObject<GamePlayDataFormat<UserJoin>>(data);
            UserJoin[] users = gameStartData.result.Users;
            Debug.Log("No. of users:  "+users.Length);
            for (int i = 0; i < users.Length; i++)
            {
                if (User.userId != users[i].UserId)
                {
                    Opponent.opponentName = users[i].UserName;
                    Opponent.opponentId = users[i].UserId;
                    Opponent.opponentRace = users[i].RaceName;
                    Opponent.opponentColor = users[i].ColorName;
                    PlayerMatch.Instance.SetOpponentName(users[i].UserName);
                    PlayerMatch.Instance.SetOpponentProfileImage(users[i].RaceName);
                    UIManager.Instance.HideScreen(Page.ShootingrangePanel.ToString());
                    UIManager.Instance.ScreenShow(Page.PlayerMatchPanel.ToString(), Hide.none);
                }
            }
            PlayerMatch.Instance.LoadGameplay();
        }

        private void OnUserJoin(string data)
        {
            Debug.Log($"User Join : {data}");
            var userJoinData = DataConverter.DeserializeObject<GamePlayDataFormat<UserJoin>>(data);
            User.RoomName = userJoinData.result.RoomName;
        }

        public void SetProfileImage(string imageName)
        {
            profileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
        }

        public void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        private void OnClickShootingRangeBack()
        {
            UIManager.Instance.ScreenShow(Page.LevelSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.HideScreen(Page.ShootingrangePanel.ToString());
        }

        private void OnClickStartGameWithCoinValue()
        {
            SocketManager.Instance.GameRequest();
        }
    }
}
