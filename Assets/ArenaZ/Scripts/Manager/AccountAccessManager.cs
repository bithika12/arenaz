using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ArenaZ.Manager;
using ArenaZ.Screens;
using RedApple;
using RedApple.Utils;
using RedApple.Api.Data;
using TMPro;

namespace ArenaZ.AccountAccess
{
    [RequireComponent(typeof(UIScreen))]
    public class AccountAccessManager : MonoBehaviour
    {
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

        [Header("Text Mesh Pro")]
        [Space(5)]
        [SerializeField] private TextMeshProUGUI popUpText;
        private const string SuccessFullyRegisterd = "User register successfully!!";
        private const string SuccessFullyLoggedIn = "User login successfully!!";
        //public Variables


        private void Start()
        {
            GettingButtonReferences();
        }
        private void OnEnable()
        {
            OnClickAccountAccessPanelOpen();
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

        private void OnClickUserRegistration()
        {
            RestManager.UserRegistration(regIf_userEmail.text, regIf_UserName.text, regIf_UserPassword.text, regIf_UserConfPassword.text, OnCompleteRegistration, OnErrorRegistration);
        }

        private void OnCompleteRegistration(CreateAccount registeredProfile)
        {
            Debug.Log("Registered:  " + registeredProfile.UserId);
            OnClickRegisterPopUpClose();
            StartCoroutine(ShowAndHidePopUpText(SuccessFullyRegisterd));
        }
        private void OnErrorRegistration(RestUtil.RestCallError obj)
        {
            Debug.LogError(obj.Error);
            StartCoroutine(ShowAndHidePopUpText(obj.Description));
        }

        private void OnClickUserLogin()
        {
            RestManager.LoginProfile(logInIf_userEmail.text, logInIf_password.text, OnCompleteLogin, OnErrorLogin);
        }

        private void OnCompleteLogin(UserLogin loggedinProfile)
        {
            Debug.Log("Logged In Profile:  " + loggedinProfile.UserId + regIf_userEmail.text);
            StartCoroutine(ShowAndHidePopUpText(SuccessFullyLoggedIn));
            OpenCharacterUI();
        }

        private void OnErrorLogin(RestUtil.RestCallError obj)
        {
            Debug.LogError(obj.Error);
            StartCoroutine(ShowAndHidePopUpText(obj.Description));
        }

        #endregion

        #region UI_Functionalities
        private IEnumerator ShowAndHidePopUpText(string message)
        {
            popUpText.text = message;
            UIManager.Instance.ShowScreen(Page.PopUpText.ToString(), Hide.none);
            yield return new WaitForSeconds(.5f);
            UIManager.Instance.HideScreen(Page.PopUpText.ToString());
        }

        private void OpenCharacterUI()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetails.ToString());
            UIManager.Instance.ShowScreen(Page.CharacterSelection.ToString(), Hide.previous);
        }

        private void OnClickFirstRegisterButton()
        {
            UIManager.Instance.ShowScreen(Page.RegistrationOverlay.ToString(),Hide.none);
        }

        private void OnClickFirstLoginButton()
        {
            UIManager.Instance.ShowScreen(Page.LogINOverlay.ToString(), Hide.none);
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
            UIManager.Instance.ShowScreen(Page.AcoountAccesOverlay.ToString(),Hide.none);
        }

        private void OnClickAccountAccessPanelClose()
        {
            UIManager.Instance.HideScreen(Page.AcoountAccesOverlay.ToString());
        }
        #endregion
    }
}
