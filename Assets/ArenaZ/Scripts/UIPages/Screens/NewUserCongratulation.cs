using System.Collections;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine;
using UnityEngine.UI;

public class NewUserCongratulation : MonoBehaviour
{
    [SerializeField] private Text welcomeText;

    public void SetWelComeText(string text)
    {
        welcomeText.text = ConstantStrings.newAccountWelcomeFirst+" "+text+" "+ConstantStrings.newAccountWelcomeSecond;
    }

    public void CloseScreen()
    {
        UIManager.Instance.HideScreen(Page.NewUserCongratulationPanel.ToString());
    }
}
