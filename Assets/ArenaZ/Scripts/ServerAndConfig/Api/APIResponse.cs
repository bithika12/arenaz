using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ.Api
{
    public class APIResponse{}

    #region Authentication

    public class CreateAccountResponse
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Location { get; set; }
        public string ActivationToken { get; set; }
        public int Id { get; set; }
        public string PrestaUserId { get; set; }
        public bool IsUserSuspended { get; set; }
    }

    #endregion
}
