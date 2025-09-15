using ChatBot.Application.Queries.Dto;
using ChatBot.Domain.Entities.Chats;
using ChatBot.Domain.Entities.Reactions;
using ChatBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Application.Queries;

public class GetChatByIdQueryHandler(ChatBotDbContext dbContext) : IRequestHandler<GetChatByIdQuery, ChatDetailsDto?>
{
    public async Task<ChatDetailsDto?> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Chats
            .AsNoTracking()
            .Where(c => c.Id == request.ChatId)
            .Select(c => new ChatDetailsDto
            {
                Id = c.Id,
                Title = c.Title,
                Messages = c.Messages
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.Content != null)
                    .Select(m => new
                    {
                        Message = m,
                        Reaction = dbContext.Reactions
                            .Where(r => r.MessageId == m.Id)
                            .Select(r => Map(r.ReactionType))
                            .FirstOrDefault()
                    })
                    .Select(mr => new ChatDetailsDto.ChatMessageDto
                    {
                        Id = mr.Message.Id,
                        Content = mr.Message.Content,
                        MessageType = MapMessageType(mr.Message.MessageType),
                        Reaction = mr.Reaction,
                        CreatedAt = mr.Message.CreatedAt
                    })
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static string? Map(ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "like",
            ReactionType.Dislike => "dislike",
            _ => null
        };
    }

    private static string MapMessageType(MessageType messageType) => messageType switch
    {
        MessageType.User => "user",
        MessageType.System => "system",
        _ => "Unknown"
    };
}