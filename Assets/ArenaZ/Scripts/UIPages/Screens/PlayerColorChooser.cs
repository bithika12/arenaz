using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using ArenaZ.Manager;
using System.Collections.Generic;
using DevCommons.Utility;

namespace ArenaZ.Screens 
{
	public class PlayerColorChooser : MonoBehaviour
	{
        [SerializeField] private Button darkGreenButton;
        [SerializeField] private Button lightBlueButton;
        [SerializeField] private Button yellowButton;
        [SerializeField] private Button whiteButton;
        [SerializeField] private Button redButton;
        [SerializeField] private Button greyButton;
        [SerializeField] private Button darkBlueButton;
        [SerializeField] private Button orangeButton;
        [SerializeField] private Button lightGreenButton;
        [SerializeField] private Button tealButton;

        [SerializeField] private Image selectedColorImage;
        [SerializeField] private List<ColorButtonData> colorButtonDatas = new List<ColorButtonData>();

        public static Action<string> setColorAfterChooseColor;

        private void Start()
        {           
            gettingButtonReferences();
        }

        private void OnDestroy()
        {
            releaseButtonReferences();
        }
        #region BUTTON_REFERENCES
        private void gettingButtonReferences()
        {
            darkGreenButton.onClick.AddListener(()=> onClickSetDartAndCharacterColor(darkGreenButton));
            lightBlueButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(lightBlueButton));
            yellowButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(yellowButton));
            whiteButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(whiteButton));
            redButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(redButton));
            greyButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(greyButton));
            darkBlueButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(darkBlueButton));
            orangeButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(orangeButton));
            lightGreenButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(lightGreenButton));
            tealButton.onClick.AddListener(() => onClickSetDartAndCharacterColor(tealButton));
        }

        private void releaseButtonReferences()
        {
            darkGreenButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(darkGreenButton));
            lightBlueButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(lightBlueButton));
            yellowButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(yellowButton));
            whiteButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(whiteButton));
            redButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(redButton));
            greyButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(greyButton));
            darkBlueButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(darkBlueButton));
            orangeButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(orangeButton));
            lightGreenButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(lightGreenButton));
            tealButton.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(tealButton));
        }
        #endregion
        private void onClickSetDartAndCharacterColor(Button myButton)
        {
            string colorName = myButton.GetComponent<ButtonImageChanger>().GetButtonType;
            setColorAfterChooseColor?.Invoke(colorName);
            Debug.Log("Clicked Color Name: " + colorName);
            UIManager.Instance.ToggleScreenWithAnim(Page.PlayerColorChooser.ToString());
        }

        public void SetSelectedColor(string a_Color)
        {
            EColor t_Color = EColor.DarkBlue;
            if (t_Color.TryParse(a_Color, out t_Color))
            {
                ColorButtonData t_Data = colorButtonDatas.Where(x => x.ColorType.Equals(t_Color)).First();
                selectedColorImage.sprite = t_Data.ColorImage;
            }
        }
    }
}

[System.Serializable]
public class ColorButtonData
{
    public EColor ColorType = EColor.DarkBlue;
    public Sprite ColorImage;
}

