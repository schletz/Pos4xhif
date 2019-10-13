using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthExample.App.Services
{
    /// <summary>
    /// DTO Objekt für den HTTP Request Body.
    /// </summary>
    public class ApplicationUser
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public long? PupilId { get; set; }
        public string Token { get; set; } = "";
    }
}
