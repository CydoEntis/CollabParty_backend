﻿using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class QuestRepository : BaseRepository<Quest>, IQuestRepository
{
    private readonly AppDbContext _db;

    public QuestRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<Quest> UpdateAsync(Quest entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.Quests.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}