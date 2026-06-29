using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Infrastructure.Services;

public class ChatAgentService : IChatService
{
    private readonly IChatRepository _repository;
    private readonly Kernel _kernel;

    private const string ProductCatalog = """
        | Nazwa produktu | Kategoria | Cena | Opis |
        |---|---|---|---|
        | Royal Canin Adult Dog 15kg | Karma dla ps�w | 289 z� | Premium karma dla doros�ych ps�w |
        | Whiskas Adult Kurczak 7kg | Karma dla kot�w | 129 z� | Sucha karma dla doros�ych kot�w |
        | Trixie Drapak XL 150cm | Akcesoria dla kot�w | 399 z� | Wysoki drapak z platformami |
        | Kong Classic Large | Zabawki dla ps�w | 69 z� | Wytrzyma�a zabawka na smako�yki |
        | Ferplast Klatka dla chomika | Gryzonie | 189 z� | Klatka 60x40cm z wyposa�eniem |
        | Tetra AquaSafe 500ml | Akwarystyka | 45 z� | Uzdatniacz wody do akwarium |
        | JBL ProFlora CO2 Set | Akwarystyka | 540 z� | Kompletny zestaw CO2 dla ro�lin |
        | Vitapol Siano dla kr�lik�w 1kg | Gryzonie | 25 z� | Naturalne siano ��kowe |
        """;

    public ChatAgentService(IChatRepository repository, Kernel kernel)
    {
        _repository = repository;
        _kernel = kernel;
    }

    public async Task<string> AskAsync(string question)
    {
        var writerAgent = new ChatCompletionAgent
        {
            Name = "Writer",
            Kernel = _kernel,
            Instructions = $"""
                Jeste� asystentem sklepu PetWorld.
                Odpowiadasz na pytania klient�w na podstawie katalogu produkt�w.
                
                Katalog produkt�w:
                {ProductCatalog}
                
                Odpowiadaj po polsku, konkretnie i pomocnie.
                """
        };

        var criticAgent = new ChatCompletionAgent
        {
            Name = "Critic",
            Kernel = _kernel,
            Instructions = """
                Jeste� krytykiem odpowiedzi sklepu PetWorld.
                Oceniasz czy odpowied� Writera jest:
                - Poprawna merytorycznie
                - Pomocna dla klienta
                - Oparta na katalogu produkt�w
                
                Je�li odpowied� jest dobra, napisz tylko: APPROVED
                Je�li wymaga poprawy, napisz: REVISION: [co poprawi�]
                """
        };

        var chat = new AgentGroupChat(writerAgent, criticAgent)
        {
            ExecutionSettings = new AgentGroupChatSettings
            {
                TerminationStrategy = new ApprovalTerminationStrategy
                {
                    MaximumIterations = 3
                }
            }
        };

        chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, question));

        string lastAnswer = string.Empty;
        int iterations = 0;
        bool isApproved = false;

await foreach (var response in chat.InvokeAsync())
{
    Console.WriteLine($"Agent: {response.AuthorName} | Content: {response.Content}");
    
    if (response.AuthorName == "Writer" && !string.IsNullOrEmpty(response.Content))
    {
        lastAnswer = response.Content;
        iterations++;
    }
 if (response.AuthorName == "Critic")
    {
        var content = response.Content?.ToUpper() ?? string.Empty;
        isApproved = content.Contains("APPROVED") || 
                     content.Contains("DOBRA") || 
                     content.Contains("POPRAWNA") ||
                     content.Contains("OK") ||
                     !content.Contains("REVISION");
    }
}

if (string.IsNullOrEmpty(lastAnswer))
{
    lastAnswer = "Przepraszam, nie mogłem znaleźć odpowiedzi.";
}

        await _repository.SaveMessageAsync(new ChatMessage
        {
            Question = question,
            Answer = lastAnswer,
            Iterations = iterations,
            IsApproved = isApproved,
            CreatedAt = DateTime.UtcNow
        });

        return lastAnswer;
    }
}

public class ApprovalTerminationStrategy : TerminationStrategy
{
    protected override Task<bool> ShouldAgentTerminateAsync(
        Agent agent,
        IReadOnlyList<ChatMessageContent> history,
        CancellationToken ct)
    {
        var lastMessage = history.LastOrDefault()?.Content ?? string.Empty;
        return Task.FromResult(lastMessage.Contains("APPROVED"));
    }
}