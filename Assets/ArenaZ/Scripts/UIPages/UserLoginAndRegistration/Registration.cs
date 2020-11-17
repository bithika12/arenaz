using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using RedApple.Utils;
using UnityEngine;
using UnityEngine.UI;
using ArenaZ.SettingsManagement;
using ArenaZ.Screens;
using DevCommons.Utility;
using Newtonsoft.Json;

namespace ArenaZ.RegistrationUser
{
    public class Registration : MonoBehaviour
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button registersBackButton;
        [SerializeField] private Button registerButton;

        [Header("Input Fields")]
        [Space(5)]
        [SerializeField] private InputField regIf_userEmail;
        [SerializeField] private InputField regIf_UserName;
        [SerializeField] private InputField regIf_UserPassword;
        [SerializeField] private InputField regIf_UserConfPassword;

        [Header("Integer")]
        [Space(5)]
        private float PopUpduration;
        
        [Header("Class Ref")]
        [Space(5)]
        [SerializeField] private MapHandler mapHandlerRef;
        [SerializeField] private Wallet.WalletHandler walletHandlerRef;

        RegularExpression checking = new RegularExpression();
        private string password;

        //Public Variables
        private void OnEnable()
        {
            GettingButtonReferences();
            Settings.inputFieldclear += ClearRegInputFieldData;
        }

        private void OnDisable()
        {
            ReleaseButtonReferences();
            Settings.inputFieldclear -= ClearRegInputFieldData;
        }

        #region Button_References
        private void GettingButtonReferences()
        {
            registersBackButton.onClick.AddListener(OnClickRegisterPopUpClose);
            registerButton.onClick.AddListener(OnClickUserRegistration);
        }

        private void ReleaseButtonReferences()
        {
            registersBackButton.onClick.RemoveListener(OnClickRegisterPopUpClose);
            registerButton.onClick.RemoveListener(OnClickUserRegistration);
        }
        #endregion

        public void ClearRegInputFieldData()
        {
            regIf_userEmail.text = null;
            regIf_UserName.text = null;
            regIf_UserPassword.text = null;
            regIf_UserConfPassword.text = null;
        }

        private void OnClickUserRegistration()
        {
            if (GetMessageWhenFaultCheckOnRegistration(regIf_userEmail.text, Checking.EmailID) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnRegistration(regIf_userEmail.text, Checking.EmailID), PopUpduration);
                return;
            }
            else if (GetMessageWhenFaultCheckOnRegistration(regIf_UserName.text, Checking.Username) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnRegistration(regIf_UserName.text, Checking.Username), PopUpduration);
                return;
            }
            else if (GetMessageWhenFaultCheckOnRegistration(regIf_UserPassword.text, Checking.Password) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), GetMessageWhenFaultCheckOnRegistration(regIf_UserPassword.text, Checking.Password), PopUpduration);
                return;
            }
            else if (string.IsNullOrWhiteSpace(regIf_UserConfPassword.text))
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), ConstantStrings.wrongConfPassword, PopUpduration);
                return;
            }
            else if (regIf_UserPassword.text != regIf_UserConfPassword.text)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), ConstantStrings.wrongConfPassword, PopUpduration);
                return;
            }
            else if (regIf_UserPassword.text.Equals(regIf_UserConfPassword.text))
            {
                password = regIf_UserConfPassword.text;
                RestManager.UserRegistration(regIf_userEmail.text, regIf_UserName.text, password, OnCompleteRegistration, OnErrorRegistration);
            }
        }

        private void OnCompleteRegistration(CreateAccount registeredProfile)
        {
            Debug.Log($"Registered: {JsonConvert.SerializeObject(registeredProfile)}");
            RestManager.AccessToken = registeredProfile.AccessToken;

            UserData loginData = new UserData(registeredProfile.Email, password, registeredProfile.AccessToken);
            FileHandler.SaveToFile<UserData>(loginData, ConstantStrings.USER_SAVE_FILE_KEY);

            storeUserData(registeredProfile);
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), ConstantStrings.successFullyRegisterd, PopUpduration);
            OnClickRegisterPopUpClose();
            AccountAccess.Instance.TasksAfterLogin(registeredProfile.UserName, AccountAccessType.Registration, true);
            Settings.Instance.setSelectedColorImage(EColor.DarkBlue.ToString());

            mapHandlerRef.GetGames();
            walletHandlerRef.GetWalletDetails();
        }

        private void storeUserData(CreateAccount userDetails)
        {
            User.UserName = userDetails.UserName;
            User.UserId = userDetails.UserId;
            User.UserEmailId = userDetails.Email;
            User.UserAccessToken = userDetails.AccessToken;
            User.UserCoin = userDetails.UserCoin;
            User.UserCup = userDetails.UserCup;
        }

        private void OnErrorRegistration(RestUtil.RestCallError obj)
        {
            Debug.LogError("Error On Registration: " + obj.Description);
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), obj.Description, PopUpduration);
        }

        private bool withinLimit(string message, int limit)
        {
            if (message.Length <= limit)
                return true;
            else
                return false;
        }

        private string GetMessageWhenFaultCheckOnRegistration(string message, Checking type)
        {
            switch (type)
            {
                case Checking.Username:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return  ConstantStrings.userNameBlank;
                    }
                    if (checking.hasSpace.IsMatch(message))
                    {
                        return ConstantStrings.userNameContainedSpace;
                    }
                    if(!checking.hasMinimum3Chars.IsMatch(message))
                    {
                        return ConstantStrings.doesNotHaveMinThreeChar;
                    }
                    if (checking.hasspecialCharacter.IsMatch(message))
                    {
                        return ConstantStrings.userNameSpecialCharacter;
                    }
                    if (!withinLimit(message, 13))
                    {
                        return ConstantStrings.userNameLimitOverflow;
                    }
                    break;
                case Checking.EmailID:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return ConstantStrings.mailIsNotValid;
                    }
                    if (!checking.emailFormat.IsMatch(message))
                    {
                        return ConstantStrings.mailIsNotValid;
                    }
                    break;

                case Checking.Password:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return ConstantStrings.passwordIsNotValid;
                    }
                    if (!checking.hasNumber.IsMatch(message))
                    {
                        return ConstantStrings.doesNotHaveNumber;
                    }
                    if (!checking.hasUpperChar.IsMatch(message))
                    {
                        return ConstantStrings.doesNotHaveUpperCaseChar;
                    }
                    if (!checking.hasLowerChar.IsMatch(message))
                    {
                        return ConstantStrings.doesNotHaveLowerCaseChar;
                    }
                    //if (!checking.hasspecialCharacter.IsMatch(message))
                    //{
                    //    return ConstantStrings.doesNotHaveSpecialChar;
                    //}
                    if (checking.hasSpace.IsMatch(message))
                    {
                        return ConstantStrings.passwordContainedSpace;
                    }
                    if (!checking.hasMinimum8Chars.IsMatch(message))
                    {
                        Debug.Log("Does Not Contain 8 Char");
                        return ConstantStrings.doesNotHaveMinEightChar;
                    }
                    break;
                default:
                    Debug.LogError("Please Choose Any Checking Type");
                    break;
            }
            return null;
        }


        private void OnClickRegisterPopUpClose()
        {
            ClearRegInputFieldData();
            UIManager.Instance.HideScreen(Page.RegistrationOverlay.ToString());
        }
    }
}

[System.Serializable]
public class UserData
{
    public UserData()
    {
    }

    public UserData(string a_Email, string a_Password, string a_AccessToken)
    {
        Email = a_Email;
        Password = a_Password;
        AccessToken = a_AccessToken;
    }

    public string Email { get; set; }
    public string Password { get; set; }
    public string AccessToken { get; set; }
}