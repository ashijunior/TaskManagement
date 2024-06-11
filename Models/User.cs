using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManager_Simplex_.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } //PRIMARY KEY
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
