using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Model
{
    public enum Usertype { Owner = 1, Admin }
    [Table("User")]
    public class User : IEntity<int>
    {
        public User(string username, string salt, string passwordHash, Usertype usertype)
        {
            Username = username;
            Salt = salt;
            PasswordHash = passwordHash;
            Usertype = usertype;
            Guid = Guid.NewGuid();
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        [MaxLength(255)]
        public string Username { get; set; }
        [MaxLength(44)]  // 256 bit Hash as base64
        public string Salt { get; set; }
        [MaxLength(88)]  // 512 bit SHA512 Hash as base64
        public string PasswordHash { get; set; }
        public Usertype Usertype { get; set; }
        public ICollection<Store> Stores { get; } = new List<Store>();
    }
}
