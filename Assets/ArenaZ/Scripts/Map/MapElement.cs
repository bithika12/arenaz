using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ArenaZ
{
    public class MapElement : MonoBehaviour
    {
        [SerializeField] private Button elementButton;
        [SerializeField] private Image elementImage;
        [SerializeField] private Text elementName;

        private GameData data;
        private Action<GameData> onClickCallback;

        private void Start()
        {
            if (elementButton == null)
                elementButton = GetComponent<Button>();
            if (elementButton != null)
                elementButton.onClick.AddListener(onClick);
        }

        private void onClick()
        {
            onClickCallback?.Invoke(data);
        }

        public void Initialize(GameData a_Data, Action<GameData> a_OnClickCallback)
        {
            data = a_Data;
            onClickCallback = a_OnClickCallback;
        }
    }
}