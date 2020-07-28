using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;

[RequireComponent(typeof(Button))][DisallowMultipleComponent]
public class ButtonImageChanger : MonoBehaviour
{
    protected Button myButton;
    private SpriteState myButtonSpriteState;
    private ButtonImage buttonImageType;
    public ButtonType myButtonType;
    public string GetButtonType { get { return myButtonType.ToString(); } }

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButtonSpriteState = myButton.spriteState;
    }

    private void Start()
    {
        buttonImageType = UIManager.Instance.ButtonImageType(GetButtonType);
        ChangeButtonSpriteWhenPressed();
    }

    private void ChangeButtonSpriteWhenPressed()
    {
        if (buttonImageType.pressedSprite)
        {
            myButtonSpriteState.pressedSprite = buttonImageType.pressedSprite;
            myButton.spriteState = myButtonSpriteState;
        }
        if(buttonImageType.disabledSprite)
        {
            myButtonSpriteState.disabledSprite = buttonImageType.disabledSprite;
            myButton.spriteState = myButtonSpriteState;
        }
    }

    public void ChangeButtonSpriteWhenDisabled()
    {
        if (buttonImageType.disabledSprite)
        {
            myButton.image.sprite = buttonImageType.disabledSprite;
        }
    }

}
