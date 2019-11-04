using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ArenaZ.Manager;
using ArenaZ.Screens;
using System;
using UnityEngine.Events;

namespace ArenaZ.AccountAccess
{
    [RequireComponent(typeof(UIScreen))]
    public class AccountAccessManager : MonoBehaviour
    {
        //Private Variables
        [Header("Interactive Elements")]
        [SerializeField]
        private Button preRegisterButton;
        [SerializeField]
        private Button preLoginButton;
        [SerializeField]
        private Button registersBackButton;
        [SerializeField]
        private Button registerButton;
        [SerializeField]
        private Button forgotButton;
        [SerializeField]
        private Button loginButton;
        [SerializeField]
        private Button closeButton;
        //public Variables


        private void Start()
        {
            GettingButtonReferences();
        }
        private void OnEnable()
        {
            OpenAccountAccessPopUp();
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        private void GettingButtonReferences()
        {
            preRegisterButton.onClick.AddListener(OpenRegisterPopUp);
            preLoginButton.onClick.AddListener(OpenLogInPopUp);
            registersBackButton.onClick.AddListener(CloseRegisterPopUp);
            registerButton.onClick.AddListener(OpenRegisterPopUp);
            forgotButton.onClick.AddListener(CloseLogInPopUp);
            loginButton.onClick.AddListener(OpenCharacterUI);
            closeButton.onClick.AddListener(CloseAccountAccessPopUp);
        }

        private void ReleaseButtonReferences()
        {
            preRegisterButton.onClick.RemoveListener(OpenRegisterPopUp);
            preLoginButton.onClick.RemoveListener(OpenLogInPopUp);
            registersBackButton.onClick.RemoveListener(CloseRegisterPopUp);
            registerButton.onClick.RemoveListener(OpenAccountAccessPopUp);
            forgotButton.onClick.RemoveListener(CloseLogInPopUp);
            loginButton.onClick.RemoveListener(OpenCharacterUI);
            closeButton.onClick.RemoveListener(CloseAccountAccessPopUp);
        }

        private void OpenCharacterUI()
        {
            UIManager.Instance.HideScreen(Page.AccountAccessDetails.ToString());
            UIManager.Instance.ShowScreen(Page.CharacterSelection.ToString(), Hide.previous);
        }

        private void OpenRegisterPopUp()
        {
            UIManager.Instance.ShowScreen(Page.RegistrationOverlay.ToString(),Hide.none);
        }

        private void CloseRegisterPopUp()
        {
            UIManager.Instance.HideScreen(Page.RegistrationOverlay.ToString());
        }

        private void OpenLogInPopUp()
        {
            UIManager.Instance.ShowScreen(Page.LogINOverlay.ToString(),Hide.none);
        }

        private void CloseLogInPopUp()
        {
            UIManager.Instance.HideScreen(Page.LogINOverlay.ToString());
        }

        private void OpenAccountAccessPopUp()
        {
            UIManager.Instance.ShowScreen(Page.AcoountAccesOverlay.ToString(),Hide.none);
        }

        private void CloseAccountAccessPopUp()
        {
            UIManager.Instance.HideScreen(Page.AcoountAccesOverlay.ToString());
        }
    }
}
