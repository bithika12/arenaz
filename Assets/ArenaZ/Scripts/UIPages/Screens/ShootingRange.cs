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
                    PlayerMatch.Instance.SetEnemyName(users[i].UserName);
                    PlayerMatch.Instance.SetEnemyProfileImage(users[i].RaceName);
                    UIManager.Instance.ScreenShow(Page.PlayerMatchPanel.ToString(), Hide.none);
                }
                else
                {
                    Debug.Log("UserIdMatched");
                }
            }
            PlayerMatch.Instance.LoadScene();
        }

        private void OnUserJoin(string data)
        {
            Debug.Log($"Game Start : {data}");
            var userJoinData = DataConverter.DeserializeObject<GamePlayDataFormat<UserJoin>>(data);
            UserJoin[] users = userJoinData.result.Users;
            Debug.Log("UserId: " + userJoinData.result.Users[0].UserId);
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
