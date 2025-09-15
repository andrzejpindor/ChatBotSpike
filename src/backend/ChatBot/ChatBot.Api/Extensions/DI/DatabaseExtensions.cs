using ChatBot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChatBot.Api.Extensions.DI;

internal static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ChatBotDbContext>(options =>
        {
            options.UseSqlServer("Name=ConnectionStrings:ChatBotDb");
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }
    
    public static void PrepareDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChatBotDbContext>();
        dbContext.Database.Migrate();
    }
}

public class ChatBotDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ChatBotDbContext>
{
    public ChatBotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChatBotDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ChatBotDb");
        optionsBuilder.UseSqlServer(connectionString);

        var fakeUserProvider = new FakeCurrentUserIdProvider();

        return new ChatBotDbContext(optionsBuilder.Options, fakeUserProvider);
    }

    private class FakeCurrentUserIdProvider : IProvideCurrentUserId
    {
        public Guid GetCurrentUserId()
        {
            return Guid.Empty;
        }
    }
}