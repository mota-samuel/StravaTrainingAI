<h1 align="center">Strava Training AI</h1>

Aplicação console em C# que conecta seus dados reais do Strava com o Google Gemini AI
para gerar planos de treino semanais personalizados.

## Objetivo

Correr 5 km em menos de 20 minutos até agosto, treinando 3x por semana,
com planos gerados por IA baseados no histórico real de corridas do atleta.
Atualmente o objetivo está fixado [um objetivo pessoal meu], mas futuramente a aplicação será configurada pelo usuário

---

## Funcionalidades

- Autenticação OAuth 2.0 com o Strava — apenas no primeiro acesso
- Token persistido localmente — a partir do segundo acesso, renovado silenciosamente sem abrir o browser
- Análise automática do histórico — busca as últimas 20 atividades e calcula nível de condicionamento (Beginner / Intermediate / Advanced) baseado no pace médio real
- Plano de treino gerado por IA — o Gemini recebe o histórico real e cria um plano semanal com dias, tipo de sessão, duração, pace sugerido e dicas de recuperação
- Interface console sem dependências de UI

---

## Arquitetura

O projeto segue Clean Architecture com DDD (Domain-Driven Design), dividido em 4 camadas
com dependências sempre apontando para dentro.

```
Presentation  ->  Application  ->  Domain
Infrastructure               ->  Domain
Infrastructure ->  Application
```

```
src/
├── Application/
│   ├── Interfaces/
│   │   ├── IStravaService.cs
│   │   └── ITrainingAIService.cs
│   └── UseCases/
│       └── GenerateWeeklyPlanUseCase.cs
│
├── Communication/
│   ├── Records/
│   │   ├── StravaOptions.cs
│   │   ├── StravaTokens.cs
│   │   └── TrainingPlanContext.cs
│   ├── Request/
│   │   └── GenerateWeeklyPlanRequest.cs
│   └── Response/
│       ├── GenerateWeeklyPlanResponse.cs
│       └── StravaTokenResponse.cs
│
├── Domain/
│   ├── Entities/
│   │   ├── Activity.cs
│   │   └── Athlete.cs
│   ├── Enum/
│   │   ├── ActivityType.cs
│   │   └── FitnessLevel.cs
│   ├── Extension/
│   │   └── TimeSpanExtension.cs
│   ├── Repositories/
│   │   └── Interfaces/
│   │       ├── IActivitiesRepository.cs
│   │       └── IAthleteRepository.cs
│   └── ValueObjects/
│       ├── Distance.cs
│       └── Pace.cs
│
├── Infrastructure/
│   ├── AI/
│   │   └── Gemini/
│   │       ├── Models/
│   │       │   ├── GeminiCandidate.cs
│   │       │   ├── GeminiContent.cs
│   │       │   ├── GeminiPart.cs
│   │       │   └── GeminiResponse.cs
│   │       └── GeminiService.cs
│   ├── Configuration/
│   │   ├── DependencyInjection.cs
│   │   └── OAuthCallbackServer.cs
│   └── Strava/
│       ├── Models/
│       └── StravaService.cs
│
└── Presentation/
    ├── Helpers/
    │   └── Helpers.cs
    └── Program.cs

tests/

```
```

---

## Metodologias e Princípios

### Clean Code

- Nomes expressivos que refletem o domínio da corrida (`Pace`, `FitnessLevel`, `GetActivitiesSummary`)
- Factory methods no lugar de construtores públicos — objetos sempre em estado válido
- Funções pequenas com responsabilidade única
- Mínimo de dependências externas — apenas 2 pacotes NuGet

### SOLID

| Princípio | Aplicação |
|-----------|-----------|
| SRP | Cada classe tem uma única razão para mudar — `GeminiService` só fala com a API do Gemini |
| OCP | Trocar de provedor de IA significa criar nova classe, não editar `GenerateWeeklyPlanUseCase` |
| LSP | `IStravaService` e `ITrainingAIService` têm implementações intercambiáveis |
| ISP | Interfaces pequenas e focadas por responsabilidade |
| DIP | `GenerateWeeklyPlanUseCase` depende de abstrações, nunca de classes concretas |

### DDD (Domain-Driven Design)

- **Linguagem Ubíqua** — o código usa os termos do domínio da corrida: `Pace`, `FitnessLevel`, `Activity`
- **Aggregate Root** — `Athlete` controla o acesso às suas `Activity`s e recalcula `FitnessLevel` automaticamente
- **Value Objects** — `Pace` e `Distance` são imutáveis e comparados por valor
- **Encapsulamento de lógica** — `Athlete.RecalculateFitnessLevel()` vive dentro da entidade, não em um serviço externo

---

## Tecnologias

| Tecnologia | Uso |
|------------|-----|
| C# 12 / .NET 8 | Linguagem e runtime |
| Strava API v3 | Busca de atividades e dados do atleta |
| OAuth 2.0 | Autenticação com o Strava |
| Google Gemini 2.5 Flash | Geração dos planos de treino |
| Microsoft.Extensions.DependencyInjection | Container de injeção de dependência |
| Microsoft.Extensions.Http | Gerenciamento de HttpClient |
| System.Text.Json | Serialização e deserialização JSON nativa |
| xUnit | Framework de testes |
| Moq | Mocks nos testes de Application |
| FluentAssertions | Assertions legíveis nos testes |
| MockHttp | Simulação de chamadas HTTP nos testes de integração |

---

## Como rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Conta no [Strava](https://strava.com)
- API Key gratuita no [Google AI Studio](https://aistudio.google.com)

### 1. Criar o App no Strava

1. Acesse [strava.com/settings/api](https://www.strava.com/settings/api)
2. Crie um App com **Authorization Callback Domain**: `localhost`
3. Anote o **Client ID** e o **Client Secret**

### 2. Configurar variáveis de ambiente

No Visual Studio: clique direito no projeto `Presentation` → Properties → Debug → Open debug launch profiles UI

| Variável | Descrição |
|----------|-----------|
| `STRAVA_CLIENT_ID` | Client ID do App Strava |
| `STRAVA_CLIENT_SECRET` | Client Secret do App Strava |
| `GEMINI_API_KEY` | Chave gerada no Google AI Studio |

***OU***

Também é possível criar uma cópia do .env.example e realizar o input das variaveis, não esqueça de renomear o arquivo para .env novamente para ser lido pela aplicação.

### 3. Executar

```
F5 no Visual Studio
```

No primeiro acesso o browser abre para autorizar o Strava.
Nos acessos seguintes o token é renovado silenciosamente.

---

## Testes

```bash
dotnet test
```

| Projeto | O que testa |
|---------|-------------|
| `Domain.Tests` | Value Objects (Pace, Distance) e Entities (Athlete, Activity) |
| `Application.Tests` | GenerateWeeklyPlanUseCase com mocks de Strava e Gemini |
| `Infrastructure.Tests` | StravaService e GeminiService com HTTP mockado |

---

## Fluxo da aplicação

```
1. Verifica se strava_tokens.json existe
   - Sim: renova token silenciosamente
   - Nao: abre browser para autorizacao OAuth
2. Busca atleta e ultimas 20 atividades no Strava
3. Calcula FitnessLevel baseado no pace medio real
4. Envia contexto para o Gemini AI
5. Exibe plano de treino semanal no console
```

---

## Segurança

O arquivo `strava_tokens.json` gerado localmente não deve ser commitado.
Adicione ao `.gitignore`:

```
strava_tokens.json
```

Credenciais são lidas exclusivamente via variáveis de ambiente, nunca hardcoded no código.

---

## Próximos passos

- [ ] Aplicação ser configurada por cada user
- [ ] Persistência com SGBD para histórico de planos gerados
- [ ] Cache de atividades para reduzir chamadas à API do Strava
- [ ] Interface web com ASP.NET Core + Blazor para múltiplos usuários
- [ ] Envio do plano semanal por e-mail toda segunda-feira
- [ ] Gráficos de evolução de pace ao longo das semanas
- [ ] User poder selecionar outras AI (pagas)

