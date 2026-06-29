using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Persistence;

namespace PetWorld.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _context;

    public ChatRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveMessageAsync(ChatMessage message)
    {
        await _context.ChatMessages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetAllAsync()
    {
        return await _context.ChatMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}