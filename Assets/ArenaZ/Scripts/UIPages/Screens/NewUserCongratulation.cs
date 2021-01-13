using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUserCongratulation : MonoBehaviour
{
    [SerializeField] private Text welcomeText;

    public void setWelComeText(string text)
    {
        welcomeText.text = ConstantStrings.newAccountWelcomeFirst+" "+text+" "+ConstantStrings.newAccountWelcomeSecond;
    }
}
