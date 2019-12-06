using ArenaZ.Manager;
using ArenaZ.SettingsManagement;
using RedApple;
using RedApple.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Arenaz.LogoutUser
{
    public class Logout : MonoBehaviour
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;


        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            yesButton.onClick.AddListener(OnClickLogout);
            noButton.onClick.AddListener(OnClickLogOutAlertClose);
        }

        private void RemoveListeners()
        {
            yesButton.onClick.AddListener(OnClickLogout);
            noButton.onClick.AddListener(OnClickLogOutAlertClose);
        }

        private void OnClickLogout()
        {
            RestManager.LogOutProfile(OnCompleteLogout, OnErrorLogout);
        }

        private void OnCompleteLogout()
        {
            Settings.Instance.AfterCompleteLogout();
        }

        private void OnErrorLogout(RestUtil.RestCallError obj)
        {
            Debug.Log(obj.Description);
        }

        private void OnClickLogOutAlertClose()
        {
            UIManager.Instance.HideScreenImmediately(Page.LogOutAlertOverlay.ToString());
        }
    }
}
