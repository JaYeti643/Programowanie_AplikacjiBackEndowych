using System;
using FluentValidation;
using AppCore.Models;

public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(255).WithMessage("Company name must not exceed 255 characters.");

        RuleFor(x => x.NIP)
            .Must(nip => string.IsNullOrWhiteSpace(nip) || NIP.IsValid(nip))
            .WithMessage("Invalid NIP format.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^[0-9\-\+\s\(\)]{7,}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Industry)
            .MaximumLength(100).WithMessage("Industry must not exceed 100 characters.");

        RuleFor(x => x.Website)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid website URL format.");

        RuleFor(x => x.AnnualRevenue)
            .GreaterThanOrEqualTo(0).WithMessage("Annual revenue must be greater than or equal to 0.")
            .When(x => x.AnnualRevenue.HasValue);
    }
}

public class UpdateCompanyDtoValidator : AbstractValidator<UpdateCompanyDto>
{
    public UpdateCompanyDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(255).WithMessage("Company name must not exceed 255 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.NIP)
            .Must(nip => string.IsNullOrWhiteSpace(nip) || NIP.IsValid(nip))
            .WithMessage("Invalid NIP format.")
            .When(x => !string.IsNullOrEmpty(x.NIP));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^[0-9\-\+\s\(\)]{7,}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Industry)
            .MaximumLength(100).WithMessage("Industry must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Industry));

        RuleFor(x => x.Website)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid website URL format.")
            .When(x => !string.IsNullOrEmpty(x.Website));

        RuleFor(x => x.AnnualRevenue)
            .GreaterThanOrEqualTo(0).WithMessage("Annual revenue must be greater than or equal to 0.")
            .When(x => x.AnnualRevenue.HasValue);
    }
}
    