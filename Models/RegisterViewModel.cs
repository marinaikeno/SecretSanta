using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions; // Password regex

namespace SecretSanta.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters")]
        [DataType(DataType.Text)]
        [RegularExpression(@"(?!.*\s)^[a-zA-Z]+$", ErrorMessage = "Must be letters and cannot contain spaces")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [MinLength(2, ErrorMessage = "Last name be at least 2 characters")]
        [DataType(DataType.Text)]
        [RegularExpression(@"(?!.*\s)^[a-zA-Z]+$", ErrorMessage = "Must be letters and cannot contain spaces")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Must contain at least one number and one uppercase letter")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirmation Password is required")]
        [Compare("Password", ErrorMessage = "Must match password")]
        [DataType(DataType.Password)]
        public string C_Password { get; set; }
    }
}