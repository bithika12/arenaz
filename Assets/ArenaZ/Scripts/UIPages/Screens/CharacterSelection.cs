using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;
using UnityEngine.UI.Extensions;
using ArenaZ.LevelSelection;

[RequireComponent (typeof(UIScreen))]
public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private Button arenaButton;
    [SerializeField] private Button trainingButton;
    [SerializeField] private Button rankingButton;
    [SerializeField] private LevelSelection levelSelection;
    [SerializeField] private HorizontalScrollSnap horizontalScrollSnap;
    private string[] names = { Page.Canines.ToString(), Page.Kepler.ToString(), Page.Cyborg.ToString(), Page.CyborgSecond.ToString(), Page.Human.ToString(), Page.Ebot.ToString(), Page.KeplerSecond.ToString() };

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
        levelSelection.OnSelectionGameplayType(GameType.normal);
    }

    private void OnClickTraining()
    {
        UIManager.Instance.HideScreen(Page.CharacterSelection.ToString());
        UIManager.Instance.HideScreen(Page.TopAndBottomBar.ToString());
        UIManager.Instance.ShowScreen(Page.LevelSelection.ToString(),Hide.none);
        levelSelection.OnSelectionGameplayType(GameType.training);
    }
     
    private void OnclickRanking()
    {
        UIManager.Instance.ShowScreen(Page.LeaderBoard.ToString(), Hide.none);
    }
}
