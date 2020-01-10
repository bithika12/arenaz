using System.Collections;
using UnityEngine;
using ArenaZ.Manager;
using System;

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
            string savedLoginID = UIManager.Instance.LoadDetails(PlayerprefsValue.LoginID.ToString());
            string savedPassword = UIManager.Instance.LoadDetails(PlayerprefsValue.Password.ToString());                
            Debug.Log("Loaded String:  " + savedLoginID + "   " + savedPassword);
            if (savedLoginID != string.Empty && savedPassword != string.Empty)
            {
                yield return new WaitForSeconds(ConstantInteger.autoLoginWait);
                //UIManager.Instance.ShowScreen(Page.LogINOverlay.ToString());
                userLogin?.Invoke(savedLoginID, savedPassword);
                PlayerPrefs.SetInt(PlayerprefsValue.AutoLogin.ToString(), 1);
            }
            else
            {
                yield return null;
                UIManager.Instance.ShowScreen(Page.AccountAccesOverlay.ToString());
                PlayerPrefs.SetInt(PlayerprefsValue.Logout.ToString(), 1);
            }
        }
    }
}
