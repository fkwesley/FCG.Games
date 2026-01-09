# 🎮 FCG.Games.API

API desenvolvida para gerenciamento de jogos, com foco em micro-serviços e arquitetura orientada a eventos.
- Hospedada na Azure usando Kubernetes Services e imagem publicada no ACR (Azure Container Registry).
- [Vídeo com a apresentação da Fase 1](https://youtu.be/bmRaU8VjJZU)
- [Vídeo com a apresentação da Fase 2](https://youtu.be/BXBc6JKnRpw)
- [Vídeo com a apresentação da Fase 3](https://youtu.be/3OxTOgieuMg)
- [Vídeo com a apresentação da Fase 4](https://youtu.be/3OxTOgieuMg)

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
### **Fase 3:** 
  - **Migração arquitetura Monolitica x Micro-serviços:**
    - Separação da API em serviços distintos com base nos contextos delimitados (Users, Games, Orders, Payments)
    - Cada API com seu próprio repositório e infraestrutura (banco de dados, container app e pipeline CI/CD)
  - **Adoção de soluções Serverless:**
    - Arquitetura orientada a eventos com comunicação assíncrona via mensageria (Azure Service Bus)
    - Utilização de Azure Functions como gatilho das mensagens do Service Bus (Tópicos e Subscriptions)
    - Utilização do Azure API Management para gerenciamento e segurança das APIs com políticas de rate limit e cache
  - **Otimização na busca de jogos:**
    - Implementação de ElasticSearch para indexação dos jogos e logs 
    - Ganho de performance com consultas avançadas
    - Implementação de filtros, paginação e ordenação, inclusive endpoint de jogos mais bem avaliados
### **Fase 4:**     
  - **Orquestração de Containers usando Kubernetes:**
    - Migração das aplicações hospedadas em Azure Container Apps (ACA) para Azure Kubernetes Services (AKS)
    - Implementação de HPA (Horizontal Pod AutoScaler) para escalonamento horizontal automatico
    - Implementação de configMap e secrets do Kubernetes para gerenciamento de configurações sensíveis
    - Implementação de Health Probes para garantir a disponibilidade da aplicação
    - Implementação de Deployments e Services para gerenciamento dos pods e exposição das aplicações
    - Implementação de Statefulset e PVC (Persistent Volume Claim) para serviços que necessitam de persistência de dados
  - **Comunicação Assíncrona entre serviços:**
    - Utilização de filas e tópicos no RabbitMQ e ServiceBus para enfilerar requisições e garantir resiliência 
  - **Otimização das imagens Docker**
    - Migração versão da imagem Docker do .NET para uma versão mais leve, otimizando recursos dos containers
    - Aplicações adaptadas para trabalhar com a versão mais leve
    - Redução de aproximadamente 50% do tamanho das imagens
  - **Monitoramento
    - Elastic.APM instrumentado nas apis e no worker service
    - dashboards com métricas de CPU, memória, requisições, pods...

## 🚀 Tecnologias Utilizadas

| Tecnologia            | Versão/Detalhes                                           |
|-|-|
| .NET                  | .NET 8                                                    |
| C#                    | 12                                                        |
| Entity Framework      | Core, com Migrations                                      |
| Banco de Dados        | SQL Server (ou SQLite para testes)                        |
| Autenticação          | JWT (Bearer Token)                                        |
| Testes                | xUnit, Moq, FluentAssertions                              |
| Swagger               | Swashbuckle.AspNetCore                                    |
| Segurança             | PBKDF2 + salt com SHA256                                  |
| Logger                | Middleware de Request/Response + LogId                    |
| Docker                | Multi-stage Dockerfile para build e runtime               |
| Monitoramento         | Elastic.APM + New Relic (.NET Agent) + Azure              |
| Mensageria            | Azure Service Bus (Tópicos e Subscriptions) + RabbitMQ    |
| Consumer de Mensagens | Azure Functions                                           |
| Orquestração          | Azure Kubernetes Services                                 |
| API Gateway           | Azure API Management                                      |
| CI/CD                 | GitHub Actions                                            |
| Testes de Carga       | K6                                                        |
| ElasticSearch         | Indexação e busca avançada                                |


## 🧠 Padrões e Boas Práticas

- Camadas separadas por responsabilidade (Domain, Application, Infrastructure, API)
- Interfaces para abstração de serviços externos no domínio
- Injeção de dependência configurada via Program.cs
- Tratamento global de exceções via middleware
- DTOs com validações automáticas via DataAnnotations


## ✅ Principais Funcionalidades

### Jogos
- ✅ Criação de jogos + Disparo para fila de pagamento de forma assíncrona
- ✅ Listagem de jogos
- ✅ Consulta de jogo por id
- ✅ Consulta avançada de jogos com filtros, Fuzzy Search, paginação e ordenação (com ElasticSearch)
- ✅ Atualização de status dos jogos
- ✅ Cancelamento de jogos
- ✅ Relatório de jogos mais bem avaliados (com ElasticSearch)

### Segurança e Middleware
- ✅ Middleware de erro global
- ✅ Retorno padronizado com `ErrorResponse`
- ✅ Registro de logs com `RequestId` único
- ✅ Autenticação com Token JWT gerado pela [FCG.Users.API](https://github.com/fkwesley/FCG.Users) 
- ✅ Verificação de permissões por endpoint

## 🧪 Testes

- ✅ Testes unitários completos
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
  git clone https://github.com/fkwesley/FCG.Games.git
  ```
- Configurar conexões com o banco de dados e servicebus no `appsettings.json` ou nas variáveis de ambiente
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FiapCloudGamesDb;Trusted_Connection=True;",
      "ServiceBusConnection": "Endpoint=sb://<NAMESPACE>.servicebus.windows.net/;SharedAccessKeyName=<KEY_NAME>;SharedAccessKey=<KEY_VALUE>"
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
  Update-Database -Project Infrastructure -StartupProject API -Connection "Server=(server)\(instance);Database=(dbname);Trusted_Connection=True;TrustServerCertificate=True"
  ```
- Rodar Testes
  ```bash
  dotnet test
  ```
- Executar a aplicação
  ```bash
  dotnet run --project API
  ```
- Acessar a documentação Swagger em `http://localhost:<porta>/swagger/index.html`


 ## 🔐 Autenticação e Autorização

- Faça login com um usuário existente via users/auth/login
- Use o token Bearer retornado no header Authorization das demais requisições protegidas.


 ## 📁 Estrutura de Pastas

 ```bash
FCG.Games.API/
│
├── API/                        # Camada de apresentação (Controllers, Middlewares, Program.cs)
│   ├── Controllers/                # Endpoints REST
│   ├── Middleware/                 # Tratamento de erros, logs, etc.
│   └── Program.cs                  # Ponto de entrada da aplicação
│
├── Application/                # Camada de aplicação (DTOs, serviços, interfaces de uso)
│   ├── Interfaces/                 # Interfaces usadas pela API
│   ├── Services/                   # Serviços que coordenam o domínio
│   └── DTOs/                       # Objetos de transferência de dados
│   └── Helpers/                    # Classes auxiliares
│   └── Exceptions/                 # Exceções específicas da camada de orquestração
│   └── Mappings/                   # Mapeamentos entre DTOs e entidades
│   └── Settings/                   # Configurações da aplicação
│
├── Domain/                     # Camada de domínio (regra de negócio, entidades, contratos)
│   ├── Entities/                   # Entidades como User e Game
│   ├── Exceptions/                 # Exceções do domínio
│   ├── Repositories/               # Interfaces dos repositórios (sem dependência de EF)
│
├── Infrastructure/             # Implementações (EF, hashing, repositórios concretos)
│   ├── Configurations/             # Configurações do EF (ex: SqlServerConfig)
│   ├── Context/                    # DbContext do Entity Framework
│   ├── Interfaces/                 # Interfaces de serviços externos
│   ├── Mappings/                   # Configurações de entidades (Fluent API)
│   ├── Repositories/               # Repositórios que implementam a camada de domínio
│   └── Migrations/                 # Histórico de migrations geradas
│   └── Services/                   # Implementações de serviços externos/infra (ex: ServiceBusService)
│
├── Tests/                      # Testes automatizados (xUnit)
│   ├── UnitTests/                  # Testes Unitários
│       ├── Domain/                 # Testes de entidades, regex e regras
│       ├── Application/            # Testes de serviços (ex: UserService)
│       ├── Infrastructure/         # Testes de hashing, token, etc.
│       └── Helpers/                # Setup de mocks e objetos fake
│
├── Kubernetes/                 # Manifests para deploy no AKS
├── Documentation/              # Documentação do projeto
├── .github/                    # Configurações do GitHub Actions para CI/CD
│
├── .gitattributes              # Configurações do Git
├── .gitigore                   # Arquivo para ignorar arquivos no Git
├── load-test.js                # Script de teste de carga com K6
├── Dockerfile                  # Dockerfile para containerização
├── README.md                   # Este arquivo
└── 
 ```


## 🔗 Diagrama de Relacionamento (Simplificado)
```plaintext
+--------------------+           +------------------+        +------------------+
|   Request_log      |<--------->|    Trace_log     |        |      Games       |
+--------------------+   (1:N)   +------------------+        +------------------+
| LogId (PK)         |           | TraceId (PK)     |        | GameId (PK)      |
| UserId (FK)        |           | LogId (FK)       |        | Name             |
| HttpMethod         |           | Timestamp        |        | Description      |
| Path               |           | Level            |        | Genre            |
| StatusCode         |           | Message          |        | ReleaseDate      |
| RequestBody        |           | StackTrace       |        | CreatedAt        |
| ResponseBody       |           +------------------+        | UpdatedAt        |     
| StartDate          |                                       | Rating           |
| Duration           |                                       | Price            |
+--------------------+                                       +------------------+
```


## 🚀 Pipeline CI/CD

O workflow está definido em `.github/workflows/ci-cd-aks.yml`. 
Automatizando os seguintes passos:

- Build e testes unitários
- Build da imagem Docker
- Push para Azure Container Registry (ACR)
- MultiStage para Deploy automatizado no Azure Kubernetes Services:
   - DEV
   - UAT (necessário aprovação)
   - PRD (apenas com PR na branch `master` e necessário aprovação)
   

## ☁️ Infraestrutura na Azure

O projeto utiliza os seguintes recursos na Azure:

- **Azure Resource Group**: `RG_FCG`
- **Azure SQL Database**: `FCG.UsersDB`
- **Azure Container Registry (ACR)**: `acrfcg.azurecr.io`
- **Azure Kubernetes Services (AKS)**: `aks-fcg-notification`
- **Azure Api Management**: `apim-fcg`
- **Azure Service Bus**: `servicebus-fcg`
- **Azure Functions**: `func-fcg-payments`
- **RabbitMQ**: `fcg.notification.queue`

As variáveis de ambiente sensíveis (como strings de conexão) são gerenciadas via Azure e GitHub Secrets.
[Link para o desenho de infraestrutura](https://miro.com/app/board/uXjVIteOb6w=/?share_link_id=230805148396)

## 🐳Dockerfile e 📊Monitoramento

Este projeto utiliza um Dockerfile em duas etapas para garantir uma imagem otimizada e segura:

- **Stage 1 - Build**: Usa a imagem oficial do .NET SDK 8.0 para restaurar dependências, compilar e publicar a aplicação em modo Release.
- **Stage 2 - Runtime**: Utiliza a versão alpine (mais leve do ASP.NET 8.0) para executar a aplicação, copiando apenas os artefatos publicados da etapa de build, o que reduz o tamanho final da imagem.

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