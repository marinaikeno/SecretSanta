using System;
using System.Collections.Generic; // to use Lists
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSanta.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public DateTime ExchangeDate { get; set; }
        public float SpendLimit { get; set; }
        public string ExchangeLocation { get; set; }
        [InverseProperty("Event")]
        public List<Participant> Participants { get; set; }
        [InverseProperty("Event")]
        public List<SecretSantaModel> SecretSantas { get; set; }
        public int OrganizerId { get; set; }
        public User Organizer { get; set; }
        public DateTime CreatedAt { get; set; }

        public Event()
        {
            Participants = new List<Participant>();
            SecretSantas = new List<SecretSantaModel>();
        }
    }
}