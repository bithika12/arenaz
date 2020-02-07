using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using RedApple;
using RedApple.Utils;
using System;
using ArenaZ.Manager;

namespace ArenaZ
{
    public class MailBoxHandler : MonoBehaviour
    {
        [SerializeField] private MailBoxCell mailBoxCellPrototype;
        [SerializeField] private Transform container;

        [SerializeField] private MailDescription mailDescription;

        private List<MailBoxCell> activeMailBoxCells = new List<MailBoxCell>();

        private void OnEnable()
        {
            ClearWindow();
            RestManager.FetchNotifications(User.UserEmailId, OnRequestSuccess, OnRequestFailure);
            UIManager.Instance.HideScreen(Page.MailPopUp.ToString());
        }

        private void OnRequestSuccess(MessageDetails a_MessageDetails)
        {
            Debug.Log($"MessageDetails: {JsonConvert.SerializeObject(a_MessageDetails)}");
            if (a_MessageDetails != null && a_MessageDetails.messageDatas.Any())
            {
                a_MessageDetails.messageDatas.ForEach(x =>
                {
                    GameObject t_Go = Instantiate(mailBoxCellPrototype.gameObject, container);
                    t_Go.SetActive(true);
                    MailBoxCell t_Cell = t_Go.GetComponent<MailBoxCell>();
                    t_Cell.InitializeCell(x, OnSelectMessage);
                    activeMailBoxCells.Add(t_Cell);
                });
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError a_Error)
        {
            Debug.LogError($"MessageDetails: {a_Error.Description}");
        }

        private void OnSelectMessage(MessageData obj)
        {
            UIManager.Instance.ShowScreen(Page.MailPopUp.ToString());
            mailDescription.InitializeCell(obj, response =>
            {
                UIManager.Instance.HideScreen(Page.MailPopUp.ToString());
            });
        }

        private void ClearWindow()
        {
            if (activeMailBoxCells.Any())
            {
                activeMailBoxCells.ForEach(x => Destroy(x.gameObject));
                activeMailBoxCells.Clear();
            }
        }

        public void CloseWindow()
        {
            UIManager.Instance.HideScreen(Page.MailboxPanel.ToString());
        }
    }
}
