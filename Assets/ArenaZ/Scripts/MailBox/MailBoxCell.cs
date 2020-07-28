using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevCommons.UI;
using System;
using UnityEngine.UI;
using System.Linq;
using DevCommons.Utility;
using ArenaZ;
using DG.Tweening;

namespace ArenaZ
{
    public class MailBoxCell : Cell<MessageData>
    {
        [SerializeField] private Image falshingImage;
        [SerializeField] private Image icon;
        [SerializeField] private Text date;
        [SerializeField] private Text message;

        [SerializeField] private List<Sprite> icons = new List<Sprite>();

        private Sequence tweenSequence;
        private bool highlight = false;

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

                if (cellData.ReadStatus == 0)
                {
                    highlight = true;
                    falshingImage.color = new Color(1, 1, 1, 0);
                    highlightMessage();
                }
            }
        }

        protected override void OnClick()
        {
            stopHighlightingMessage();
            m_OnClickCallback?.Invoke(m_CellData);
        }

        private void highlightMessage()
        {
            if (falshingImage != null && highlight)
            {
                tweenSequence = DOTween.Sequence();
                tweenSequence.AppendInterval(1.0f);
                tweenSequence.Append(falshingImage.DOFade(0.25f, 1.0f).SetLoops(2, LoopType.Yoyo));
                tweenSequence.OnComplete(() => highlightMessage());
                tweenSequence.Play();
            }
        }

        private void stopHighlightingMessage()
        {
            highlight = false;
            if (tweenSequence != null)
                tweenSequence.Kill();
            falshingImage.color = new Color(1, 1, 1, 0);
        }
    }
}
