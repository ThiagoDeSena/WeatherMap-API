using Microsoft.EntityFrameworkCore;
using WeatherMap.Configurations;
using WeatherMap.Data;
using WeatherMap.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar Entity Framework com MySQL
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         new MySqlServerVersion(new Version(8, 0, 25))
//     ));

// Configurar ApiSettings
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// Registrar HttpClient para o WeatherService
builder.Services.AddHttpClient<IWeatherService, OpenMeteoService>();

// Registrar outros serviços
builder.Services.AddScoped<IWeatherService, OpenMeteoService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Weather Map API",
        Version = "v1",
        Description = "API para consultar dados climáticos e análises meteorológicas",
        Contact = new()
        {
            Name = "Seu Nome",
            Email = "seu.email@exemplo.com"
        }
    });

    // Incluir comentários XML na documentação
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configurar CORS se necessário
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Criar o banco de dados automaticamente se não existir
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     context.Database.EnsureCreated(); // Cria o banco se não existir
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Map API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz da aplicação
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Middleware para health check
app.MapGet("/health", async (IWeatherService weatherService) =>
{
    var isHealthy = await weatherService.IsApiHealthyAsync();
    return isHealthy
        ? Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow })
        : Results.Problem("API não está respondendo");
});

// Endpoint de teste simples
app.MapGet("/", () => new
{
    message = "Weather Map API está funcionando!",
    version = "1.0",
    timestamp = DateTime.UtcNow,
    documentation = "/swagger"
});

app.Run();