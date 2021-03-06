using ArenaZ.Manager;
using ArenaZ.SettingsManagement;
using DevCommons.Utility;
using RedApple;
using RedApple.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ.LogoutUser
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
            FileHandler.DeleteSaveFile(ConstantStrings.USER_SAVE_FILE_KEY);
            FileHandler.ClearPlayerPrefs();

            OnClickLogOutAlertClose();
            Settings.Instance.AfterCompleteLogout();
        }

        private void OnErrorLogout(RestUtil.RestCallError obj)
        {
            Debug.Log(obj.Description);
            OnClickLogOutAlertClose();
        }

        private void OnClickLogOutAlertClose()
        {
            UIManager.Instance.HideScreenImmediately(Page.LogOutAlertOverlay.ToString());
        }
    }
}
