# 🎮 FCG.FiapCloudGames.API

API desenvolvida para gerenciamento de usuários e jogos, com foco em boas práticas de arquitetura DDD, autenticação segura, validação robusta e testes automatizados.
- Hospedada na Azure usando Container Apps e imagem publicada no ACR (Azure Container Registry).
- [Vídeo com a apresentação da Fase 1](https://youtu.be/bmRaU8VjJZU)
- [Vídeo com a apresentação da Fase 2](https://youtu.be/BXBc6JKnRpw)

## 📌 Objetivo

Desenvolver uma API RESTful robusta e escalável, aplicando:

### **Fase 1:**
  - Domain-Driven Design (DDD) 
  - Clean Architecture 
  - Principios SOLID 
  - Middleware para log de requisições e traces 
  - Middleware de tratamento de erros centralizado
  - Exceptions personalizadas
  - Uso do Entity Framework Core com migrations
  - Autenticação baseada em JWT
  - Autorização baseada em permissões
  - Hash seguro de senhas com salt
  - Validações de domínio e DTOs
  - Testes unitários com TDD 
  - Documententação Swagger com Swashbuckle.AspNetCore
### **Fase 2:**
  - **Escalabilidade:**
    - Utilização de Docker para empacotamento da aplicação em container
    - Versionamento de imagems Docker no ACR 
    - Execução da aplicação em containers orquestrados pelo Azure Container Apps garantindo resiliência
  - **Confiabilidade:**
    - Build, testes unitários e push da imagem Docker via CI/CD multi-stage
    - Parametrização de variáveis e secrets no GitHub Environments
    - Testes de carga e performance utilziando K6
  - **Monitoramento:**
    - Traces no New Relic
    - Logs no New Relic
    - Dashboards de monitoramento (New Relic e Azure)


## 🚀 Tecnologias Utilizadas

| Tecnologia        | Versão/Detalhes                  |
|-|-|
| .NET              | .NET 8                           |
| C#                | 12                               |
| Entity Framework  | Core, com Migrations             |
| Banco de Dados    | SQL Server (ou SQLite para testes) |
| Autenticação      | JWT (Bearer Token)               |
| Testes            | xUnit, Moq, FluentAssertions     |
| Swagger           | Swashbuckle.AspNetCore           |
| Segurança         | PBKDF2 + salt com SHA256         |
| Logger            | Middleware de Request/Response + LogId |
| Docker            | Multi-stage Dockerfile para build e runtime |
| Monitoramento     | New Relic (.NET Agent) + Azure |


## 🧠 Padrões e Boas Práticas

- Camadas separadas por responsabilidade (Domain, Application, Infrastructure, API)
- Interfaces para abstração de serviços externos no domínio
- Injeção de dependência configurada via Program.cs
- Tratamento global de exceções via middleware
- DTOs com validações automáticas via DataAnnotations


## ✅ Principais Funcionalidades

### Usuários
- ✅ Criação de usuários
- ✅ Autenticação com JWT
- ✅ Hash seguro de senhas com salt
- ✅ Verificação de senha no login
- ✅ Validação de senha forte
- ✅ Validação de formato de e-mail
- ✅ Controle de permissões (admin)

### Jogos
- ✅ Cadastro e listagem de jogos
- ✅ Validação de campos e tamanho máximo
- ✅ Validação de gênero permitido
- ✅ Validação de quantidade mínima de dados enviados

### Segurança e Middleware
- ✅ Middleware de erro global
- ✅ Retorno padronizado com `ErrorResponse`
- ✅ Registro de logs com `RequestId` único
- ✅ Token JWT com verificação de permissões por endpoint


## 🧪 Testes

- ✅ Testes unitários completos de:
  - Regras de domínio
  - Hash de senhas
  - Autenticação
  - Serviços e repositórios mockados
- ✅ Cobertura de cenários felizes e inválidos
- ✅ Testes de carga e performance (utilizando K6)
  ```bash
  k6 run load-test.js
  ```

## ⚙️ Pré-requisitos
- .NET 8 SDK instalado
- SQL Server
- K6 para testes de carga (opcional)


## 🛠️ Setup do Projeto
Siga esses passos para configurar e rodar o projeto localmente:

### 
- Clonar o repositório
  ```bash
  git clone https://github.com/seu-usuario/fiap-cloud-games.git
  ```
- Configurar a conexão com o banco de dados no `appsettings.json` ou nas variáveis de ambiente
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FiapCloudGamesDb;Trusted_Connection=True;"
    },
    "Jwt": {
      "Key": "sua-chave-secreta-supersegura",
      "Issuer": "FiapCloudGames",
      "Audience": "FiapCloudGamesUsers"
    }
  }
  ```
- Executar as migrations para criar o banco de dados, passando a connectionString
  ```bash
  Update-Database -Project FCG.Infrastructure -StartupProject FCG.API -Connection "Server=(localdb)\(instance);Database=FiapCloudGamesDb;Trusted_Connection=True;TrustServerCertificate=True"
  ```
- Rodar Testes
  ```bash
  dotnet test
  ```
- Executar a aplicação
  ```bash
  dotnet run --project FCG.API
  ```
- Acessar a documentação Swagger em `http://localhost:<porta>/swagger/index.html`


 ## 🔐 Autenticação e Autorização

- Faça login com um usuário existente via /auth/login
- Use o token Bearer retornado no header Authorization das demais requisições protegidas.


 ## 📁 Estrutura de Pastas

 ```bash
FCG.FiapCloudGames/
│
├── FCG.API/                        # Camada de apresentação (Controllers, Middlewares, Program.cs)
│   ├── Controllers/                # Endpoints REST
│   ├── Middleware/                 # Tratamento de erros, logs, etc.
│   └── Program.cs                  # Ponto de entrada da aplicação
│
├── FCG.Application/                # Camada de aplicação (DTOs, serviços, interfaces de uso)
│   ├── Interfaces/                 # Interfaces usadas pela API
│   ├── Services/                   # Serviços que coordenam o domínio
│   └── DTOs/                       # Objetos de transferência de dados
│   └── Helpers/                    # Classes auxiliares
│   └── Exceptions/                 # Exceções específicas da camada de orquestração
│   └── Mappings/                   # Mapeamentos entre DTOs e entidades
│   └── Settings/                   # Configurações da aplicação
│
├── FCG.Domain/                     # Camada de domínio (regra de negócio, entidades, contratos)
│   ├── Entities/                   # Entidades como User e Game
│   ├── Exceptions/                 # Exceções do domínio
│   ├── Repositories/               # Interfaces dos repositórios (sem dependência de EF)
│
├── FCG.Infrastructure/             # Implementações (EF, hashing, repositórios concretos)
│   ├── Context/                    # DbContext do Entity Framework
│   ├── Mappings/                   # Configurações de entidades (Fluent API)
│   ├── Repositories/               # Repositórios que implementam a camada de domínio
│   └── Migrations/                 # Histórico de migrations geradas
│
├── FCG.Tests/                      # Testes automatizados (xUnit)
│   ├── UnitTests/                  # Testes Unitários
│       ├── Domain/                 # Testes de entidades, regex e regras
│       ├── Application/            # Testes de serviços (ex: UserService)
│       ├── Infrastructure/         # Testes de hashing, token, etc.
│       └── Helpers/                # Setup de mocks e objetos fake
│   ├── IntegrationTests/           # Testes de Integração
│
├── FCG.Documentation/              # Documentação do projeto
├── .github/                        # Configurações do GitHub Actions para CI/CD
│
├── .gitattributes                  # Configurações do Git
├── .gitigore                       # Arquivo para ignorar arquivos no Git
├── load-test.js                    # Script de teste de carga com K6
├── Dockerfile                      # Dockerfile para containerização
├── README.md                       # Este arquivo
└── 
 ```


## 🔗 Diagrama de Relacionamento (Simplificado)
```plaintext
+------------------+           +--------------------+           +------------------+            +------------------+
|     Users        |<--------->|   Request_log      |<--------->|    Trace_log     |            |      Games       |
+------------------+   (1:N)   +--------------------+   (1:N)   +------------------+            +------------------+
| UserId (PK)      |           | LogId (PK)         |           | TraceId (PK)     |            | GameId (PK)      |
| Name             |           | UserId (FK)        |           | LogId (FK)       |            | Name             |
| Email            |           | HttpMethod         |           | Timestamp        |            | Description      |
| PasswordHash     |           | Path               |           | Level            |            | Genre            |
| IsActive         |           | StatusCode         |           | Message          |            | ReleaseDate      |
| CreatedAt        |           | RequestBody        |           | StackTrace       |            | CreatedAt        |
| UpdatedAt        |           | ResponseBody       |           +------------------+            | UpdatedAt        |
| IsAdmin          |           | StartDate          |                                           | Rating           |
+------------------+           | EndDate            |                                           +------------------+ 
                               | Duration           |
                               +--------------------+       
```


## 🚀 Pipeline CI/CD

O workflow está definido em `.github/workflows/ci-cd-fcg.yml`. 
Automatizando os seguintes passos:

- Build e testes unitários
- Build da imagem Docker
- Push para Azure Container Registry (ACR)
- MultiStage para Deploy automatizado no Azure Container Apps:
   - DEV
   - UAT (necessário aprovação)
   - PRD (apenas com PR na branch `master` e necessário aprovação)
   

## ☁️ Infraestrutura na Azure

O projeto utiliza os seguintes recursos na Azure:

- **Azure Resource Group**: `RG_FCG`
- **Azure SQL Database**: `fiapcloudgamesdb`
- **Azure Container Registry (ACR)**: `acrfcg.azurecr.io`
- **Azure Container Apps**:
  - DEV: `aca-fcg-dev` 
  - UAT: `aca-fcg-uat` 
  - PRD: `aca-fcg` 

As variáveis de ambiente sensíveis (como strings de conexão) são gerenciadas via Azure e GitHub Secrets.
[Link para o desenho de infraestrutura](https://miro.com/app/board/uXjVIteOb6w=/?share_link_id=230805148396)

## 🐳Dockerfile e 📊Monitoramento

Este projeto utiliza um Dockerfile em duas etapas para garantir uma imagem otimizada e segura:

- **Stage 1 - Build**: Usa a imagem oficial do .NET SDK 8.0 para restaurar dependências, compilar e publicar a aplicação em modo Release.
- **Stage 2 - Runtime**: Utiliza a imagem mais leve do ASP.NET 8.0 para executar a aplicação, copiando apenas os artefatos publicados da etapa de build, o que reduz o tamanho final da imagem.

Além disso, o agente do **New Relic** é instalado na imagem de runtime para habilitar monitoramento detalhado da aplicação. As variáveis de ambiente necessárias para a configuração do agente são definidas no Dockerfile, podendo ser sobrescritas via ambiente de execução (ex.: Kubernetes, Azure Container Apps).

Esse processo segue as melhores práticas:

- **Multi-stage build:** mantém a imagem final enxuta e rápida para deploy.
- **Separação clara:** entre build e runtime para evitar expor ferramentas de desenvolvimento.
- **Instalação do agente New Relic:** automatizada e integrada para facilitar o monitoramento.
- **Configuração via variáveis de ambiente:** flexível e segura para licenças e nomes de aplicação.

 ## ✍️ Autor
- Frank Vieira
- GitHub: @fkwesley
- Projeto desenvolvido para fins educacionais no curso da FIAP.
 
