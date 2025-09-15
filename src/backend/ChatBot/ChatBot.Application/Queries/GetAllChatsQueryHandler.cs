using ChatBot.Application.Queries.Dto;
using ChatBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Application.Queries;

public class GetAllChatsQueryHandler(ChatBotDbContext dbContext)
    : IRequestHandler<GetAllChatsQuery, IEnumerable<ChatListItemDto>>
{
    public async Task<IEnumerable<ChatListItemDto>> Handle(GetAllChatsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Chats
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ChatListItemDto
            {
                Id = x.Id,
                Title = x.Title
            })
            .ToListAsync(cancellationToken);
    }
}