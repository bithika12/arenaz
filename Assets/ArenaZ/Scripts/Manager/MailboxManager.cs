using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;

namespace ArenaZ.Mail
{
    [RequireComponent(typeof(UIScreen))]
    public class MailboxManager : RedAppleSingleton<MailboxManager>
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

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        private void GettingButtonReferences()
        {
            mailBoxCloseButton.onClick.AddListener(OnClickMailBoxClose);
            mailPopUpCloseButton.onClick.AddListener(OnClickMailBoxClose);
        }

        private void ReleaseButtonReferences()
        {
            mailBoxCloseButton.onClick.RemoveListener(OnClickMailBoxClose);
            mailPopUpCloseButton.onClick.RemoveListener(OnClickMailBoxClose);
        }

        private void OnClickMailBoxClose()
        {
            UIManager.Instance.HideScreen(Page.Mailbox.ToString());
        }

        private void OnClickMailPopUpClose()
        {
            UIManager.Instance.HideScreen(Page.MailPopUp.ToString());
        }

        //Private Variables
    }
}
