using ChatBot.Domain.Entities.Chats.Exceptions;

namespace ChatBot.Domain.Entities.Chats;

public class Chat
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    // protected virtual ICollection<ChatMessage> _messages { get; set; } = new List<ChatMessage>();
    // public virtual IReadOnlyCollection<ChatMessage> Messages => _messages; 
    private readonly List<ChatMessage> _messages = new();
    public virtual IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }

    protected Chat()
    {
    }

    public Chat(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public ChatMessage CreateForUser(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new EmptyUserMessageException();
        }
        
        return new ChatMessage(Id, UserId, content, MessageType.User);
    }

    public ChatMessage CreateForSystem()
    {
        return new ChatMessage(Id, UserId, null, MessageType.System);
    }
}