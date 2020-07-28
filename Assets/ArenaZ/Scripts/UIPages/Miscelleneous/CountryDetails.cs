using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using RedApple.Utils;
using UnityEngine;
using UnityEngine.UI;

public class CountryDetails : MonoBehaviour
{
    [SerializeField]
    private Image countryButtonImage;
    [SerializeField]
    private Text countryButtonText;
    [SerializeField]
    private int popUpCloseDuration;

    private float firstFetchTime;
    private float updateTime;
    private bool fetchedDetails=false;

    private void Start()
    {       
        GetCountryDetails();
    }

    private void Update()
    {               
        if(fetchedDetails)
        {
            return;
        }
        else
        {
            updateTime = Time.deltaTime;
            if (updateTime - firstFetchTime>10)
            {
                GetCountryDetails();
            }
        }
    }

    private void GetCountryDetails()
    {
        firstFetchTime = Time.deltaTime;
        RestManager.GetCountryDetails(OnCompletionOfCountryDetailsFetch, OnErrorCountryDetailsFetch);
    }

    private void OnCompletionOfCountryDetailsFetch(CountryData details)
    {
        Debug.Log("The Country Code Is:     " + details.Country_code);
        if (!countryButtonImage.enabled)
        {
            countryButtonImage.enabled = true;
        }
        countryButtonImage.sprite = UIManager.Instance.GetCorrespondingCountrySprite(details.Country_code.ToLower());
        countryButtonText.text = details.Country_code;
        fetchedDetails = true;
    }

    private void OnErrorCountryDetailsFetch(RestUtil.RestCallError obj)
    {
        UIManager.Instance.ShowPopWithText(Page.PopUpTextSettings.ToString(), ConstantStrings.noInternet, popUpCloseDuration);
    }
}
