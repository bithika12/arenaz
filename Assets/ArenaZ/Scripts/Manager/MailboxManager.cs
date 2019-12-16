using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;
using RedApple;

namespace ArenaZ.Mail
{
    public class MailboxManager : Singleton<MailboxManager>
    {
        //Private Variables
        [SerializeField] private GameObject MailPopup;
        [SerializeField] private Button mailBoxCloseButton;
        [SerializeField] private Button mailPopUpCloseButton;
        //Public Variables
        [HideInInspector]
        public string previoslyTouchedGOMailID;
        [HideInInspector]
        public GameObject previouslyTouchedMailShowButton;

        private void Start()
        {
            GettingButtonReferences();
        }

        private void OnEnable()
        {
            DeactivateScreenIfActivated();
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        public void DeactivateScreenIfActivated()
        {
            if (previouslyTouchedMailShowButton)
            {
                UIManager.Instance.HideScreenImmediately(Page.MailPopUp.ToString());               
                previouslyTouchedMailShowButton.SetActive(false);
            }
        }

        private void GettingButtonReferences()
        {
            mailBoxCloseButton.onClick.AddListener(OnClickMailBoxClose);
            mailPopUpCloseButton.onClick.AddListener(OnClickMailPopUpClose);
        }

        private void ReleaseButtonReferences()
        {
            mailBoxCloseButton.onClick.RemoveListener(OnClickMailBoxClose);
            mailPopUpCloseButton.onClick.RemoveListener(OnClickMailPopUpClose);
        }

        private void OnClickMailBoxClose()
        {
            UIManager.Instance.HideScreen(Page.MailboxPanel.ToString());
        }

        private void OnClickMailPopUpClose()
        {
            UIManager.Instance.HideScreenNormalWithAnim(Page.MailPopUp.ToString());
        }
    }
}
