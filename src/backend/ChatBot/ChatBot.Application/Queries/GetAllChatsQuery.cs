using ChatBot.Application.Queries.Dto;
using MediatR;

namespace ChatBot.Application.Queries;

public class GetAllChatsQuery : IRequest<IEnumerable<ChatListItemDto>>;