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

        private void Awake()
        {
            Debug.Log("Awake Called");
            LoginHandler.userLogin += onClickUserLogin;
        }

        private void Start()
        {
            Settings.inputFieldclear += ClearLoginInputFieldData;
            PlayerPrefs.GetInt(PlayerprefsValue.AutoLogin.ToString(), 0);
            GettingButtonReferences();
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        #region Button_References
        private void GettingButtonReferences()
        {
            loginBackButton.onClick.AddListener(OnClickLoginPopUpClose);
            loginForgotButton.onClick.AddListener(OnClickForgotPasswordPopUpShow);
            loginButton.onClick.AddListener(()=> onClickUserLogin(logInIf_userEmail.text,logInIf_password.text));
        }

        private void ReleaseButtonReferences()
        {
            loginBackButton.onClick.RemoveListener(OnClickLoginPopUpClose);
            loginForgotButton.onClick.RemoveListener(OnClickForgotPasswordPopUpShow);
            loginButton.onClick.RemoveListener(() => onClickUserLogin(logInIf_userEmail.text, logInIf_password.text));
        }
        #endregion

        private void ClearLoginInputFieldData()
        {
            logInIf_userEmail.text = null;
            logInIf_password.text = null;
        }

        private void onClickUserLogin(string emailID,string password)
        {
            Debug.Log("Logging in");
            if (GetMessageWhenFaultCheckOnLogin(emailID, Checking.EmailID) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnLogin(emailID, Checking.EmailID), PopUpduration);
                return;
            }
            else if (GetMessageWhenFaultCheckOnLogin(password, Checking.Password) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnLogin(password, Checking.Password), PopUpduration);
                return;
            }
            else
            {
                RestManager.LoginProfile(emailID, password, OnCompleteLogin, OnErrorLogin);
            }
        }

        private void OnCompleteLogin(UserLogin loggedinProfile)
        {
            RestManager.AccessToken = loggedinProfile.AccessToken;
            Debug.Log("OnLogin User Access Token:  " + loggedinProfile.AccessToken);
            Debug.Log("Login Successful: " + loggedinProfile);
            if (Application.platform == RuntimePlatform.Android && PlayerPrefs.GetInt(PlayerprefsValue.AutoLogin.ToString()) == 0)
            {
                Debug.Log("Saving UserName and Password");
                UIManager.Instance.SaveDetails(PlayerprefsValue.LoginID.ToString(), logInIf_userEmail.text);
                UIManager.Instance.SaveDetails(PlayerprefsValue.Password.ToString(), logInIf_password.text);
            }
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
            UIManager.Instance.ShowScreen(Page.ForgotPasswordOverlay.ToString());
        }
    }
}
