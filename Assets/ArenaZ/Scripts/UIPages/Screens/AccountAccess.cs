using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using System.Collections;
using ArenaZ.SettingsManagement;

namespace ArenaZ.Screens
{
    public class AccountAccess : RedAppleSingleton<AccountAccess>
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

        public void TasksAfterLogin(string userName,AccountAccessType type)
        {
            StartCoroutine(DoTasksAfterLogin(userName,type));
        }

        private IEnumerator DoTasksAfterLogin(string userName,AccountAccessType accessType)
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
            User.userName = userName;
            Settings.Instance.LogInLogOutButtonNameSet(Constants.logout);
            PlayerPrefs.SetInt("Logout", 0);
            OpenCharacterUI();
            UIManager.Instance.setUserName?.Invoke(userName);
            CharacterSelection.Instance.ResetCharacterScroller(userName);
        }


        private void OnClickFirstRegisterButton()
        {
            UIManager.Instance.ScreenShow(Page.RegistrationOverlay.ToString(), Hide.none);
        }

        private void OnClickFirstLoginButton()
        {
            UIManager.Instance.ScreenShow(Page.LogINOverlay.ToString(), Hide.none);
        }

        private void OnClickAccountAccessPanelOpen()
        {
            UIManager.Instance.ScreenShow(Page.AcoountAccesOverlay.ToString(), Hide.none);
        }

        private void OnClickAccountAccessPanelClose()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetailsPanel.ToString());
        }

        private void OpenCharacterUI()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetailsPanel.ToString());
            UIManager.Instance.ScreenShow(Page.CharacterSelectionPanel.ToString(), Hide.none);
        }
        #endregion
    }
}
