using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaZ.Manager;

public class CheckInternetConnection : MonoBehaviour
{
    [SerializeField] private float internetCheckIntervalTime;
    private Coroutine showDialougue = null;

    private void Start()
    {
        showDialougue = StartCoroutine(showDialog());
    }

    private IEnumerator showDialog()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Debug.Log("EnablePopUp");
            enablePopUp();
            yield return new WaitForSeconds(internetCheckIntervalTime);
            if(showDialougue != null)
            {
                StopCoroutine(showDialougue);
            }
            showDialougue = StartCoroutine(showDialog());
        }
        else
        {
            //Debug.Log("DisablePopUp");
            disablePopUp();
            yield return new WaitForSeconds(internetCheckIntervalTime);
            if (showDialougue != null)
            {
                StopCoroutine(showDialougue);
            }
            showDialougue = StartCoroutine(showDialog());
        }
    }

    public void disablePopUp()
    {
        UIManager.Instance.HideScreen(Page.CheckInternetConnectionPanel.ToString());
    }

    private void enablePopUp()
    {
        UIManager.Instance.ShowScreen(Page.CheckInternetConnectionPanel.ToString());
    }
}
