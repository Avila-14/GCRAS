namespace GCRAS.Models
{
    public class User
    {
        public int Id { get; set; } // Identifiers the user (primary key)
        public string Username { get; set; } // The login name of the user. 
        public string password { get; set; } // The password used by the user to log in. 
        public string Email { get; set; } // The user's email address
        public string Role { get; set; } // Defines the user's level of access or permissions
        public string CustomKey { get; set; } // Key unique to access
        public bool IsTemporary { get; set; } // Reffers if the usser is visitor
        public DateTime RegDate { get; set; } //
    }

        
}
