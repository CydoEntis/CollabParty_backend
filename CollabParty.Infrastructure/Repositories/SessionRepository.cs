﻿using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class SessionRepository : BaseRepository<Session>, ISessionRepository
{
    private readonly AppDbContext _db;

    public SessionRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<Session> UpdateAsync(Session entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.Sessions.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}