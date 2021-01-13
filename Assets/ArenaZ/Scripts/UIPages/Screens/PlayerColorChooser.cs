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
        [SerializeField] private List<Button> colorButtons;

        [SerializeField] private Image selectedColorImage;
        [SerializeField] private List<ColorButtonData> colorButtonDatas = new List<ColorButtonData>();

        private List<string> colorButtonNames = new List<string>();
        public List<string> ColorButtonNames { get { return colorButtonNames; } }
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
            colorButtons.ForEach(button => { button.onClick.AddListener(() => onClickSetDartAndCharacterColor(button)); colorButtonNames.Add(button.GetComponent<ButtonImageChanger>().GetButtonType); });
        }

        private void releaseButtonReferences()
        {
            colorButtons.ForEach(button => { button.onClick.RemoveListener(() => onClickSetDartAndCharacterColor(button)); });
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

