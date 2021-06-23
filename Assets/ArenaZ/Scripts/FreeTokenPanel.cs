using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Screens;
using ArenaZ.GameMode;
using RedApple.Utils;
using Newtonsoft.Json;
using ArenaZ.Manager;
using RedApple;

public class FreeTokenPanel : MonoBehaviour
{
    [SerializeField] private GameObject freeTokenObj;
    [SerializeField] private GameObject[] tickmark;
    [SerializeField] private Button claimBttn;
    [SerializeField] private Sprite[] claimBttnSprite;
    [SerializeField] private GameObject[] gameCountTypeObj;
    [SerializeField] private ShootingRange shootingRange;
    [SerializeField] private Sprite[] chestOpenBox;
    [SerializeField] private GameObject chestImageAnim;
    [SerializeField] private GameObject defaultChestImage;

    public void HideFreeTokenPanel()
    {
        freeTokenObj.SetActive(false);
    }

    private void PlayChestAnimation()
    {
        defaultChestImage.SetActive(false);
        chestImageAnim.SetActive(true);
        StopCoroutine("ChestAnimation");
        StartCoroutine("ChestAnimation");
    }
    private IEnumerator ChestAnimation()
    {
        for (int i = 0; i < chestOpenBox.Length; i++)
        {
            yield return new WaitForSeconds(0.1f);
            chestImageAnim.GetComponent<Image>().sprite = chestOpenBox[i];
        }
        yield return new WaitForSeconds(0.5f);
        defaultChestImage.SetActive(true);
        chestImageAnim.SetActive(false);
        freeTokenObj.SetActive(false);
    }
    public void EnableFreeTokenPanel()
    {
        EnableTickMark(User.UserGameCount);
        ShowPanel(User.GameCountType); 
    }
    private void EnableTickMark(int _tickMarkCount)
    {
        for(int i = 0;i<tickmark.Length;i++)
        {
            if(i< _tickMarkCount)
            {
                tickmark[i].SetActive(true);
            }
            else
            {
                tickmark[i].SetActive(false);
            }
        }
        if(User.GameCountType == 5)
        {
            if(_tickMarkCount >= 5)
            {
                claimBttn.interactable = true;
                claimBttn.GetComponent<Image>().sprite = claimBttnSprite[0];
            }
            else
            {
                claimBttn.interactable = true;
                claimBttn.GetComponent<Image>().sprite = claimBttnSprite[1];
            }
        }
        if(User.GameCountType == 10)
        {
            if (_tickMarkCount >= 10)
            {
                claimBttn.interactable = true;
                claimBttn.GetComponent<Image>().sprite = claimBttnSprite[0];
            }
            else
            {
                claimBttn.interactable = true;
                claimBttn.GetComponent<Image>().sprite = claimBttnSprite[1];
            }
        }
    }
    public void ClaimBttnClick()
    {
        RestManager.AddUserFreeCoinIncentive(User.UserName, "1", OnAddUserCoin, OnAddUserCoinError);
        PlayChestAnimation();
    }
    public void OnAddUserCoin()
    {
        shootingRange.Refresh();
        //freeTokenObj.SetActive(false);
    }
    public void OnAddUserCoinError(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On resend email: " + a_ErrorObj.Description);
    }
    private void ShowPanel(int _gameCountType)
    {
        if (_gameCountType == 5)
        {
            gameCountTypeObj[0].SetActive(true);
            gameCountTypeObj[1].SetActive(false);
        }
        else
        {
            gameCountTypeObj[0].SetActive(true);
            gameCountTypeObj[1].SetActive(true);
        }
    }
}
