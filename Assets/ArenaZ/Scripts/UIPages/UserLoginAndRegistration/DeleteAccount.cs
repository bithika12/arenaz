using ArenaZ.Manager;
using ArenaZ.SettingsManagement;
using DevCommons.Utility;
using RedApple;
using RedApple.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ
{
    public class DeleteAccount : MonoBehaviour
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
            yesButton.onClick.AddListener(onClickDelete);
            noButton.onClick.AddListener(onClickDeleteAlertClose);
        }

        private void RemoveListeners()
        {
            yesButton.onClick.AddListener(onClickDelete);
            noButton.onClick.AddListener(onClickDeleteAlertClose);
        }

        private void onClickDelete()
        {
            RestManager.DeleteAccount(OnCompleteDelete, OnErrorDelete);
        }

        private void OnCompleteDelete()
        {
            FileHandler.DeleteSaveFile(ConstantStrings.USER_SAVE_FILE_KEY);
            FileHandler.ClearPlayerPrefs();

            onClickDeleteAlertClose();
            Settings.Instance.AfterCompleteLogout();
        }

        private void OnErrorDelete(RestUtil.RestCallError obj)
        {
            Debug.Log(obj.Description);
            onClickDeleteAlertClose();
        }

        private void onClickDeleteAlertClose()
        {
            UIManager.Instance.HideScreenImmediately(Page.DeleteAccountAlertOverlay.ToString());
        }
    }
}
