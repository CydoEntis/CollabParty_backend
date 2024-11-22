﻿using CollabParty.Application.Common.Dtos.Avatar;

namespace CollabParty.Application.Common.Dtos;

public class LoginDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public TokenDto Tokens { get; set; }
    public UserAvatarDto Avatar { get; set; }
    public int Currency { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentExp { get; set; }
    public int ExpToNextLevel { get; set; }
}