using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationDemo.Model
{
    /// <summary>
    /// DTO Objekt für den HTTP Request Body.
    /// </summary>
    public class UserCredentials
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
