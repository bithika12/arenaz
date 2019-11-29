using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using ArenaZ.AccountAccess;
using UnityEngine;
using UnityEngine.UI;
using RedApple.Utils;
using System;

public class Login : MonoBehaviour
{
    [Header("Buttons")]
    [Space(5)]
    [SerializeField] private Button forgotButton;
    [SerializeField] private Button loginButton;

    [Header("Input Fields")]
    [Space(5)]
    [SerializeField] private InputField logInIf_userEmail;
    [SerializeField] private InputField logInIf_password;

    [Header("Integer")]
    [Space(5)]
    [SerializeField] private int PopUpduration;

    RegularExpression regExp = new RegularExpression();

    private void OnEnable()
    {
        GettingButtonReferences();
        Settings.Instance.inputFieldclear += ClearLoginInputFieldData;
    }

    private void OnDisable()
    {
        ReleaseButtonReferences();
        Settings.Instance.inputFieldclear -= ClearLoginInputFieldData;
    }

    #region Button_References
    private void GettingButtonReferences()
    {
        forgotButton.onClick.AddListener(OnClickLoginPopUpClose);
        loginButton.onClick.AddListener(OnClickUserLogin);
    }

    private void ReleaseButtonReferences()
    {
        forgotButton.onClick.RemoveListener(OnClickLoginPopUpClose);
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
        User.userName = loggedinProfile.UserName;
        Settings.Instance.LogInLogOutButtonNameSet(Constants.logout);
        PlayerPrefs.SetInt("Logout", 0);
        ClearLoginInputFieldData();
        OnClickLoginPopUpClose();
        OpenCharacterUI();
        UIManager.Instance.setUserName?.Invoke(loggedinProfile.UserName);
        CharacterSelection.Instance.ResetCharacterScroller(loggedinProfile.UserName);
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
                    return Checking.EmailID.ToString() + " " + Constants.isNull;
                }
                break;

            case Checking.Password:
                if (string.IsNullOrWhiteSpace(message))
                {
                    return Checking.Password.ToString() + " " + Constants.isNull;
                }
                break;
        }
        return null;
    }

    private void OpenCharacterUI()
    {
        UIManager.Instance.HideScreen(Page.AccountAccessDetailsPanel.ToString());
        UIManager.Instance.ScreenShow(Page.CharacterSelectionPanel.ToString(), Hide.none);
    }

    private void OnClickLoginPopUpClose()
    {
        ClearLoginInputFieldData();
        UIManager.Instance.HideScreen(Page.LogINOverlay.ToString());
    }
}
