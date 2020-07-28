using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacebookLogin : MonoBehaviour
{
    List<string> perms = new List<string>() { "public_profile", "email" };
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            Debug.Log("Trying to initialize the sdk");
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            Debug.Log("Activating the app");
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }


    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("Initialized");
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Debug.Log("Game Paused");
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            Debug.Log("Game Resumed");
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }


    public void Login()
    {
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }    

    private void AuthCallback(ILoginResult result)
    {
        Debug.Log("Logging to facebook");
        if (FB.IsLoggedIn)
        {
            Debug.Log("Login Successful");
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
}
