using PetWorld.Domain.Entities;

namespace PetWorld.Domain.Interfaces;

public interface IChatRepository
{
    Task SaveMessageAsync(ChatMessage message);
    Task<IEnumerable<ChatMessage>> GetAllAsync();
}