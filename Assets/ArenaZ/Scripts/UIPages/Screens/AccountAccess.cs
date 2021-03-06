using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using System.Collections;
using ArenaZ.SettingsManagement;
using RedApple;
using System;

namespace ArenaZ.Screens
{
    public class AccountAccess : Singleton<AccountAccess>
    {
        //Private Variables
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button firstRegisterButton;
        [SerializeField] private Button firstLoginButton;
        [SerializeField] private Button closeButton;

        [Header("Integers And Floating Points")]
        [Space(5)]
        [SerializeField] private int delayToOpenCharacterUI;

        [SerializeField] private CharacterSelection characterSelection;
        [SerializeField] private NewUserCongratulation newUserCongratulation;

        private void Start()
        {
            GettingButtonReferences();
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        #region Button_References
        private void GettingButtonReferences()
        {
            firstRegisterButton.onClick.AddListener(OnClickFirstRegisterButton);
            firstLoginButton.onClick.AddListener(OnClickFirstLoginButton);
            closeButton.onClick.AddListener(OnClickAccountAccessPanelClose);
        }

        private void ReleaseButtonReferences()
        {
            firstRegisterButton.onClick.RemoveListener(OnClickFirstRegisterButton);
            firstLoginButton.onClick.RemoveListener(OnClickFirstLoginButton);
            closeButton.onClick.RemoveListener(OnClickAccountAccessPanelClose);
        }
        #endregion

        #region UI_Functionalities
        public void TasksAfterLogin(string userName, AccountAccessType type, bool newUser, string coinValue ="")
        {
            StartCoroutine(DoTasksAfterLogin(userName, type, newUser, coinValue));
        }

        private IEnumerator DoTasksAfterLogin(string userName,AccountAccessType accessType, bool newUser, string coinValue = "")
        {
            float delay = 0f;
            if (accessType == AccountAccessType.Registration)
            {
                delay = delayToOpenCharacterUI;
            }
            else
            {
                delay = 0;
            }
            yield return new WaitForSeconds(delay);
            UIManager.Instance.SetComponent<Text>(Page.LoggedInText.ToString(),true);
            Settings.Instance.LogInLogOutButtonNameSet(ConstantStrings.logout);
            PlayerPrefs.SetInt(PlayerPrefsValue.Logout.ToString(), 0);
            OpenCharacterUI();
            UIManager.Instance.setUserName?.Invoke(userName);
            UIManager.Instance.setCoinAndCup?.Invoke(User.UserCoin.ToString(), User.UserCup.ToString());
            CharacterSelection.Instance.ResetCharacterScroller(userName);
            SocketManager.Instance.AddUser();

            yield return new WaitForSeconds(0.5f);
            int t_UserCoin = 0;
            int.TryParse(coinValue, out t_UserCoin);
            if (newUser && t_UserCoin > 0)
            {
                UIManager.Instance.ShowScreen(Page.NewUserCongratulationPanel.ToString());
                newUserCongratulation.SetWelComeText(coinValue);
            }
        }


        private void OnClickFirstRegisterButton()
        {
            UIManager.Instance.ShowScreen(Page.RegistrationOverlay.ToString());
        }

        private void OnClickFirstLoginButton()
        {
            UIManager.Instance.ShowScreen(Page.LogINOverlay.ToString());
        }

        private void OnClickAccountAccessPanelOpen()
        {
            UIManager.Instance.ShowScreen(Page.AccountAccesOverlay.ToString());
        }

        private void OnClickAccountAccessPanelClose()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetailsPanel.ToString());
        }

        private void OpenCharacterUI()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetailsPanel.ToString());
            UIManager.Instance.ShowScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(Page.CharacterSelectionPanel.ToString());

            characterSelection.Invoke("Initialize", 1.5f);
        }
        #endregion
    }
}

#region Unused
//Type userdataType = UserData.GetType();
//if (userdataType.Equals(typeof(UserLogin)))
//{
//    storeUserData(typeof(UserLogin));
//}
//else
//{

//}

//MethodInfo dsfg = userdataType.GetProperty("UserId").GetMethod;
//Debug.Log(dsfg);
#endregion