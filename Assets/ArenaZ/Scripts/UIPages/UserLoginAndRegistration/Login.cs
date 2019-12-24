using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using UnityEngine;
using UnityEngine.UI;
using RedApple.Utils;
using ArenaZ.SettingsManagement;
using ArenaZ.Screens;
using ArenaZ.RegistrationUser;

namespace ArenaZ.LoginUser
{
    public class Login : MonoBehaviour
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button loginForgotButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button loginBackButton;

        [Header("Input Fields")]
        [Space(5)]
        [SerializeField] private InputField logInIf_userEmail;
        [SerializeField] private InputField logInIf_password;

        [Header("Integer")]
        [Space(5)]
        [SerializeField] private int PopUpduration;

        RegularExpression regExp = new RegularExpression();

        private void Start()
        {
            Settings.Instance.inputFieldclear += ClearLoginInputFieldData;
        }

        private void OnEnable()
        {
            GettingButtonReferences();           
        }

        private void OnDisable()
        {
            ReleaseButtonReferences();
        }

        #region Button_References
        private void GettingButtonReferences()
        {
            loginBackButton.onClick.AddListener(OnClickLoginPopUpClose);
            loginForgotButton.onClick.AddListener(OnClickForgotPasswordPopUpShow);
            loginButton.onClick.AddListener(OnClickUserLogin);
        }

        private void ReleaseButtonReferences()
        {
            loginBackButton.onClick.RemoveListener(OnClickLoginPopUpClose);
            loginForgotButton.onClick.RemoveListener(OnClickForgotPasswordPopUpShow);
            loginButton.onClick.RemoveListener(OnClickUserLogin);
        }
        #endregion

        private void ClearLoginInputFieldData()
        {
            logInIf_userEmail.text = null;
            logInIf_password.text = null;
        }

        private void OnClickUserLogin()
        {
            Debug.Log("Logging in");
            if (GetMessageWhenFaultCheckOnLogin(logInIf_userEmail.text, Checking.EmailID) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnLogin(logInIf_userEmail.text, Checking.EmailID), PopUpduration);
                return;
            }
            else if (GetMessageWhenFaultCheckOnLogin(logInIf_password.text, Checking.Password) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnLogin(logInIf_password.text, Checking.Password), PopUpduration);
                return;
            }
            else
            {
                RestManager.LoginProfile(logInIf_userEmail.text, logInIf_password.text, OnCompleteLogin, OnErrorLogin);
            }
        }

        private void OnCompleteLogin(UserLogin loggedinProfile)
        {
            storeUserData(loggedinProfile);
            AccountAccess.Instance.TasksAfterLogin(loggedinProfile.UserName,AccountAccessType.Login);
            OnClickLoginPopUpClose();
        }


        private void storeUserData(UserLogin userLogin)
        {
            User.userName = userLogin.UserName;
            User.userId = userLogin.UserId;
            User.userEmailId = userLogin.Email;
            User.UserAccessToken = userLogin.AccessToken;
        }

        private void OnErrorLogin(RestUtil.RestCallError restError)
        {
            Debug.LogError(restError.Description);
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), restError.Description, PopUpduration);
        }

        private string GetMessageWhenFaultCheckOnLogin(string message, Checking type)
        {
            switch (type)
            {
                case Checking.EmailID:
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return ConstantStrings.loginEmailPasswordBlank;
                    }
                    break;

                case Checking.Password:
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return ConstantStrings.loginEmailPasswordBlank;
                    }
                    break;
            }
            return null;
        }

        private void OnClickLoginPopUpClose()
        {
            ClearLoginInputFieldData();
            UIManager.Instance.HideScreen(Page.LogINOverlay.ToString());
        }

        private void OnClickForgotPasswordPopUpShow()
        {
            UIManager.Instance.HideScreenImmediately(Page.LogINOverlay.ToString());
            UIManager.Instance.ScreenShowNormal(Page.ForgotPasswordOverlay.ToString());
        }
    }
}
