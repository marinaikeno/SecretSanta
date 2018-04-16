using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions; // Password regex

namespace SecretSanta.Models
{
    public class WishViewModel
    {
        [Required(ErrorMessage = "Event Name is required")]
        [MinLength(2, ErrorMessage = "Event name must be at least 2 characters")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

    }
}