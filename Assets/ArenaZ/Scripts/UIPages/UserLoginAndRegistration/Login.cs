using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using UnityEngine;
using UnityEngine.UI;
using RedApple.Utils;
using ArenaZ.SettingsManagement;
using ArenaZ.Screens;
using ArenaZ.RegistrationUser;
using DevCommons.Utility;
using Newtonsoft.Json;

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

        [Header("Class Ref")]
        [Space(5)]
        [SerializeField] private MapHandler mapHandlerRef;
        [SerializeField] private Wallet.WalletHandler walletHandlerRef;

        RegularExpression regExp = new RegularExpression();
        private string password;

        private void Awake()
        {
            Debug.Log("Awake Called");
            LoginHandler.userLogin += onClickUserLogin;
        }

        private void OnEnable()
        {
            Settings.inputFieldclear += ClearLoginInputFieldData;
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

        private void onClickUserLogin(string emailID, string password)
        {
            emailID = emailID.NonWhitespaceStr();
            password = password.NonWhitespaceStr();

            logInIf_userEmail.text = emailID;
            logInIf_password.text = password;

            Debug.Log("Logging in");
            //if (GetMessageWhenFaultCheckOnLogin(emailID, Checking.EmailID) != null)
            //{
            //    UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnLogin(emailID, Checking.EmailID), PopUpduration);
            //    return;
            //}

            if (string.IsNullOrEmpty(emailID) || string.IsNullOrWhiteSpace(emailID))
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), ConstantStrings.emptyFieldWarning, PopUpduration);
                return;
            }
            else if (GetMessageWhenFaultCheckOnLogin(password, Checking.Password) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnLogin(password, Checking.Password), PopUpduration);
                return;
            }
            else
            {
                this.password = password;
                RestManager.LoginProfile(emailID, password, OnCompleteLogin, OnErrorLogin);
            }
        }

        private void OnCompleteLogin(UserLogin loggedinProfile)
        {
            Debug.Log($"Login: {JsonConvert.SerializeObject(loggedinProfile)}");
            RestManager.AccessToken = loggedinProfile.AccessToken;

            UserData userData = new UserData(loggedinProfile.Email, password, loggedinProfile.AccessToken);
            FileHandler.SaveToFile<UserData>(userData, ConstantStrings.USER_SAVE_FILE_KEY);

            storeUserData(loggedinProfile);
            AccountAccess.Instance.TasksAfterLogin(loggedinProfile.UserName, AccountAccessType.Login, false);
            OnClickLoginPopUpClose();

            mapHandlerRef.GetGames();
            walletHandlerRef.GetWalletDetails();
        }

        private void storeUserData(UserLogin userLogin)
        {
            User.UserName = userLogin.UserName;
            User.UserId = userLogin.UserId;
            User.UserEmailId = userLogin.Email;
            User.UserAccessToken = userLogin.AccessToken;
            User.UserCoin = userLogin.UserCoin;
            User.UserCup = userLogin.UserCup;
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
