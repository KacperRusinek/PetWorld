namespace PetWorld.Application.Interfaces;

public interface IChatService
{
    Task<string> AskAsync(string question);
}