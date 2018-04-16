using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions; // Password regex

namespace SecretSanta.Models
{
    public class EventViewModel
    {
        [Required(ErrorMessage = "Event Name is required")]
        [MinLength(2, ErrorMessage = "Event name must be at least 2 characters")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Exchange date is required")]
        [DataType(DataType.DateTime)]
        [ValidateDate]

        public DateTime ExchangeDate { get; set; }
        [Required(ErrorMessage = "Spending limit is required")]
        [MinValue]
        public float SpendLimit { get; set; }
        [Required(ErrorMessage = "Please select a exchange location")]
        public string ExchangeLocation { get; set; }

    }
    public class ValidateDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime Today = DateTime.Now;
            if (value is DateTime)
            {
                DateTime InputDate = (DateTime)value;
                if (InputDate > Today)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("It is impossible to have your event in the past");
                }
            }

            return new ValidationResult("Please enter a valid date");
        }
    }
    public class MinValue : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((float)value > 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Spending limit is required");
        }
    }
}