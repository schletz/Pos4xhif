using System;
using System.ComponentModel.DataAnnotations;

namespace ExamManager.App.Entities
{
    public class User : IEntity<int>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public User(string username, string salt, string passwordHash, Teacher teacher)
        {
            Username = username;
            Salt = salt;
            PasswordHash = passwordHash;
            Teacher = teacher;
            TeacherId = teacher.Id;
        }

        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        public string Username { get; set; }
        [MaxLength(44)]   // 256 (32 Bytes) Base64 Encoded string
        public string Salt { get; set; }
        [MaxLength(88)]   // 512 (64 Bytes) Base64 Encoded string
        public string PasswordHash { get; set; }
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
    }

}
