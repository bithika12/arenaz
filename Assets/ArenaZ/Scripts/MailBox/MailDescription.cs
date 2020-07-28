using DevCommons.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ
{
    public class MailDescription : Cell<MessageData>
    {
        [SerializeField] private Text title;
        [SerializeField] private Text description;

        public override void InitializeCell(MessageData cellData, Action<MessageData> onClickCallback = null)
        {
            base.InitializeCell(cellData, onClickCallback);
            description.text = cellData.Description;
        }
    }
}