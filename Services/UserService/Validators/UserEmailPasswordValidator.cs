﻿using DTO;
using FluentValidation;

namespace UserService.Validators
{
    public class UserEmailPasswordValidator : AbstractValidator<UserEmailPassword>
    {
        public UserEmailPasswordValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage("Email address must not be empty.")
                .EmailAddress()
                .WithMessage("Email address must be in valid format.")
                .Length(50)
                .WithMessage("Email length must be 50 symbols.");

            RuleFor(user => user.PasswordHash)
                .NotEmpty()
                .WithMessage("Password hash must not be empty.")
                .Length(40)
                .WithMessage("Password's hash length must be 40 symbols.");
        }
    }
}
