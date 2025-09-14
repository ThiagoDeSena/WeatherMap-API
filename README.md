# 🌤️ WeatherMap API

Uma API robusta para consulta e armazenamento de dados meteorológicos, integrando com a API Open-Meteo para fornecer informações climáticas em tempo real e históricas.

<div align="center">
  <img src="https://github.com/user-attachments/assets/07e73d9f-ddbe-46df-a47c-5201875e8382" alt="Endpoints da WeatherMap API" width="600" style="border: 1px solid #ddd; border-radius: 8px; padding: 10px; background: white;">
</div>

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Pré-requisitos](#-pré-requisitos)
- [Configuração](#-configuração)
- [Endpoints](#-endpoints)
- [Exemplos de Uso](#-exemplos-de-uso)
- [API Externa](#-api-externa)
- [Estrutura do Banco](#-estrutura-do-banco)
- [Segurança](#-segurança)

## 🎯 Visão Geral

A WeatherMap API permite consultar dados meteorológicos de qualquer localidade do mundo através da integração com a Open-Meteo API, persistindo as informações em um banco de dados MySQL para análise histórica e tendências.

## ✨ Funcionalidades

- ✅ **Consulta em tempo real** de condições climáticas
- ✅ **Previsão de 7 dias** com dados detalhados
- ✅ **Persistência em banco** MySQL para histórico
- ✅ **CRUD completo** dos dados meteorológicos
- ✅ **Análises e estatísticas** avançadas
- ✅ **Consultas SQL raw** para performance
- ✅ **Documentação Swagger** completa
- ✅ **Health checks** e monitoramento

## 🛠️ Tecnologias

- **.NET 8.0** - Framework principal
- **Entity Framework Core 7.0** - ORM para MySQL
- **Pomelo.EntityFrameworkCore.MySql** - Provedor MySQL
- **Swashbuckle.AspNetCore** - Documentação Swagger/OpenAPI
- **MySQL Workbench** - Gerenciamento do banco de dados
- **Open-Meteo API** - Dados meteorológicos externos

## 📦 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/)
- [MySQL Workbench](https://dev.mysql.com/downloads/workbench/) (opcional)
- [Git](https://git-scm.com/)

## ⚙️ Configuração

### 1. Clone o repositório
```bash
git clone <url-do-repositorio>
cd WeatherMap
```

### 2. Configure o banco de dados
```bash
# Copie o template de configuração
cp appsettings.json.example appsettings.json
```

### 3. Edite o `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Port=3306;Database=SEU_BANCO;user=SEU_USUARIO;password=SUA_SENHA;"
  }
}
```

### 4. Execute a aplicação
```bash
dotnet restore
dotnet run
```

### 5. Acesse a documentação
Abra no navegador: `http://localhost:5082/swagger`

## 🌐 Endpoints Principais

### 📍 Consulta por Cidade
```http
POST /api/Weather/fetch-and-save/city/{cityName}
```
**Exemplo:**
```http
POST /api/Weather/fetch-and-save/city/Maracanau?forecastDays=7
```

### 📍 Consulta por Coordenadas
```http
POST /api/Weather/fetch-and-save/coordinates?latitude=-3.875&longitude=-38.625
```

### 📊 Histórico de Dados
```http
GET /api/Weather/history?limit=50
GET /api/Weather/saved/1
GET /api/Weather/saved/location/Maracanau
```

### 📈 Análises e Estatísticas
```http
GET /api/Weather/analytics/trends/Maracanau?days=30
GET /api/Weather/analytics/locations-stats-raw?days=30
POST /api/Weather/analytics/location-comparison-raw
```

### 🗑️ Gerenciamento de Dados
```http
PUT /api/Weather/saved/1/location
DELETE /api/Weather/saved/1
DELETE /api/Weather/cleanup?daysOld=90
```

## 🎯 Exemplos de Uso

### Consulta e armazenamento de dados:
```bash
curl -X POST "http://localhost:5082/api/Weather/fetch-and-save/city/São Paulo?forecastDays=7"
```

### Resposta de exemplo:
```json
{
  "success": true,
  "message": "Dados climáticos obtidos e salvos com sucesso",
  "savedId": 7,
  "location": "Maracanaú, Brasil",
  "retrievedAt": "2025-09-14T21:24:50.981406Z",
  "data": {
    "latitude": -3.875,
    "longitude": -38.625,
    "current": {
      "temperature": 25.9,
      "feelsLike": 28.2,
      "humidity": 70,
      "weatherDescription": "Céu limpo"
    },
    "dailyForecast": [...]
  }
}
```

## 🔗 API Externa

### Open-Meteo API
- **Documentação**: https://open-meteo.com/
- **Endpoint Principal**: https://api.open-meteo.com/v1/forecast
- **Geocoding**: https://geocoding-api.open-meteo.com/v1/search

### Exemplo de integração:
```csharp
var weatherData = await _weatherService.GetWeatherByCityAsync(
    "Maracanau", 
    null, 
    7
);
```

## 🗃️ Estrutura do Banco

### Tabela Principal: `WeatherHistories`
```sql
CREATE TABLE WeatherHistories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    LocationName VARCHAR(200),
    Latitude DECIMAL(9,6),
    Longitude DECIMAL(9,6),
    CurrentTemperature DECIMAL(5,2),
    CurrentHumidity INT,
    CurrentWeatherDescription VARCHAR(500),
    CreatedAt DATETIME,
    RetrievedAt DATETIME
);
```

### Tabela: `DailyForecasts`
```sql
CREATE TABLE DailyForecasts (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    WeatherHistoryId INT,
    ForecastDate DATE,
    TemperatureMax DECIMAL(5,2),
    TemperatureMin DECIMAL(5,2),
    PrecipitationSum DECIMAL(5,2),
    FOREIGN KEY (WeatherHistoryId) REFERENCES WeatherHistories(Id) ON DELETE CASCADE
);
```

## 🔒 Segurança

### Arquivos Protegidos
O arquivo `.gitignore` está configurado para proteger dados sensíveis:

```gitignore
# Credenciais de banco
appsettings.json

# Variáveis de ambiente
.env
*.env.local

# Arquivos de usuário
*.user
*.userosscache
```

### Template Seguro
Use o `appsettings.json.example` como template:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=weathermapdb;user=root;password=SUA_SENHA_AQUI;"
  }
}
```

## 🚀 Deploy e Produção

### Variáveis de Ambiente
Para produção, configure as variáveis de ambiente:

```bash
export DB_SERVER=servidor-producao
export DB_NAME=weathermapdb_prod
export DB_USER=usuario_producao
export DB_PASSWORD=senha_segura
```

### Health Check
Verifique o status da API:
```http
GET /health
```

## 📞 Suporte

- **Desenvolvedor**: Thiago de Sena
- **Email**: thiagosena316@gmail.com
- **linkedin**: [Clique aqui](https://www.linkedin.com/in/thiago-de-sena-developer/)
- **Documentação**: `/swagger` (localmente)

## 📄 Licença

Este projeto é para fins educacionais e demonstrativos.

---
