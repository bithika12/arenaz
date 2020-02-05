using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace DevCommons
{
    namespace UI
    {
        public class Cell<T> : MonoBehaviour
        {
            public Button _customButton;

            protected T m_CellData;
            protected Action<object> m_OnClickCallback;

            private void Awake()
            {
                if (_customButton == null)
                {
                    Button button = GetComponent<Button>();
                    if (button != null)
                        button.onClick.AddListener(OnClick);
                }
                else
                    _customButton.onClick.AddListener(OnClick);
            }

            protected virtual void OnClick()
            {
                m_OnClickCallback?.Invoke(m_CellData);
            }

            public virtual void InitializeCell(T cellData, Action<object> onClickCallback = null)
            {
                m_CellData = cellData;
                m_OnClickCallback = onClickCallback;
            }

            public virtual void InitializeCallback(Action<object> onClickCallback = null)
            {
                m_OnClickCallback = onClickCallback;
            }

            public virtual void UpdateCell(T cellData, Action<object> onClickCallback = null)
            {
                m_CellData = cellData;
                m_OnClickCallback = onClickCallback;
            }

            public virtual T CellData()
            {
                return m_CellData;
            }
        }
    }
}
