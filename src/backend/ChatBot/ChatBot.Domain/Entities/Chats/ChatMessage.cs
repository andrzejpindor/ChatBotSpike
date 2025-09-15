using ChatBot.Domain.Entities.Chats.Exceptions;

namespace ChatBot.Domain.Entities.Chats;

public class ChatMessage
{
    public Guid Id { get; protected set; }
    public Guid ChatId { get; protected set; }
    public Guid UserId { get; protected set; }
    public string? Content { get; protected set; }
    public MessageType MessageType { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    protected ChatMessage()
    {
    }

    public void ConcatenateContent(string additionalContent)
    {
        if (MessageType == MessageType.User) throw new CannotAlterUserMessage();
        Content += additionalContent;
    }

    internal ChatMessage(Guid chatId, Guid userId, string content, MessageType messageType)
    {
        ChatId = chatId;
        UserId = userId;
        Content = content;
        MessageType = messageType;
    }
}