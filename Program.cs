using Microsoft.EntityFrameworkCore;
using PaystackIntegrationPracticeApi.ActionFilter;
using PaystackIntegrationPracticeApi.Data;
using PaystackIntegrationPracticeApi.Interfaces;
using PaystackIntegrationPracticeApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

builder.Services.AddSqlite<ApplicationDbContext>("Data Source=Application.db");

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("paystack", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.paystack.co/");
    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "sk_test_3442fefa9693af3d9955a0edcb33f034bd1b9cf0");
});

builder.Services.AddScoped<IPaystackIntegrationService, PaystackIntegrationService>();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddScoped<PaystackWebhookIPFilter>(container =>
{
    var loggerFactory = container.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<PaystackWebhookIPFilter>();

    return new PaystackWebhookIPFilter(
        builder.Configuration["PaystackWebhookIPSafeList"], logger);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
