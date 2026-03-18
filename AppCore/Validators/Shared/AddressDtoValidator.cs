using FluentValidation;

namespace AppCore.Validators.Shared;


    /** Zdefiniuj walidator dla adresu w osobnym pliku (katalog Shared w Validators),
     *  dodaj odpowiednie komunikaty
     *  - Street - nie pusty, maks długość 200
     *  - City - nie pusty, maks długość 100
     *  - PostalCode - nie pusty,format xx-xxx, gdzie x to cyfra
     *  - Country - nie pusty, maksymalnie 100 znaków
     *  - Type - nie pusty, musi być jedną ze stałych wyliczeniowych
    */
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Ulica jest wymagana")
                .MaximumLength(200).WithMessage("Ulica może mieć maksymalnie 200 znaków");
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Miasto jest wymagana")
                .MaximumLength(100).WithMessage("Miasto może mieć maksymalnie 100 znaków");
            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Kod pocztowy jest wymagany")
                .Matches(@"^\d{2}-\d{3}$").WithMessage("Kod pocztowy musi być w formacie xx-xxx");
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Kraj jest wymagany")
                .MaximumLength(100).WithMessage("Kraj może mieć maksymalnie 100");
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Typ jest wymagany")
                .IsInEnum();
        }
    }
