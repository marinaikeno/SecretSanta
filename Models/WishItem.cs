using System;
using System.Collections.Generic; // to use Lists
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSanta.Models
{
    public class WishItem
    {
        public int WishItemId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public WishItem()
        {

        }
    }
}