using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;

public class CharacterNameShowAndHide : MonoBehaviour
{
    UIScreenChild[] names = new UIScreenChild[6];
    [SerializeField]
    private HorizontalScrollSnap horizontalScrollSnap;

   private void Start()
   {
        names = GetComponentsInChildren<UIScreenChild>();
        HideRestCharacterNames();
   }
    private void HideRestCharacterNames()
    {
        for (int i = 1; i < names.Length; i++)
        {
            names[i].Hide();
        }
    }
    public void PageChecker()
    {
        names[horizontalScrollSnap.CurrentPage].Show();
        names[horizontalScrollSnap._previousPage].Hide();
    }
}   
   
   