using System;
using System.Collections;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ.Screens
{
    public class UiTrainingPopup : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text messageText;

        [SerializeField] private Button proccedBtn;

        public void Show(string title, string message, Action a_OnProcced)
        {
            StartCoroutine(ShowPopUp(title, message, a_OnProcced));
        }

        private IEnumerator ShowPopUp(string title, string message, Action a_OnProcced)
        {
            yield return new WaitForSeconds(2.5f);
            UIManager.Instance.ShowScreen(Page.TrainingPopupPanel.ToString(), Hide.none);

            titleText.text = title;
            messageText.text = message;

            proccedBtn.onClick.AddListener(() =>
            {
                a_OnProcced?.Invoke();
                OnReset();
            });
        }

        private void OnReset()
        {
            proccedBtn.onClick.RemoveAllListeners();
            UIManager.Instance.HideScreenImmediately(Page.TrainingPopupPanel.ToString());
        }
    }
}