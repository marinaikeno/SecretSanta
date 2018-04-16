using System;
using System.Collections.Generic; // to use Lists
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSanta.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event Event { get; set; }
        public Boolean Accepted { get; set; }

    }
}