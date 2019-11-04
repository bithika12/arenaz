﻿using UnityEngine;

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
        }
    }
}
