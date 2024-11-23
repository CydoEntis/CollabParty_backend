﻿using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CollabParty.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public int CurrentLevel { get; set; } = 1;

        [Required]
        public int CurrentExp { get; set; } = 0;

        public int Currency { get; set; } = 0;

        [Required]
        public int ExpToNextLevel { get; set; } = 100;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserAvatar> UserAvatars { get; set; } = new List<UserAvatar>();

        public int CalculateExpForLevel(int level)
        {
            int baseExp = 100;
            return baseExp * level;
        }
    }
}