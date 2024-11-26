﻿using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class UserAvatarRepository : BaseRepository<UserAvatar>, IUserAvatarRepository
{
    private readonly AppDbContext _db;

    public UserAvatarRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task AddRangeAsync(IEnumerable<UserAvatar> userAvatars)
    {
        await _db.UserAvatars.AddRangeAsync(userAvatars);
        await _db.SaveChangesAsync();
    }
    
    public async Task<UserAvatar> UpdateAsync(UserAvatar entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.UserAvatars.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}