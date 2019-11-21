using UnityEngine;
using TMPro;

namespace ArenaZ.Screens
{
    public class UIScreenChild : UIScreen
    {
        protected override void Awake()
        {
            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
            }
            if (GetComponent<TextMeshProUGUI>())
            {
                txtMeshPro = GetComponent<TextMeshProUGUI>();
            }
        }
    }
}
