using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using RedApple.Utils;
using UnityEngine;
using UnityEngine.UI;
using ArenaZ.SettingsManagement;
using ArenaZ.Screens;


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
        RegularExpression checking = new RegularExpression();

        //Public Variables

        private void OnEnable()
        {
            GettingButtonReferences();
            Settings.Instance.inputFieldclear += ClearRegInputFieldData;
        }

        private void OnDisable()
        {
            ReleaseButtonReferences();
            Settings.Instance.inputFieldclear -= ClearRegInputFieldData;
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
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), Constants.wrongConfPassword, PopUpduration);
                return;
            }
            else if (regIf_UserPassword.text != regIf_UserConfPassword.text)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), Constants.wrongConfPassword, PopUpduration);
                return;
            }
            else if (regIf_UserPassword.text.Equals(regIf_UserConfPassword.text))
            {
                RestManager.UserRegistration(regIf_userEmail.text, regIf_UserName.text, regIf_UserPassword.text, OnCompleteRegistration, OnErrorRegistration);
                Debug.Log("Mail UserName password:   " + regIf_userEmail.text + regIf_UserName.text + regIf_UserPassword.text + regIf_UserConfPassword.text);
            }
        }

        private void OnCompleteRegistration(CreateAccount registeredProfile)
        {
            Debug.Log("Registered:  " + registeredProfile.UserName);
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), Constants.successFullyRegisterd, PopUpduration);
            OnClickRegisterPopUpClose();
            AccountAccess.Instance.TasksAfterLogin(registeredProfile.UserName,AccountAccessType.Registration);
        }

        private void OnErrorRegistration(RestUtil.RestCallError obj)
        {
            Debug.LogError("Error On Registration: " + obj.Description);
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), obj.Description, PopUpduration);
        }

        private string GetMessageWhenFaultCheckOnRegistration(string message, Checking type)
        {
            switch (type)
            {
                case Checking.Username:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return  Constants.userNameBlank;
                    }
                    if (checking.hasSpace.IsMatch(message))
                    {
                        return Constants.userNameContainedSpace;
                    }
                    if(!checking.hasMinimum3Chars.IsMatch(message))
                    {
                        return Constants.doesNotHaveMinThreeChar;
                    }
                    break;
                case Checking.EmailID:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return Constants.mailIsNotValid;
                    }
                    if (!checking.emailFormat.IsMatch(message))
                    {
                        return Constants.mailIsNotValid;
                    }
                    break;

                case Checking.Password:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return Constants.passwordIsNotValid;
                    }
                    if (!checking.hasNumber.IsMatch(message))
                    {
                        return Constants.doesNotHaveNumber;
                    }
                    if (!checking.hasUpperChar.IsMatch(message))
                    {
                        return Constants.doesNotHaveUpperCaseChar;
                    }
                    if (!checking.hasLowerChar.IsMatch(message))
                    {
                        return Constants.doesNotHaveLowerCaseChar;
                    }
                    if (!checking.hasspecialCharacter.IsMatch(message))
                    {
                        return Constants.doesNotHaveSpecialChar;
                    }
                    if (checking.hasSpace.IsMatch(message))
                    {
                        return Constants.passwordContainedSpace;
                    }
                    if (!checking.hasMinimum8Chars.IsMatch(message))
                    {
                        Debug.Log("Does Not Contain 8 Char");
                        return Constants.doesNotHaveMinEightChar;
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
