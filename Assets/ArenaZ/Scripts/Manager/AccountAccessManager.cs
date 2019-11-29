using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using ArenaZ.Manager;
using ArenaZ.Screens;
using RedApple;
using RedApple.Utils;
using RedApple.Api.Data;
using System.Text.RegularExpressions;
using ArenaZ.LevelMangement;
using ArenaZ.GameMode;

namespace ArenaZ.AccountAccess
{
    public class AccountAccessManager : RedAppleSingleton<AccountAccessManager>
    {
        //Private Variables
        [Header("Buttons")][Space(5)]
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

        #region UnUsable_Code

        //public void ClearRegInputFieldData()
        //{
        //    regIf_userEmail.text = null;
        //    regIf_UserName.text = null;
        //    regIf_UserPassword.text = null;
        //    regIf_UserConfPassword.text = null;
        //}

        //public void ClearLoginInputFieldData()
        //{
        //    logInIf_userEmail.text = null;
        //    logInIf_password.text = null;
        //}

        //private void OnClickUserRegistration()
        //{
        //    if (RegistrationFaultCheck(regIf_userEmail.text, Checking.EmailID) != null)
        //    {
        //        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), RegistrationFaultCheck(regIf_userEmail.text, Checking.EmailID), PopUpduration);
        //        return;
        //    }
        //    else if (RegistrationFaultCheck(regIf_UserName.text, Checking.Username) != null)
        //    {
        //        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), RegistrationFaultCheck(regIf_UserName.text, Checking.Username), PopUpduration);
        //        return;
        //    }
        //    else if (RegistrationFaultCheck(regIf_UserPassword.text, Checking.Password) != null)
        //    {
        //        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), RegistrationFaultCheck(regIf_UserPassword.text, Checking.Password), PopUpduration);
        //        return;
        //    }
        //    else  if (string.IsNullOrWhiteSpace(regIf_UserConfPassword.text))
        //    {
        //        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(),Constants.wrongConfPassword, PopUpduration);
        //        return;
        //    }
        //    else if (regIf_UserPassword.text != regIf_UserConfPassword.text)
        //    {
        //        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(),Constants.wrongConfPassword, PopUpduration);
        //        return;
        //    }
        //    else if (regIf_UserPassword.text.Equals(regIf_UserConfPassword.text))
        //    {
        //        RestManager.UserRegistration(regIf_userEmail.text, regIf_UserName.text, regIf_UserPassword.text, OnCompleteRegistration, OnErrorRegistration);
        //        Debug.Log("Mail UserName password:   " + regIf_userEmail.text + regIf_UserName.text + regIf_UserPassword.text + regIf_UserConfPassword.text);
        //    }           
        //}

        //private void OnCompleteRegistration(CreateAccount registeredProfile)
        //{
        //    Debug.Log("Registered:  " + registeredProfile.UserId);
        //    OnClickRegisterPopUpClose();
        //    ClearRegInputFieldData();
        //    UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(),Constants.successFullyRegisterd, PopUpduration);
        //}

        //private void OnErrorRegistration(RestUtil.RestCallError obj)
        //{
        //    Debug.LogError("Error On Registration: "+obj.Description);
        //    UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(),Constants.emailAlreadyExists, PopUpduration);
        //}

        //private void OnClickUserLogin()
        //{
        //    Debug.Log("Logging in");
        //    if (LoginFaultCheck(logInIf_userEmail.text, Checking.EmailID) != null)
        //    {
        //        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), LoginFaultCheck(logInIf_userEmail.text, Checking.EmailID), PopUpduration);
        //        return;
        //    }
        //    else if (LoginFaultCheck(logInIf_password.text, Checking.Password) != null)
        //    {
        //        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), LoginFaultCheck(logInIf_password.text, Checking.Password), PopUpduration);
        //        return;
        //    }
        //    else
        //    {
        //        RestManager.LoginProfile(logInIf_userEmail.text, logInIf_password.text, OnCompleteLogin, OnErrorLogin);
        //    }
        //}

        //private void OnCompleteLogin(UserLogin loggedinProfile)
        //{
        //    Debug.Log("Logged In Profile:  " + loggedinProfile.UserId + regIf_userEmail.text);
        //    UserName = loggedinProfile.UserName;
        //    LogInAndLogOutButtonName = Constants.logout;
        //    PlayerPrefs.SetInt("Logout", 0);
        //    ClearLoginInputFieldData();
        //    OnClickLoginPopUpClose();
        //    OpenCharacterUI();
        //    UIManager.Instance.setUserName?.Invoke(UserName);
        //    CharacterSelection.Instance.ResetCharacterScroller(loggedinProfile.UserName);
        //}

        //private void OnErrorLogin(RestUtil.RestCallError restError)
        //{
        //    Debug.LogError(restError.Description);
        //    UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), restError.Description, PopUpduration);
        //}

        //private string RegistrationFaultCheck(string message,Checking type)
        //{
        //    RegularExpression checking = new RegularExpression();
        //    switch (type)
        //    {
        //        case Checking.Username:

        //            if (string.IsNullOrWhiteSpace(message))
        //            {
        //                return Checking.Username.ToString() + " " + Constants.isNull;
        //            }
        //            if (!checking.hasNumber.IsMatch(message))
        //            {
        //                return Checking.Username.ToString() + " " + Constants.doesNotHaveNumber;
        //            }
        //            if (!checking.hasCapAndSmall.IsMatch(message))
        //            {
        //                return Checking.Username.ToString() + " " + Constants.doesNotHaveChar;
        //            }
        //            if (checking.hasSpace.IsMatch(message))
        //            {
        //                return Checking.Username.ToString() + " " + Constants.containedSpace;
        //            }
        //            break;
        //        case Checking.EmailID:

        //            if (string.IsNullOrWhiteSpace(message))
        //            {
        //                return Checking.EmailID.ToString() + " " + Constants.isNull;
        //            }
        //            if (!checking.emailFormat.IsMatch(message))
        //            {
        //                return Checking.EmailID.ToString() + " " + Constants.mailIsNotValid;
        //            }
        //            break;

        //        case Checking.Password:

        //            if (string.IsNullOrWhiteSpace(message))
        //            {
        //                return Checking.Password.ToString() + " " + Constants.passwordIsNull;
        //            }
        //            if (!checking.hasNumber.IsMatch(message))
        //            {
        //                return Checking.Password.ToString() + " " + Constants.doesNotHaveNumber;
        //            }
        //            if (!checking.hasUpperChar.IsMatch(message))
        //            {
        //                return Checking.Password.ToString() + " " + Constants.doesNotHaveUpperCaseChar;
        //            }
        //            if (!checking.hasLowerChar.IsMatch(message))
        //            {
        //                return Checking.Password.ToString() + " " + Constants.doesNotHaveLowerCaseChar;
        //            }
        //            if (!checking.hasspecialCharacter.IsMatch(message))
        //            {
        //                return Checking.Password.ToString() + " " + Constants.doesNotHaveSpecialChar;
        //            }
        //            if (checking.hasSpace.IsMatch(message))
        //            {
        //                return Checking.Password.ToString() + " " + Constants.containedSpace;
        //            }
        //            if (!checking.hasMinimum8Chars.IsMatch(message))
        //            {
        //                return Checking.Password.ToString() + " " + Constants.doesNotHaveMinEightChar;
        //            }
        //            break;
        //        default:
        //            Debug.LogError("Please Choose Any Checking Type");
        //            break;
        //    }
        //    return null;
        //}

        //private string LoginFaultCheck(string message, Checking type)
        //{
        //    RegularExpression regExp = new RegularExpression();
        //    switch (type)
        //    {
        //        case Checking.EmailID:
        //            if(string.IsNullOrWhiteSpace(message))
        //            {
        //                return Checking.EmailID.ToString() + " " + Constants.isNull;
        //            }
        //            break;

        //        case Checking.Password:
        //            if(string.IsNullOrWhiteSpace(message))
        //            {
        //                return Checking.Password.ToString() + " " +Constants.isNull;
        //            }
        //            break;
        //    }
        //    return null;
        //}

        //public void OnClickLogout()
        //{
        //    RestManager.LogOutProfile(OnCompleteLogout, OnErrorLogout);
        //}

        //private void OnCompleteLogout(UserLogin obj)
        //{
        //    Debug.Log("User Logged Out SuccessFully");
        //    LogInAndLogOutButtonName = Constants.login;
        //    TopAndBottomBarScreen.Instance.count = 0;
        //    // UIManager.Instance.ShowPopWithText(Page.PopUpTextSettings.ToString(), successFullyLoggedOut, PopUpduration);
        //}

        //private void OnErrorLogout(RestUtil.RestCallError obj)
        //{
        //    Debug.Log(obj.Description);
        //}
        #endregion


        #region UI_Functionalities

        private void OnClickFirstRegisterButton()
        {
            UIManager.Instance.ScreenShow(Page.RegistrationOverlay.ToString(),Hide.none);
        }

        private void OnClickFirstLoginButton()
        {
            UIManager.Instance.ScreenShow(Page.LogINOverlay.ToString(), Hide.none);
        }

        private void OnClickAccountAccessPanelOpen()
        {
            UIManager.Instance.ScreenShow(Page.AcoountAccesOverlay.ToString(),Hide.none);
        }

        private void OnClickAccountAccessPanelClose()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetailsPanel.ToString());
        }
        #endregion
    }
}
