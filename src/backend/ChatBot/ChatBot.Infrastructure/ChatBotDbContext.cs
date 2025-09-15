using ChatBot.Domain.Entities.Chats;
using ChatBot.Domain.Entities.Reactions;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Infrastructure;

public class ChatBotDbContext : DbContext
{
    private readonly Guid _currentUserId;
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<MessageReaction> Reactions { get; set; }

    public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options, IProvideCurrentUserId currentUserId) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        _currentUserId = currentUserId.GetCurrentUserId();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.ToTable("Chats");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            entity.Property(e => e.Title).IsRequired(false);
            entity.HasIndex(e => e.UserId, "IX_Chats_UserId");
            entity.HasMany(e => e.Messages).WithOne().HasForeignKey(x => x.ChatId);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasConversion(c => c,
                    c => DateTime.SpecifyKind(c, DateTimeKind.Utc));
            entity.HasQueryFilter(e => e.UserId == _currentUserId);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("ChatMessages");
            entity.HasKey(e => e.Id);
            entity.Property(x => x.Content).IsRequired(false);
            entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasConversion(c => c,
                    c => DateTime.SpecifyKind(c, DateTimeKind.Utc));
            entity.HasIndex(e => e.ChatId);
            entity.HasQueryFilter(e => e.UserId == _currentUserId);
        });

        modelBuilder.Entity<MessageReaction>(entity =>
        {
            entity.ToTable("MessageReactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            entity.HasIndex(e => new { e.MessageId, e.UserId }, "IX_MessageReactions_MessageId_UserId").IsUnique();

            entity.HasQueryFilter(e => e.UserId == _currentUserId);
        });
    }
}