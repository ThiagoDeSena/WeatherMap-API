# 🌤️ WeatherMap API

Uma API robusta .NET 8 para consulta e armazenamento de dados meteorológicos, integrando com a API Open-Meteo para fornecer informações climáticas em tempo real e análises históricas avançadas.

<div align="center">
  <img src="https://github.com/user-attachments/assets/07e73d9f-ddbe-46df-a47c-5201875e8382" alt="Endpoints da WeatherMap API" width="600" style="border: 1px solid #ddd; border-radius: 8px; padding: 10px; background: white;">
</div>

## 📋 Índice

- [Visão Geral do Projeto](#-visão-geral-do-projeto)
- [Funcionalidades Implementadas](#-funcionalidades-implementadas)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Arquitetura da Solução](#-arquitetura-da-solução)
- [Pré-requisitos](#-pré-requisitos)
- [Configuração e Instalação](#-configuração-e-instalação)
- [Endpoints da API](#-endpoints-da-api)
- [Demonstração de Consultas SQL Brutas](#-demonstração-de-consultas-sql-brutas)
- [Exemplos de Uso Completos](#-exemplos-de-uso-completos)
- [API Externa - Open-Meteo](#-api-externa---open-meteo)
- [Estrutura do Banco de Dados](#-estrutura-do-banco-de-dados)
- [Tratamento de Erros](#-tratamento-de-erros)
- [Práticas de Desenvolvimento](#-práticas-de-desenvolvimento)
- [Segurança e Boas Práticas](#-segurança-e-boas-práticas)
- [Recursos Extras Implementados](#-recursos-extras-implementados)

## 🎯 Visão Geral do Projeto

### Objetivo Principal
Demonstrar competências em desenvolvimento backend .NET 8 através de uma API completa que:
- **Integra com API externa** (Open-Meteo) para obtenção de dados meteorológicos
- **Processa e transforma** dados JSON complexos em modelos tipados
- **Persiste dados** em banco MySQL com relacionamentos 1:N
- **Oferece CRUD completo** com operações avançadas
- **Utiliza consultas SQL brutas** para demonstrar conhecimento em banco de dados

### Diferenciais Técnicos
- ✅ **Arquitetura em camadas** (Controllers → Services → Data)
- ✅ **Dependency Injection** nativo do .NET
- ✅ **Entity Framework Core** com Code-First Migrations
- ✅ **Consultas SQL raw** para performance crítica
- ✅ **Logging estruturado** com ILogger
- ✅ **Documentação OpenAPI/Swagger** completa
- ✅ **Tratamento de exceções** robusto

## ✨ Funcionalidades Implementadas

### 📡 Integração com API Externa
- [x] Consumo da **Open-Meteo API** via HttpClient
- [x] **Geocoding** para conversão cidade → coordenadas
- [x] **Forecast API** para dados meteorológicos detalhados
- [x] **Transformação de dados** JSON → DTOs → Models
- [x] **Health check** da API externa

### 💾 Persistência e Banco de Dados
- [x] **MySQL** como SGBD principal
- [x] **Entity Framework Core** 7.0 com Pomelo.MySQL
- [x] **Code-First Migrations** para versionamento do schema
- [x] **Relacionamentos 1:N** (WeatherHistory → DailyForecasts)
- [x] **Consultas SQL brutas** para análises complexas
- [x] **Índices e constraints** otimizados

### 🔧 CRUD Completo
- [x] **CREATE**: Salvar novos dados meteorológicos
- [x] **READ**: Consultas por ID, localização, coordenadas, histórico
- [x] **UPDATE**: Atualizar informações de localização
- [x] **DELETE**: Remover registros específicos e limpeza automática

### 📊 Análises e Relatórios
- [x] **Tendências temporais** de temperatura
- [x] **Estatísticas por localização** (média, min, max)
- [x] **Comparação entre cidades** múltiplas
- [x] **Health check do banco** via SQL bruta

## 🛠️ Tecnologias Utilizadas

### Backend Framework
- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - Criação de APIs REST

### Banco de Dados
- **MySQL 8.0+** - Sistema de gerenciamento de banco
- **Entity Framework Core 7.0** - ORM para .NET
- **Pomelo.EntityFrameworkCore.MySql 7.0** - Provider MySQL

### Documentação e Testes
- **Swashbuckle.AspNetCore 6.4** - Geração automática de documentação OpenAPI
- **ILogger** - Sistema de logging nativo do .NET

### API Externa
- **Open-Meteo API** - Dados meteorológicos gratuitos
- **HttpClient** - Cliente HTTP nativo para consumo de APIs

## 🏗️ Arquitetura da Solução

```
WeatherMap/
├── Controllers/           # Endpoints da API
│   ├── WeatherController.cs    # CRUD completo + Analytics
│   └── TestController.cs       # Testes da API externa
├── Services/              # Lógica de negócio
│   ├── IWeatherService.cs      # Interface para API externa
│   ├── OpenMeteoService.cs     # Implementação Open-Meteo
│   ├── IWeatherDatabaseService.cs  # Interface para banco
│   └── WeatherDatabaseService.cs   # Implementação banco
├── Models/                # Entidades do banco
│   ├── WeatherHistory.cs       # Histórico climático
│   └── DailyForecast.cs        # Previsões diárias
├── DTOs/                  # Objetos de transferência
│   ├── WeatherResponse.cs      # Response da API externa
│   ├── CurrentWeatherDto.cs    # Dados atuais
│   └── DailyForecastDto.cs     # Previsão diária
├── Data/                  # Contexto do banco
│   └── AppDbContext.cs         # Entity Framework Context
└── Configurations/       # Configurações
    └── ApiSettings.cs          # Settings da API externa
```

## 📦 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) ou superior
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/)
- [MySQL Workbench](https://dev.mysql.com/downloads/workbench/) (recomendado)
- [Git](https://git-scm.com/) para controle de versão
- IDE: Visual Studio 2022+ ou VS Code

## ⚙️ Configuração e Instalação

### 1. Clone e Configure o Projeto
```bash
# Clone o repositório
git clone <url-do-repositorio>
cd WeatherMap

# Restaure as dependências
dotnet restore

# Verifique se o build está funcionando
dotnet build
```

### 2. Configure o Banco de Dados
```bash
# Copie o arquivo de configuração template
cp appsettings.json.example appsettings.json
```

### 3. Edite as Configurações
Edite o arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=weathermapdb;user=root;password=SUA_SENHA;"
  },
  "ApiSettings": {
    "OpenMeteoBaseUrl": "https://api.open-meteo.com/v1",
    "GeocodingBaseUrl": "https://geocoding-api.open-meteo.com/v1"
  }
}
```

### 4. Execute as Migrations
```bash
# Crie a primeira migration (se necessário)
dotnet ef migrations add InitialCreate

# Aplique as migrations ao banco
dotnet ef database update
```

### 5. Execute a Aplicação
```bash
# Inicie a aplicação
dotnet run

# Ou para development com hot reload
dotnet watch run
```

### 6. Acesse a Documentação
- **Swagger UI**: http://localhost:5082/swagger
- **Health Check**: http://localhost:5082/health
- **Endpoint raiz**: http://localhost:5082/

## 🌐 Endpoints da API

### 📍 Operações Principais (CRUD)

#### Buscar e Salvar Dados
```http
POST /api/Weather/fetch-and-save/city/{cityName}
POST /api/Weather/fetch-and-save/coordinates?latitude={lat}&longitude={lon}
```

#### Consultar Dados Salvos
```http
GET /api/Weather/history?limit=50
GET /api/Weather/saved/{id}
GET /api/Weather/saved/location/{locationName}
```

#### Atualizar e Deletar
```http
PUT /api/Weather/saved/{id}/location     # Atualizar localização
DELETE /api/Weather/saved/{id}           # Deletar registro
DELETE /api/Weather/cleanup?daysOld=90   # Limpeza automática
```

### 📊 Analytics e Consultas SQL Brutas

#### Estatísticas Avançadas
```http
GET /api/Weather/analytics/trends/{locationName}?days=30
GET /api/Weather/analytics/locations-stats-raw?days=30
GET /api/Weather/analytics/temperature-trends-raw/{locationName}?days=30
GET /api/Weather/analytics/database-health-raw
POST /api/Weather/analytics/location-comparison-raw
```

### 🧪 Endpoints de Teste
```http
GET /api/Test/health                     # Health check API externa
GET /api/Test/weather/city/{cityName}    # Teste geocoding + forecast
GET /api/Test/weather/coordinates        # Teste forecast por coordenadas
GET /api/Test/geocoding/{cityName}       # Teste geocoding isolado
```

## 🔍 Demonstração de Consultas SQL Brutas

### Exemplo 1: Estatísticas por Localização
```csharp
public async Task<List<WeatherLocationStats>> GetLocationStatisticsRawAsync(int days = 30)
{
    var sql = @"
        SELECT 
            LocationName,
            COUNT(*) as TotalRecords,
            ROUND(AVG(CurrentTemperature), 2) as AvgTemperature,
            ROUND(MAX(CurrentTemperature), 2) as MaxTemperature,
            ROUND(MIN(CurrentTemperature), 2) as MinTemperature,
            MIN(CreatedAt) as FirstRecord,
            MAX(CreatedAt) as LastRecord
        FROM WeatherHistories 
        WHERE CreatedAt >= {0}
        GROUP BY LocationName 
        ORDER BY TotalRecords DESC";

    return await _context.Database
        .SqlQueryRaw<WeatherLocationStats>(sql, cutoffDate)
        .ToListAsync();
}
```

### Exemplo 2: Tendências Temporais
```csharp
public async Task<List<WeatherTrendData>> GetTemperatureTrendsRawAsync(string locationName, int days = 30)
{
    var sql = @"
        SELECT 
            DATE(CreatedAt) as Date,
            ROUND(AVG(CurrentTemperature), 2) as AvgTemp,
            ROUND(MAX(CurrentTemperature), 2) as MaxTemp,
            ROUND(MIN(CurrentTemperature), 2) as MinTemp,
            COUNT(*) as RecordCount
        FROM WeatherHistories 
        WHERE LocationName LIKE {0} AND CreatedAt >= {1}
        GROUP BY DATE(CreatedAt)
        ORDER BY Date DESC";

    return await _context.Database
        .SqlQueryRaw<WeatherTrendData>(sql, $"%{locationName}%", cutoffDate)
        .ToListAsync();
}
```

## 📋 Exemplos de Uso Completos

### 1. Consulta e Armazenamento
```bash
curl -X POST "http://localhost:5082/api/Weather/fetch-and-save/city/Fortaleza?forecastDays=7" \
  -H "Content-Type: application/json"
```

### 2. Resposta Típica
```json
{
  "success": true,
  "message": "Dados climáticos obtidos e salvos com sucesso",
  "savedId": 15,
  "location": "Fortaleza, Brasil",
  "retrievedAt": "2025-09-14T21:24:50.981406Z",
  "data": {
    "latitude": -3.7304,
    "longitude": -38.5267,
    "timezone": "America/Fortaleza",
    "current": {
      "time": "2025-09-14T18:30:00",
      "temperature": 28.5,
      "feelsLike": 31.2,
      "humidity": 75,
      "pressure": 1013.2,
      "windSpeed": 12.5,
      "weatherDescription": "Parcialmente nublado",
      "isDay": true
    },
    "dailyForecast": [
      {
        "date": "2025-09-14T00:00:00",
        "temperatureMax": 31.2,
        "temperatureMin": 24.1,
        "precipitationSum": 2.5,
        "precipitationProbability": 45,
        "windSpeedMax": 15.3,
        "weatherDescription": "Chuva leve"
      }
    ]
  }
}
```

### 3. Consulta de Analytics
```bash
curl -X GET "http://localhost:5082/api/Weather/analytics/locations-stats-raw?days=30"
```

### 4. Comparação entre Cidades
```bash
curl -X POST "http://localhost:5082/api/Weather/analytics/location-comparison-raw" \
  -H "Content-Type: application/json" \
  -d '{
    "locationNames": ["Fortaleza", "São Paulo", "Rio de Janeiro"]
  }'
```

## 🔗 API Externa - Open-Meteo

### Documentação Oficial
- **Website**: https://open-meteo.com/
- **Forecast API**: https://open-meteo.com/en/docs
- **Geocoding API**: https://open-meteo.com/en/docs/geocoding-api

### Por que Open-Meteo?
- ✅ **Gratuita** para uso não-comercial
- ✅ **Sem necessidade de API Key**
- ✅ **Dados globais** de alta qualidade
- ✅ **Múltiplos formatos** (JSON, CSV)
- ✅ **Previsão de até 7 dias**
- ✅ **Dados históricos** disponíveis

### Endpoints Utilizados
```http
# Geocoding - Converter cidade em coordenadas
GET https://geocoding-api.open-meteo.com/v1/search?name={city}&count=1

# Forecast - Obter dados meteorológicos
GET https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,relative_humidity_2m&daily=temperature_2m_max,temperature_2m_min
```

## 🗃️ Estrutura do Banco de Dados

### Diagrama ER Conceitual
```
WeatherHistory (1) -----> (N) DailyForecast
     |                           |
     |- Id (PK)                  |- Id (PK)
     |- LocationName             |- WeatherHistoryId (FK)
     |- Latitude                 |- ForecastDate
     |- Longitude                |- TemperatureMax
     |- CurrentTemperature       |- TemperatureMin
     |- CurrentHumidity          |- PrecipitationSum
     |- CreatedAt                |- WindSpeedMax
     |- RetrievedAt              |- WeatherDescription
```

### Script de Criação (MySQL)
```sql
-- Tabela principal de histórico climático
CREATE TABLE WeatherHistories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    LocationName VARCHAR(200) NOT NULL,
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    Timezone VARCHAR(100),
    CurrentTime DATETIME NOT NULL,
    CurrentTemperature DECIMAL(5,2) NOT NULL,
    CurrentFeelsLike DECIMAL(5,2) NOT NULL,
    CurrentHumidity INT NOT NULL,
    CurrentPressure DECIMAL(7,2) NOT NULL,
    CurrentWindSpeed DECIMAL(5,2) NOT NULL,
    CurrentWindDirection DECIMAL(5,2) NOT NULL,
    CurrentCloudCover INT NOT NULL,
    CurrentPrecipitation DECIMAL(5,2) NOT NULL,
    CurrentWeatherDescription VARCHAR(100),
    CurrentIsDay BOOLEAN NOT NULL,
    RetrievedAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_location (LocationName),
    INDEX idx_coordinates (Latitude, Longitude),
    INDEX idx_created_at (CreatedAt)
);

-- Tabela de previsões diárias
CREATE TABLE DailyForecasts (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    WeatherHistoryId INT NOT NULL,
    ForecastDate DATE NOT NULL,
    TemperatureMax DECIMAL(5,2) NOT NULL,
    TemperatureMin DECIMAL(5,2) NOT NULL,
    PrecipitationSum DECIMAL(5,2) NOT NULL,
    PrecipitationProbability INT NOT NULL,
    WindSpeedMax DECIMAL(5,2) NOT NULL,
    WeatherDescription VARCHAR(100),
    
    FOREIGN KEY (WeatherHistoryId) REFERENCES WeatherHistories(Id) ON DELETE CASCADE,
    INDEX idx_weather_history (WeatherHistoryId),
    INDEX idx_forecast_date (ForecastDate)
);
```

## ⚠️ Tratamento de Erros

### Estratégias Implementadas

#### 1. Validação de Entrada
```csharp
if (forecastDays < 1 || forecastDays > 7)
{
    return BadRequest(new { message = "forecastDays deve estar entre 1 e 7" });
}
```

#### 2. Logging Estruturado
```csharp
_logger.LogInformation("Buscando dados climáticos para cidade: {City}", cityName);
_logger.LogError(ex, "Erro ao salvar dados climáticos para {Location}", location);
```

#### 3. Try-Catch Consistente
```csharp
try
{
    var weatherData = await _weatherService.GetWeatherByCityAsync(cityName);
    // ... lógica
    return Ok(result);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro específico da operação");
    return StatusCode(500, new { error = "Erro interno", message = ex.Message });
}
```

#### 4. Códigos de Status HTTP Apropriados
- `200 OK` - Operação realizada com sucesso
- `400 Bad Request` - Parâmetros inválidos
- `404 Not Found` - Recurso não encontrado
- `500 Internal Server Error` - Erro interno do servidor

## 🔧 Práticas de Desenvolvimento

### Padrões Arquiteturais
- **Repository Pattern** via Entity Framework
- **Dependency Injection** nativo do .NET Core
- **Separation of Concerns** (Controllers → Services → Data)
- **Interface Segregation** (IWeatherService, IWeatherDatabaseService)

### Code Quality
- **Naming Conventions** C# padrão (.NET Guidelines)
- **Async/Await** para operações I/O
- **LINQ** para consultas em memória
- **Data Annotations** para validação de modelos

### Git Strategy
```bash
# Commits estruturados e descritivos
feat: add weather data persistence with MySQL
fix: handle null location names in geocoding
docs: update README with SQL raw queries examples
refactor: extract weather service interface
```

## 🔒 Segurança e Boas Práticas

### Proteção de Dados Sensíveis

#### .gitignore Configurado
```gitignore
# Configurações sensíveis
appsettings.json
appsettings.Production.json
appsettings.Development.json

# Logs e cache
*.log
obj/
bin/
.vs/
*.user
*.suo

# Banco de dados local
*.db
*.sqlite

# Variáveis de ambiente
.env*
```

#### Template de Configuração
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Port=3306;Database=SEU_BANCO;user=SEU_USUARIO;password=SUA_SENHA;"
  },
  "ApiSettings": {
    "OpenMeteoBaseUrl": "https://api.open-meteo.com/v1",
    "GeocodingBaseUrl": "https://geocoding-api.open-meteo.com/v1"
  }
}
```

### SQL Injection Prevention
- **Parâmetros tipados** em consultas SQL brutas
- **Entity Framework** como primeira linha de defesa
- **Validação de entrada** em todos os endpoints

### CORS e Headers
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## 🚀 Recursos Extras Implementados

### 1. Consultas SQL Brutas Avançadas
- Agregações complexas (AVG, MIN, MAX, COUNT)
- JOINs entre tabelas relacionadas
- Subconsultas para rankings
- Parâmetros seguros contra SQL Injection

### 2. Analytics e Business Intelligence
- Tendências temporais de temperatura
- Estatísticas comparativas entre cidades
- Padrões climáticos mais frequentes
- Health check do banco de dados

### 3. Operações de Manutenção
- Limpeza automática de dados antigos
- Atualização de informações de localização
- Monitoramento de performance
- Logs estruturados para debugging

### 4. Documentação Swagger Completa
- Descrições detalhadas de todos os endpoints
- Exemplos de request/response
- Modelos de dados documentados
- Interface interativa para testes

### 5. Health Checks
- Status da API externa (Open-Meteo)
- Conectividade com banco de dados
- Endpoints de monitoramento
- Métricas de performance

## 🏆 Critérios de Avaliação Atendidos

### ✅ Organização do Código
- Arquitetura em camadas bem definida
- Separação de responsabilidades clara
- Interfaces e implementações desacopladas
- Estrutura de pastas organizada

### ✅ Boas Práticas
- Dependency Injection
- Async/Await para operações I/O
- Logging estruturado
- Tratamento de exceções consistente

### ✅ Tratamento de Erros
- Try-catch em todos os métodos críticos
- Códigos de status HTTP apropriados
- Logging de erros detalhado
- Validação de entrada robusta

### ✅ Uso do Banco de Dados
- Entity Framework Core com relacionamentos
- Migrations para versionamento
- Consultas SQL brutas para performance
- Índices otimizados

### ✅ Documentação
- README.md completo e detalhado
- Swagger/OpenAPI configurado
- Comentários XML nos controllers
- Exemplos de uso práticos

### ✅ Recursos Extras
- Analytics avançados
- Consultas SQL complexas
- Health checks
- Operações de manutenção

## 📞 Informações de Contato

- **Desenvolvedor**: Thiago de Sena
- **Email**: thiagosena316@gmail.com
- **LinkedIn**: [Thiago de Sena Developer](https://www.linkedin.com/in/thiago-de-sena-developer/)
- **Documentação API**: `/swagger` (ambiente local)

## 📄 Licença e Uso

Este projeto foi desenvolvido para fins **educacionais e demonstrativos** como parte de uma avaliação técnica para **Desenvolvedor Backend Júnior**.

### Recursos Utilizados
- **Open-Meteo API** - Dados meteorológicos gratuitos
- **.NET 8** - Framework Microsoft
- **MySQL Community** - Sistema de banco de dados
- **Entity Framework Core** - ORM Microsoft

---

**Desenvolvido com ❤️ usando .NET 8 e boas práticas de desenvolvimento backend.**
