using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using ArenaZ.Manager;

public class CharacterNameShowAndHide : MonoBehaviour
{
    private string[] names = { Page.Canines.ToString(),Page.Kepler.ToString(),Page.Cyborg.ToString(),Page.CyborgSecond.ToString(),Page.Human.ToString(),Page.Ebot.ToString(),Page.KeplerSecond.ToString() };
    [SerializeField]
    private HorizontalScrollSnap horizontalScrollSnap;

    private void Start()
    {
        ShowFirstText();
    }

    private void ShowFirstText()
    {
        UIManager.Instance.ShowScreen(Page.Canines.ToString(),Hide.none);
    }

    public void PageChecker()
    {
        UIManager.Instance.ShowScreen(names[horizontalScrollSnap._currentPage],Hide.previous);
    }
}   
   
   