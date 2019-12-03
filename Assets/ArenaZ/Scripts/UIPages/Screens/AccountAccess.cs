using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;

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
        #endregion
    }
}
