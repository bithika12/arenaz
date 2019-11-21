﻿using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;

[RequireComponent(typeof(Button))][DisallowMultipleComponent]
public class ButtonImageChanger : MonoBehaviour
{
    private Button myButton;
    private SpriteState myButtonSpriteState;
    private ImageType buttonImageType;
    public ButtonType myButtonType;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButtonSpriteState = myButton.spriteState;
    }

    private void Start()
    {
        buttonImageType = UIManager.Instance.ButtonImageType(myButtonType);
        ChangeButtonSpriteWhenPressed();
    }

    private void ChangeButtonSpriteWhenPressed()
    {
        if (buttonImageType.pressedSprite)
        {
            myButtonSpriteState.pressedSprite = buttonImageType.pressedSprite;
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
