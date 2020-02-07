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
            [SerializeField]protected Button m_CustomButton;

            protected T m_CellData;
            protected Action<T> m_OnClickCallback;

            private void Awake()
            {
                if (m_CustomButton == null)
                {
                    Button button = GetComponent<Button>();
                    if (button != null)
                        button.onClick.AddListener(OnClick);
                }
                else
                    m_CustomButton.onClick.AddListener(OnClick);
            }

            protected virtual void OnClick()
            {
                m_OnClickCallback?.Invoke(m_CellData);
            }

            public virtual void InitializeCell(T cellData, Action<T> onClickCallback = null)
            {
                m_CellData = cellData;
                m_OnClickCallback = onClickCallback;
            }

            public virtual void InitializeCallback(Action<T> onClickCallback = null)
            {
                m_OnClickCallback = onClickCallback;
            }

            public virtual void UpdateCell(T cellData, Action<T> onClickCallback = null)
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
