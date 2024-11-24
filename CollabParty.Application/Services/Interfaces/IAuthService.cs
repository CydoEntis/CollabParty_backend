﻿using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginDto>> Login(LoginCredentialsDto dto);
    Task<Result<LoginDto>> Register(RegisterCredentialsDto dto);
    Task<Result> Logout(TokenDto dto);
    Task<Result<TokenDto>> RefreshTokens(TokenDto dto);
}