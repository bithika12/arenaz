using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedApple;
using RedApple.Utils;
using Newtonsoft.Json;
using ArenaZ.Manager;
using ArenaZ.Screens;

public class EmailVerification : MonoBehaviour
{
    [SerializeField] private InputField verificationCode;
    [SerializeField] private GameObject alertPopup;
    [SerializeField] private GameObject blankPopup;
    [SerializeField] private CharacterSelection characterSelection;
    [SerializeField] private GameObject emailVerificationbPanel;

    public void ResendBttnClick()
    {
        verificationCode.text = "";
        RestManager.ResendEmail(User.UserEmailId, OnResendMailComplete, OnResendEmailError);
    }

    public void VerifyBttnClick()
    {
        if(verificationCode.text == "")
        {
            blankPopup.SetActive(true);
        }
        else
        {
            RestManager.EmailVerification(verificationCode.text, User.UserEmailId, OnEmailVerificationCheckComplete, OnErrorEmailVerificationCheck);
        }
    }

    private void OnEmailVerificationCheckComplete(EmailVerifyCheckResponse a_Response)
    {
        Debug.Log($"Email Verification: {JsonConvert.SerializeObject(a_Response)}");
        if (a_Response != null)
        {
            Debug.Log("The status............................"+a_Response.Status);
            if (a_Response.Status.Equals("active"))
            {
                User.IPVerify = 1;
                verificationCode.text = "";
                characterSelection.OnClickArena(GameType.normal);
                emailVerificationbPanel.SetActive(false);
            }
            else
            {
                alertPopup.SetActive(true);
            }
        }
    }
    private void OnErrorEmailVerificationCheck(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On Email Verification: " + a_ErrorObj.Description);
        if (a_ErrorObj.Description.Equals("The email address and verify code you entered is incorrect. Please try again."))
        {
            alertPopup.SetActive(true);
        }
    }
    public void CloseBttnClick()
    {
        verificationCode.text = "";
    }
    private void OnResendMailComplete()
    {
 
    }
    private void OnResendEmailError(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On resend email: " + a_ErrorObj.Description);
    }
}
public class EmailVerifyCheckResponse
{
    [JsonProperty("status")]
    public string Status;
}
