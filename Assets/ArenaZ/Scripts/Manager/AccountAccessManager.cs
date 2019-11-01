using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ.AccountAccess
{
    public class AccountAccessManager : MonoBehaviour
    {
        [SerializeField]
        private UIScreenChild uiChild;
        // Start is called before the first frame update
        void Start()
        {
            uiChild.ShowGameObjWithAnim();
        }
    }
}
