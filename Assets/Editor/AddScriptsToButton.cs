using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AddScriptsToButton : EditorWindow
{
    [MenuItem("GameObject/Add Button Image Changer Script To Buttons")]
    public static void AddButtonImageChangerToButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach(Button button in buttons)
        {
            button.gameObject.AddComponent<ButtonImageChanger>();
        }
    }
    [MenuItem("Window/Change All Button Transition To SpriteSwap")]
    public static void ChangeAllButtonTransitionToSpriteSwap()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            if(button.transition!=Selectable.Transition.SpriteSwap)
            {
                button.transition = Selectable.Transition.SpriteSwap;
            }
        }
    }
}
