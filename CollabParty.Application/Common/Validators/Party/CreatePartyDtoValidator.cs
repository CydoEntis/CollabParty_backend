﻿using CollabParty.Application.Common.Dtos.Party;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.Party;

public class CreatePartyDtoValidator : AbstractValidator<CreatePartyDto>
{
    public CreatePartyDtoValidator()
    {
        RuleFor(x => x.PartyName).NotEmpty().WithMessage("Party name is required").MinimumLength(5)
            .WithMessage("Party name must be at least 5 characters.").MaximumLength(25)
            .WithMessage("Party name must not exceed 25 characters.");
        
        RuleFor(x => x.PartyName).NotEmpty().WithMessage("Party description is required").MinimumLength(25)
            .WithMessage("Party description must be at least 25 characters.").MaximumLength(200)
            .WithMessage("Party description must not exceed 200 characters.");
    }
}