using System.Collections;
using UnityEngine;
using ArenaZ.Manager;
using System;
using DevCommons.Utility;
using Newtonsoft.Json;

namespace ArenaZ.LoginUser
{
	public class LoginHandler : MonoBehaviour
	{
        public static Action<string,string> userLogin;

        private void Start()
        {
            StartCoroutine(logInCheck());
        }

        private IEnumerator logInCheck()
        {
            //FileHandler.DeleteSaveFile(ConstantStrings.USER_SAVE_FILE_KEY);
            bool statue = false;
            UserData loginData = FileHandler.ReadFromFile<UserData>(ConstantStrings.USER_SAVE_FILE_KEY, out statue);
            if (statue)
            {
                yield return new WaitForSeconds(ConstantInteger.autoLoginWait);
                Debug.Log($"LoginData : {JsonConvert.SerializeObject(loginData)}");
                userLogin?.Invoke(loginData.Email, loginData.Password);
            }
            else
            {
                Debug.Log("Not Loggedin");
                UIManager.Instance.HideScreenImmediately(Page.TopAndBottomBarPanel.ToString());
                UIManager.Instance.ShowScreen(Page.AccountAccesOverlay.ToString());
                PlayerPrefs.SetInt(PlayerPrefsValue.Logout.ToString(), 1);
            }
        }

        //private IEnumerator logInCheck()
        //{
        //    string savedLoginID = UIManager.Instance.LoadDetails(PlayerprefsValue.LoginID.ToString());
        //    string savedPassword = UIManager.Instance.LoadDetails(PlayerprefsValue.Password.ToString());
        //    Debug.Log("Loaded String:  " + savedLoginID + "   " + savedPassword);
        //    if (savedLoginID != string.Empty && savedPassword != string.Empty)
        //    {
        //        yield return new WaitForSeconds(ConstantInteger.autoLoginWait);
        //        //UIManager.Instance.ShowScreen(Page.LogINOverlay.ToString());
        //        userLogin?.Invoke(savedLoginID, savedPassword);
        //        PlayerPrefs.SetInt(PlayerprefsValue.AutoLogin.ToString(), 1);
        //    }
        //    else
        //    {
        //        yield return null;
        //        UIManager.Instance.ShowScreen(Page.AccountAccesOverlay.ToString());
        //        PlayerPrefs.SetInt(PlayerprefsValue.Logout.ToString(), 1);
        //    }
        //}
    }
}
