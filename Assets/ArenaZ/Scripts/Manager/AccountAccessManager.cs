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
    [RequireComponent(typeof(UIScreen))]
    public class AccountAccessManager : RedAppleSingleton<AccountAccessManager>
    {
        #region Constants
        //Constants
        private const string successFullyRegisterd = "User registered successfully!!";
        private const string successFullyLoggedOut = "User Loggedout SuccessFully!!";
        private const string emailAlreadyExists = "Email Already Exists";
        private const string noInternet = "Please Check Your Internet Connection!!";
        private const string successFullyLoggedIn = "User login successfully!!";
        private const string doesNotHaveNumber = "Must Contain One Number!!";
        private const string doesNotHaveChar = "Must Contain Characters";
        private const string mailIsNotValid = "Is Not In A Proper Format Ex: abcd@a.com";
        private const string doesNotContainAtTheRate = "Must Contain @ Symbol";
        private const string doesNotHaveSpecialChar = "Must Contain One Special Character!!";
        private const string doesNotHaveUpperCaseChar = "MustContain One Upper Case Character!!";
        private const string doesNotHaveLowerCaseChar = "Must Contain One Lower Case Character!!";
        private const string doesNotHaveMinEightChar = "Must contain 8 Characters!!";
        private const string containedSpace = "Doesn't Contain Any Space";
        private const string passwordIsNull = "Password should be like - Ex : Abc@1234";
        private const string isNull = "Cannot Be Blank";
        private const string wrongConfPassword = "PassWord Doesn't Match!!";
        private const string defaultUserName = "UserName";
        private const string login = "Log In";
        private const string logout = "Log Out";
        #endregion
        //Private Variables
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button firstRegisterButton;
        [SerializeField] private Button firstLoginButton;
        [SerializeField] private Button registersBackButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button forgotButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button closeButton;

        [Header("Registration Input Fields")]
        [Space(5)]
        [SerializeField] private InputField regIf_userEmail;
        [SerializeField] private InputField regIf_UserName;
        [SerializeField] private InputField regIf_UserPassword;
        [SerializeField] private InputField regIf_UserConfPassword;

        [Header("LogIn Input Fields")]
        [Space(5)]
        [SerializeField] private InputField logInIf_userEmail;
        [SerializeField] private InputField logInIf_password;

        [Header("Integer")]
        [Space(5)]
        [SerializeField] private int PopUpduration;

        private Checking checkingType;
        //public Variables
        public string CountryId { get; private set; }
        public string UserName { get; private set; }
        public string LogInAndLogOutButtonName { get; private set; }
        public Sprite CountrySprite { get; private set; }


        protected void Start()
        {
            GetCountryDetailsOnStart();
            GettingButtonReferences();
            LogInAndLogOutButtonName = login;
        }
        private void OnEnable()
        {
            OnClickAccountAccessPanelOpen();
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }
        #region Country_Details
        private void GetCountryDetailsOnStart()
        {
            RestManager.GetCountryDetails(OnCompletionOfCountryDetailsFetch, OnErrorCountryDetailsFetch);
        }
        private void OnCompletionOfCountryDetailsFetch(CountryDetails details)
        {
            Debug.Log("The Country Code Is:     "+details.Country_code);
            CountryId = details.Country_code;
            CountrySprite = UIManager.Instance.GetCorrespondingCountrySprite(details.Country_code.ToLower());
        }
        private void OnErrorCountryDetailsFetch(RestUtil.RestCallError obj)
        {
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), noInternet, PopUpduration);
        }
        #endregion

        #region Button_References
        private void GettingButtonReferences()
        {
            firstRegisterButton.onClick.AddListener(OnClickFirstRegisterButton);
            firstLoginButton.onClick.AddListener(OnClickFirstLoginButton);
            registersBackButton.onClick.AddListener(OnClickRegisterPopUpClose);
            registerButton.onClick.AddListener(OnClickUserRegistration);
            forgotButton.onClick.AddListener(OnClickLoginPopUpClose);
            loginButton.onClick.AddListener(OnClickUserLogin);
            closeButton.onClick.AddListener(OnClickAccountAccessPanelClose);
        }

        private void ReleaseButtonReferences()
        {
            firstRegisterButton.onClick.RemoveListener(OnClickFirstRegisterButton);
            firstLoginButton.onClick.RemoveListener(OnClickFirstLoginButton);
            registersBackButton.onClick.RemoveListener(OnClickRegisterPopUpClose);
            registerButton.onClick.RemoveListener(OnClickUserRegistration);
            forgotButton.onClick.RemoveListener(OnClickLoginPopUpClose);
            loginButton.onClick.RemoveListener(OnClickUserLogin);
            closeButton.onClick.RemoveListener(OnClickAccountAccessPanelClose);
        }
        #endregion

        #region Registration_AND_Login

        private void ClearRegInputFieldData()
        {
            regIf_userEmail.text = null;
            regIf_UserName.text = null;
            regIf_UserPassword.text = null;
            regIf_UserConfPassword.text = null;
        }

        private void ClearLoginInputFieldData()
        {
            logInIf_userEmail.text = null;
            logInIf_password.text = null;
        }

        private void OnClickUserRegistration()
        {
            if (RegistrationFaultCheck(regIf_userEmail.text, Checking.EmailID) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), RegistrationFaultCheck(regIf_userEmail.text, Checking.EmailID), PopUpduration);
                return;
            }
            else if (RegistrationFaultCheck(regIf_UserName.text, Checking.Username) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), RegistrationFaultCheck(regIf_UserName.text, Checking.Username), PopUpduration);
                return;
            }
            else if (RegistrationFaultCheck(regIf_UserPassword.text, Checking.Password) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), RegistrationFaultCheck(regIf_UserPassword.text, Checking.Password), PopUpduration);
                return;
            }
            else  if (string.IsNullOrWhiteSpace(regIf_UserConfPassword.text))
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), wrongConfPassword, PopUpduration);
                return;
            }
            else if (regIf_UserPassword.text != regIf_UserConfPassword.text)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), wrongConfPassword, PopUpduration);
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
            Debug.Log("Registered:  " + registeredProfile.UserId);
            OnClickRegisterPopUpClose();
            ClearRegInputFieldData();
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), successFullyRegisterd, PopUpduration);
        }
       
        private void OnErrorRegistration(RestUtil.RestCallError obj)
        {
            Debug.LogError("Error On Registration: "+obj.Description);
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), emailAlreadyExists, PopUpduration);
        }

       
        private void OnClickUserLogin()
        {
            if (LoginFaultCheck(logInIf_userEmail.text, Checking.EmailID) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), LoginFaultCheck(logInIf_userEmail.text, Checking.EmailID), PopUpduration);
                return;
            }
            else if (LoginFaultCheck(logInIf_password.text, Checking.Password) != null)
            {
                UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), LoginFaultCheck(logInIf_password.text, Checking.Password), PopUpduration);
                return;
            }
            else
            {
                RestManager.LoginProfile(logInIf_userEmail.text, logInIf_password.text, OnCompleteLogin, OnErrorLogin);
            }
        }

        private void OnCompleteLogin(UserLogin loggedinProfile)
        {
            Debug.Log("Logged In Profile:  " + loggedinProfile.UserId + regIf_userEmail.text);
            UserName = loggedinProfile.UserName;
            LogInAndLogOutButtonName = logout;
            UIManager.Instance.setUserName?.Invoke(UserName);
            PlayerPrefs.SetInt("Logout", 0);
            ClearLoginInputFieldData();
            OnClickLoginPopUpClose();
            OpenCharacterUI();
        }

        private void OnErrorLogin(RestUtil.RestCallError restError)
        {
            Debug.LogError(restError.Description);
            UIManager.Instance.ShowPopWithText(Page.PopUpTextAccountAccess.ToString(), restError.Description, PopUpduration);
        }

        private string RegistrationFaultCheck(string message,Checking type)
        {
            RegularExp checking = new RegularExp();
            switch (type)
            {
                case Checking.Username:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return Checking.Username.ToString() + " " + isNull;
                    }
                    if (!checking.hasNumber.IsMatch(message))
                    {
                        return Checking.Username.ToString() + " " + doesNotHaveNumber;
                    }
                    if(!checking.hasCapAndSmall.IsMatch(message))
                    {
                        return Checking.Username.ToString() + " " + doesNotHaveChar;
                    }
                    if(checking.hasSpace.IsMatch(message))
                    {
                        return Checking.Username.ToString() + " " + containedSpace;
                    }
                    break;
                case Checking.EmailID:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return Checking.EmailID.ToString() + " " + isNull;
                    }
                    if (!checking.emailFormat.IsMatch(message))
                    {
                        return Checking.EmailID.ToString() + " " + mailIsNotValid;
                    }
                    break;
                    
                case Checking.Password:

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        return Checking.Password.ToString() + " " + passwordIsNull;
                    }
                    if (!checking.hasNumber.IsMatch(message))
                    {
                        return Checking.Password.ToString() + " " + doesNotHaveNumber;
                    }
                    if (!checking.hasUpperChar.IsMatch(message))
                    {
                        return Checking.Password.ToString() + " " + doesNotHaveUpperCaseChar;
                    }
                    if(!checking.hasLowerChar.IsMatch(message))
                    {
                        return Checking.Password.ToString() + " " + doesNotHaveLowerCaseChar;
                    }
                    if (!checking.hasspecialCharacter.IsMatch(message))
                    {
                        return Checking.Password.ToString() + " " + doesNotHaveSpecialChar;
                    }
                    if (checking.hasSpace.IsMatch(message))
                    {
                        return Checking.Password.ToString() + " " + containedSpace;
                    }
                    if(!checking.hasMinimum8Chars.IsMatch(message))
                    {
                        return Checking.Password.ToString() + " " + doesNotHaveMinEightChar;
                    }
                    break;
                    default:
                    Debug.LogError("Please Choose Any Checking Type");
                    break;
            }
            return null;
        }

        private string LoginFaultCheck(string message, Checking type)
        {
            RegularExp regExp = new RegularExp();
            switch (type)
            {
                case Checking.EmailID:
                    if(string.IsNullOrWhiteSpace(message))
                    {
                        return Checking.EmailID.ToString() + " " + isNull;
                    }
                    break;

                case Checking.Password:
                    if(string.IsNullOrWhiteSpace(message))
                    {
                        return Checking.Password.ToString() + " " + isNull;
                    }
                    break;
            }
            return null;
        }

        public void OnClickLogout()
        {
            RestManager.LogOutProfile(OnCompleteLogout, OnErrorLogout);
        }

        private void OnCompleteLogout(UserLogin obj)
        {
            Debug.Log("User Logged Out SuccessFully");
            LogInAndLogOutButtonName = login;
           // UIManager.Instance.ShowPopWithText(Page.PopUpTextSettings.ToString(), successFullyLoggedOut, PopUpduration);
            UIManager.Instance.showProfilePic?.Invoke((CharacterSelection.Instance.names[0]));
            UIManager.Instance.setUserName?.Invoke(defaultUserName);
        }

        private void OnErrorLogout(RestUtil.RestCallError obj)
        {
            Debug.Log(obj.Description);
        }

        #endregion

        #region UI_Functionalities

        private void OpenCharacterUI()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetails.ToString());
            UIManager.Instance.ScreenShowAndHide(Page.CharacterSelection.ToString(), Hide.none);
        }

        private void OnClickFirstRegisterButton()
        {
            UIManager.Instance.ScreenShowAndHide(Page.RegistrationOverlay.ToString(),Hide.none);
        }

        private void OnClickFirstLoginButton()
        {
            UIManager.Instance.ScreenShowAndHide(Page.LogINOverlay.ToString(), Hide.none);
        }

        private void OnClickRegisterPopUpClose()
        {
            UIManager.Instance.HideScreen(Page.RegistrationOverlay.ToString());
        }
        
        private void OnClickLoginPopUpClose()
        {
            UIManager.Instance.HideScreen(Page.LogINOverlay.ToString());
        }

        private void OnClickAccountAccessPanelOpen()
        {
            UIManager.Instance.ScreenShowAndHide(Page.AcoountAccesOverlay.ToString(),Hide.none);
        }

        private void OnClickAccountAccessPanelClose()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetails.ToString());
        }
        #endregion
    }
}
[Serializable]
public class RegularExp
{
    public Regex hasNumber = new Regex(@"[0-9]+");
    public Regex hasUpperChar = new Regex(@"[A-Z]+");
    public Regex hasLowerChar = new Regex(@"[a-z]+");
    public Regex hasCapAndSmall = new Regex(@"[a-zA-Z]+");
    public Regex hasSpace = new Regex(@"\s");
    // public Regex emailFormat = new Regex(@"^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+\.)+[a-z]{2,5}$");
    public Regex emailFormat = new Regex(@"^[a-z0-9._]+@([-a-z0-9]+\.)+[a-z]{2,5}$");
    public Regex hasMinimum8Chars = new Regex(@".{8,}");
    public Regex hasAtTheRate = new Regex(@"\@");
    public Regex hasspecialCharacter = new Regex(@"([<>\?\*\\\""/\|@&'#!])+");
}
