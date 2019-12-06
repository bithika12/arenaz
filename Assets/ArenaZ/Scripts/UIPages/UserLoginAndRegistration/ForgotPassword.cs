using System;
using UnityEngine;
using UnityEngine.UI;
using RedApple;
using RedApple.Utils;
using ArenaZ.Manager;

public class ForgotPassword : MonoBehaviour
{
    [Header("Buttons")]
    [Space(5)]
    [SerializeField] private Button forgotPasswordButton;
    [SerializeField] private Button forgotBackButton;

    [Header("Input Fields")]
    [Space(5)]
    [SerializeField] private InputField forgotPass_userEmail;
    [SerializeField] private InputField forgotPass_userName;

    [Header("Integer")]
    [Space(5)]
    private float PopUpduration;

    RegularExpression regularExp = new RegularExpression();

    private void OnEnable()
    {
        GettingButtonReferences();
    }

    private void OnDisable()
    {
        ReleaseButtonReferences();
    }

    private void ClearForgotPassWordInputFieldData()
    {
        forgotPass_userEmail.text = null;
        forgotPass_userName.text = null;
    }

    private void GettingButtonReferences()
    {
        forgotBackButton.onClick.AddListener(OnClickForgotPasswordPopUpClose);
        forgotPasswordButton.onClick.AddListener(OnClickForgotPassword);
    }

    private void ReleaseButtonReferences()
    {
        forgotBackButton.onClick.RemoveListener(OnClickForgotPasswordPopUpClose);
        forgotPasswordButton.onClick.RemoveListener(OnClickForgotPassword);
    }

    private void OnClickForgotPassword()
    {
        Debug.Log("OnClickForgotPassword");
        if (string.IsNullOrWhiteSpace(forgotPass_userEmail.text) && string.IsNullOrWhiteSpace(forgotPass_userName.text))
        {
            Debug.Log("Dutoi Null");
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), Constants.wrongEmailAndUsername, PopUpduration);
            return;
        }
        else if(!regularExp.emailFormat.IsMatch(forgotPass_userEmail.text))
        {
            Debug.Log("email ta match koreni, username call holo");
            if (string.IsNullOrWhiteSpace(forgotPass_userName.text))
            {
                Debug.Log("String is null");
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), Constants.wrongEmailAndUsername, PopUpduration);
            }
            else
            {
                RestManager.ForgotPassword(true, forgotPass_userName.text, OnCompleteForgotPassword, OnErrorForgotPassword);
            }
        }
        else if(regularExp.emailFormat.IsMatch(forgotPass_userEmail.text))
        {
            Debug.Log("email ta match korlo");
            RestManager.ForgotPassword(true, forgotPass_userEmail.text, OnCompleteForgotPassword, OnErrorForgotPassword);
        }
    }

    private void OnCompleteForgotPassword()
    {
        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), Constants.rightEmailAndUserName, PopUpduration);
        OnClickForgotPasswordPopUpClose();
       // UIManager.Instance.ScreenShowNormal(Page.LogINOverlay.ToString());
    }

    private void OnErrorForgotPassword(RestUtil.RestCallError obj)
    {
        UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), obj.Description, PopUpduration);
    }

    private void OnClickForgotPasswordPopUpClose()
    {
        ClearForgotPassWordInputFieldData();
        UIManager.Instance.HideScreenImmediately(Page.ForgotPasswordOverlay.ToString());
        UIManager.Instance.ScreenShowNormal(Page.LogINOverlay.ToString());
    }

}
