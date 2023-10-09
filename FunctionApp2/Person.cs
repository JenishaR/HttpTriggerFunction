using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpTriggerFunctionApp
{
    public class Person
    {
        public string firstname { get; set; }
        public string lastname { get; set; }

        public Person(string firstname, string lastname)
        {
            this.firstname = firstname;
            this.lastname = lastname;
        }
    }

    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.firstname).NotEmpty()
                .WithMessage("first name required")
         .Must(IsSpecialCharacter).WithMessage("First name should not contain special characters")
         .MinimumLength(3).WithMessage("First name should contain atleast 3 characters");


            RuleFor(x => x.lastname).NotEmpty()
              .WithMessage("last name required")
            .Must(IsSpecialCharacterPresent).WithMessage("Last name should not contain special characters")
            .MinimumLength(3).WithMessage("last name should contain atleast 3 characters");
        }

        public bool IsSpecialCharacter(string firstname)
        {
            return !Regex.IsMatch(firstname, @"[^a-zA-Z\s]");
        }
        public bool IsSpecialCharacterPresent(string lastname)
        {
            return !Regex.IsMatch(lastname, @"[^a-zA-Z\s]");
        }
    }
}
