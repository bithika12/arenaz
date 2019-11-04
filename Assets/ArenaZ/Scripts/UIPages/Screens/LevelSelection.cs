using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button shootingRangeButton;
    [SerializeField] private Button speedRaceButton;
    [SerializeField] private Button bunkerDefenseButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button comingSoonCloseButton;
    [SerializeField] private GameObject comingSoonPopUp;
    private GameType gamePlayType;

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
        shootingRangeButton.onClick.AddListener(OnClickShootingRange);
        speedRaceButton.onClick.AddListener(OnClickSpeedRaceAndBunkerDef);
        bunkerDefenseButton.onClick.AddListener(OnClickSpeedRaceAndBunkerDef);
        comingSoonCloseButton.onClick.AddListener(OnClickComingSoonClose);
        backButton.onClick.AddListener(OnClickBack);
    }

    private void ReleaseButtonReferences()
    {
        shootingRangeButton.onClick.RemoveListener(OnClickShootingRange);
        speedRaceButton.onClick.RemoveListener(OnClickSpeedRaceAndBunkerDef);
        bunkerDefenseButton.onClick.RemoveListener(OnClickSpeedRaceAndBunkerDef);
        comingSoonCloseButton.onClick.RemoveListener(OnClickComingSoonClose);
        backButton.onClick.RemoveListener(OnClickBack);
    }   

    public void OnSelectionGameplayType(GameType gameType)
    {
        gamePlayType = gameType;
    }

    private void OnClickShootingRange()
    {
        if(gamePlayType == GameType.normal)
        {
            UIManager.Instance.ShowScreen(Page.Shootingrange.ToString(), Hide.none);
        }
        else
        {
            UIManager.Instance.ShowScreen(Page.Shootingrange.ToString(), Hide.none);
        }
    }

    private void OnClickSpeedRaceAndBunkerDef()
    {
        UIManager.Instance.ShowScreen(Page.ComingSoonOverlay.ToString(), Hide.none);
    }

    private void OnClickComingSoonClose()
    {
        UIManager.Instance.HideScreen(Page.ComingSoonOverlay.ToString());
    }

    private void OnClickBack()
    {
        UIManager.Instance.ShowScreen(Page.CharacterSelection.ToString(),Hide.none);
        UIManager.Instance.ShowScreen(Page.TopAndBottomBar.ToString(),Hide.none);
        UIManager.Instance.HideScreen(Page.LevelSelection.ToString());
    }
}
