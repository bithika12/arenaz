using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;
using UnityEngine.UI.Extensions;
using ArenaZ.LevelMangement;
using ArenaZ.AccountAccess;
using ArenaZ.GameMode;

[RequireComponent (typeof(UIScreen))]
public class CharacterSelection : RedAppleSingleton<CharacterSelection>
{
    //Private Fields
    [Header("Buttons")]
    [Space(5)]
    [SerializeField] private Button arenaButton;
    [SerializeField] private Button trainingButton;
    [SerializeField] private Button rankingButton;

    [Header("Text Fields")]
    [Space(5)]
    [SerializeField] private Text userName;

    [Header("Scroll Snap")]
    [Space(5)]
    [SerializeField] private HorizontalScrollSnap horizontalScrollSnap;
    private readonly string[] names = { Page.Canines.ToString(), Page.Kepler.ToString(), Page.Cyborg.ToString(), Page.CyborgSecond.ToString(), Page.Human.ToString(), Page.Ebot.ToString(), Page.KeplerSecond.ToString()};
    //Public Fields

    private void Start()
    {
        GettingButtonReferences();
        ShowFirstText();      
    }

    private void OnEnable()
    {
        horizontalScrollSnap.OnSelectionPageChangedEvent.AddListener(PageChecker);
    }

    private void OnDisable()
    {
        horizontalScrollSnap.OnSelectionPageChangedEvent.RemoveListener(PageChecker);
    }

    private void OnDestroy()
    {
        ReleaseButtonReferences();       
    }
    #region Button_References
    private void GettingButtonReferences()
    {
        arenaButton.onClick.AddListener(OnClickArena);
        trainingButton.onClick.AddListener(OnClickArena);
        rankingButton.onClick.AddListener(OnclickRanking);
    }

    private void ReleaseButtonReferences()
    {
        arenaButton.onClick.RemoveListener(OnClickArena);
        trainingButton.onClick.RemoveListener(OnClickArena);
        rankingButton.onClick.RemoveListener(OnclickRanking);
    }
    #endregion

    public void SetUserName()
    {
        userName.text = AccountAccessManager.Instance.UserName;
    }

    #region UI_Functionalities
    private void ShowFirstText()
    {
        UIManager.Instance.ShowCharacterName(Page.Canines.ToString());
    }

    public void PageChecker(int pageNo)
    {
        UIManager.Instance.ShowCharacterName(names[pageNo]);
    }
    
    private void OnClickArena()
    {
        UIManager.Instance.HideScreen(Page.CharacterSelection.ToString());
        UIManager.Instance.HideScreen(Page.TopAndBottomBar.ToString());
        UIManager.Instance.ShowScreen(Page.LevelSelection.ToString(),Hide.none);
        LevelSelection.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        ShootingRange.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        Settings.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        PlayerMatch.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        LevelSelection.Instance.OnSelectionGameplayType(GameType.normal);
    }

    private void OnClickTraining()
    {
        UIManager.Instance.HideScreen(Page.CharacterSelection.ToString());
        UIManager.Instance.HideScreen(Page.TopAndBottomBar.ToString());
        UIManager.Instance.ShowScreen(Page.LevelSelection.ToString(),Hide.none);
        LevelSelection.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        ShootingRange.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        Settings.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        PlayerMatch.Instance.SetProfileImage(names[horizontalScrollSnap._currentPage]);
        LevelSelection.Instance.OnSelectionGameplayType(GameType.training);
    }
     
    private void OnclickRanking()
    {
        UIManager.Instance.ShowScreen(Page.LeaderBoard.ToString(), Hide.none);
    }
    #endregion
}
