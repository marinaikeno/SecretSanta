using System;
using System.Collections.Generic; // to use Lists
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSanta.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        [InverseProperty("User")]
        public List<Participant> MyEvents { get; set; }
        [InverseProperty("Organizer")]
        public List<Event> EventOrganized { get; set; }
        [InverseProperty("Recipient")]
        public List<SecretSantaModel> SecretSantas { get; set; }
        [InverseProperty("User")]
        public List<SecretSantaModel> MySecretSantas { get; set; }
        [InverseProperty("User")]
        public List<WishItem> Wishlist { get; set; }
        public DateTime CreatedAt { get; set; }

        public User()
        {
            MyEvents = new List<Participant>();
            EventOrganized = new List<Event>();
            Wishlist = new List<WishItem>();
            SecretSantas = new List<SecretSantaModel>();
        }
    }
}