namespace ChatBot.Domain.Entities.Reactions;

public class MessageReaction
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }

    protected MessageReaction()
    {
        
    }

    public MessageReaction(Guid messageId, Guid userId, ReactionType reactionType)
    {
        MessageId = messageId;
        UserId = userId;
        ReactionType = reactionType;
    }
}