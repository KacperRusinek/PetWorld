# PetWorld 🐾

Sklep internetowy z asystentem AI dla zwierząt domowych.
System AI pomaga klientom znaleźć odpowiednie produkty poprzez chat.

## Technologie

- **.NET 10 / Blazor Server** — interfejs użytkownika
- **Clean Architecture** — Domain, Application, Infrastructure, Web
- **Microsoft Semantic Kernel Agents** — Writer-Critic workflow
- **MySQL** — baza danych historii rozmów
- **Ollama (llama3.2)** — lokalny model AI (domyślny)
- **OpenAI GPT-4o-mini** — opcjonalnie przez klucz API
- **Docker / Docker Compose** — konteneryzacja

## Uruchomienie

### Wymagania

- Docker Desktop

### Szybki start

    docker compose up

Aplikacja dostępna na: **http://localhost:5000**

Po uruchomieniu pobierz model AI:

    docker exec -it petworld_ollama ollama pull llama3.2

## Konfiguracja klucza API

Domyślnie aplikacja używa lokalnego modelu **Ollama (llama3.2)** — nie wymaga klucza API.

### Opcja 1 — OpenAI (appsettings.json)

    {
      "OpenAI": {
        "ApiKey": "sk-twoj-klucz-openai"
      }
    }

### Opcja 2 — Zmienna środowiskowa

    OpenAI__ApiKey=sk-twoj-klucz docker compose up

Jeśli klucz OpenAI jest ustawiony — aplikacja używa OpenAI.
Jeśli nie — automatycznie przełącza się na Ollama.

## Architektura

    PetWorld/
    ├── PetWorld.Domain/          # Encje i interfejsy
    ├── PetWorld.Application/     # Interfejsy serwisów
    ├── PetWorld.Infrastructure/  # Implementacje (baza danych, AI)
    └── PetWorld.Web/             # Blazor Server UI

### Writer-Critic workflow

    Pytanie klienta
          ↓
     Writer Agent → generuje odpowiedź na podstawie katalogu produktów
          ↓
     Critic Agent → ocenia odpowiedź (APPROVED / REVISION)
          ↓
     Jeśli REVISION → Writer poprawia (max 3 iteracje)
          ↓
     Zatwierdzona odpowiedź → klient + zapis do MySQL

## Strony

| Strona | URL | Opis |
|--------|-----|------|
| Chat | `/` | Rozmowa z asystentem AI |
| Historia | `/history` | Historia wszystkich rozmów |

## Katalog produktów

| Produkt | Kategoria | Cena |
|---------|-----------|------|
| Royal Canin Adult Dog 15kg | Karma dla psów | 289 zł |
| Whiskas Adult Kurczak 7kg | Karma dla kotów | 129 zł |
| Trixie Drapak XL 150cm | Akcesoria dla kotów | 399 zł |
| Kong Classic Large | Zabawki dla psów | 69 zł |
| Ferplast Klatka dla chomika | Gryzonie | 189 zł |
| Tetra AquaSafe 500ml | Akwarystyka | 45 zł |
| JBL ProFlora CO2 Set | Akwarystyka | 540 zł |
| Vitapol Siano dla królików 1kg | Gryzonie | 25 zł |