using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;

namespace ArenaZ.InfoAndRules
{
    public class InfoAndRules : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

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
            closeButton.onClick.AddListener(OnClickClose);
        }

        private void ReleaseButtonReferences()
        {
            closeButton.onClick.RemoveListener(OnClickClose);
        }

        private void OnClickClose()
        {
            UIManager.Instance.HideScreen(Page.InfoAndRulesForPlayerPanel.ToString());
            UIManager.Instance.ShowDefaultScreens();
        }
    }
}
