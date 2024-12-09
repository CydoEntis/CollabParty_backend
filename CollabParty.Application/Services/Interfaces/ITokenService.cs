using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Services.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(string userId, string sessionId);
    RefreshToken CreateRefreshToken();
    CsrfToken CreateCsrfToken();
    bool IsRefreshTokenValid(Session session, string refreshToken);
}