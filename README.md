# PetWorld 🐾

Sklep internetowy z asystentem AI dla zwierząt domowych.
Zbudowany z użyciem Clean Architecture, Blazor Server i Microsoft Semantic Kernel.

## Technologie
- .NET 10 / Blazor Server
- Clean Architecture (Domain, Application, Infrastructure, Web)
- Microsoft Semantic Kernel Agents (Writer-Critic workflow)
- MySQL (baza danych)
- Ollama (lokalny model AI) / OpenAI (opcjonalnie)
- Docker / Docker Compose

## Uruchomienie

### Wymagania
- Docker Desktop

### Szybki start
```bash
docker compose up
```

Aplikacja dostępna na: http://localhost:5000

### Konfiguracja klucza API (opcjonalnie)
Domyślnie aplikacja używa lokalnego modelu Ollama (llama3.2).

Aby użyć OpenAI, ustaw klucz w `appsettings.json`:
```json
{
  "OpenAI": {
    "ApiKey": "sk-twoj-klucz"
  }
}
```

Lub przez zmienną środowiskową:
```bash
OpenAI__ApiKey=sk-twoj-klucz docker compose up
```

## Architektura