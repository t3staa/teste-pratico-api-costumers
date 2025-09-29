using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using teste_pratico.Data;
using teste_pratico.Repositories.Interfaces;
using teste_pratico.Repositories.Implementations;
using teste_pratico.Services.Interfaces;
using teste_pratico.Services.Implementations;
using teste_pratico.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Entity Framework with In-Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("CustomerDatabase"));

// Configure HttpClient for ViaCEP API
builder.Services.AddHttpClient<IViaCepService, ViaCepService>(client =>
{
    client.BaseAddress = new Uri("https://viacep.com.br/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("User-Agent", "teste-pratico-api/1.0");
});

// Register Repository dependencies
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register Service dependencies
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Configure API behavior and validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new
            {
                error = "Dados inválidos",
                message = "Um ou mais campos contêm valores inválidos.",
                type = "validation_error",
                errors = errors
            };

            return new BadRequestObjectResult(response);
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Customer Management API",
        Version = "v1",
        Description = "API para gerenciamento de clientes com integração ViaCEP"
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// IMPORTANTE: Swagger deve vir ANTES do ExceptionHandler para não ser interceptado
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Management API V1");
        c.RoutePrefix = string.Empty; // Swagger disponível na raiz
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
    });
}

// Global exception handling middleware (DEPOIS do Swagger)
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

// CORS (if needed in the future)
// app.UseCors();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created (for InMemory database)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
