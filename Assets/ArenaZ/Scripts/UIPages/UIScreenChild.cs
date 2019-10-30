using UnityEngine;
using ArenaZ.ScreenManagement;

public class UIScreenChild : UIScreen
{
    protected override void Awake()
    {
       if(GetComponent<Animator>())
        {
            animator = GetComponent<Animator>();
        }
    }
}
