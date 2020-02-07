using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevCommons.UI;
using System;
using UnityEngine.UI;
using System.Linq;
using DevCommons.Utility;
using ArenaZ;

namespace ArenaZ
{
    public class MailBoxCell : Cell<MessageData>
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text date;
        [SerializeField] private Text message;

        [SerializeField] private List<Sprite> icons = new List<Sprite>();

        public override void InitializeCell(MessageData cellData, Action<MessageData> onClickCallback = null)
        {
            base.InitializeCell(cellData, onClickCallback);
            if (cellData != null)
            {
                if (cellData.Type.Contains("message"))
                {
                    icon.sprite = icons.ElementAt(0);
                    m_CustomButton.gameObject.SetActive(true);
                }
                else if (cellData.Type.Contains("notification"))
                {
                    icon.sprite = icons.ElementAt(1);
                    m_CustomButton.gameObject.SetActive(false);
                }
                icon.SetNativeSize();

                string[] dateArr = cellData.CretaedTime.Split(' ');
                date.text = dateArr[0];

                message.text = cellData.Message;
            }
        }
    }
}
