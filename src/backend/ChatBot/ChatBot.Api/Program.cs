using ChatBot.Api.Extensions.DI;
using ChatBot.Api.Infrastructure;
using ChatBot.Application.Queries;
using ChatBot.Application.Services.ChatCompletion;
using ChatBot.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProvideCurrentUserId, CurrentUserIdProvider>();
builder.Services.AddDatabase();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<GetAllChatsQuery>()
);
builder.Services.AddTransient<IChatCompletion, FakeChatCompletion>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ConfiguredCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("ConfiguredCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.PrepareDatabase();

app.Run();