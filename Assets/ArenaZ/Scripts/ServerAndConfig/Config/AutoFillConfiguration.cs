using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ
{
    public class AutoFillConfiguration : ScriptableObject
    {
        [Header("Register and Login")]
        public bool AutoActivateUser;
        public string FirstName;
        public string LastName;
        public string UserName;
        public string Email;
        public string Password;
        [Header("Team and Session")]
        public string TeamName;
        public string SessionName;
        public string SessionDescription;
    }
}
