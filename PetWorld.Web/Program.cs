using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Persistence;
using PetWorld.Infrastructure.Repositories;
using PetWorld.Infrastructure.Services;
using PetWorld.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!; //! mowi kompilatorowi ze to nie jest null 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString)
);

// AI — OpenAI jeśli klucz ustawiony, fallback na Ollama
#pragma warning disable SKEXP0070
builder.Services.AddSingleton<Kernel>(_ =>
{
    var openAiKey = builder.Configuration["OpenAI:ApiKey"];
    var kernelBuilder = Kernel.CreateBuilder();

    if (!string.IsNullOrEmpty(openAiKey))
    {
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: "gpt-4o-mini",
            apiKey: openAiKey
        );
    }
    else
    {
        var ollamaEndpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://ollama:11434";
        kernelBuilder.AddOllamaChatCompletion(
            modelId: "llama3.2",
            endpoint: new Uri(ollamaEndpoint)
        );
    }

    return kernelBuilder.Build();
});
#pragma warning restore SKEXP0070

// Serwisy
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatService, ChatAgentService>();

var app = builder.Build();

// Auto-migracja przy starcie
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();