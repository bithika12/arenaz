using RedApple;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ.GameMode
{
    public class MessagePanel : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private Sprite spriteChange1, spriteChange2;
        [SerializeField] private GameObject emojiPanel,closeButton;
        bool isOpen = false;

        private void Awake()
        {
            emojiPanel.SetActive(false);
            closeButton.SetActive(false);
        }

        public void OpenandCloseImogis()
        {
            if(isOpen == false)
            {
                OpenEmojiPanel();
            }
            else
            {
                CloseEmojiPanel();
            }
        }
        public void OpenEmojiPanel()
        {
            isOpen = true;
            buttonImage.sprite = spriteChange2;
            emojiPanel.SetActive(true);
            closeButton.SetActive(true);
        }
        public void CloseEmojiPanel()
        {
            isOpen = false;
            buttonImage.sprite = spriteChange1;
            emojiPanel.SetActive(false);
            closeButton.SetActive(false);
            StartCoroutine(enableButton());
        }

        public void SendMessages(string message)
        {
            SocketManager.Instance.SendUserMessage(message);
            buttonImage.GetComponent<Button>().interactable = false;           
            CloseEmojiPanel();
        }     
        
        IEnumerator enableButton()
        {
            yield return new WaitForSeconds(2f);
            Debug.LogError("buttonImage.GetComponent<Button>().interactable" + buttonImage.GetComponent<Button>().interactable);
            buttonImage.GetComponent<Button>().interactable = true;
        }
    }
}