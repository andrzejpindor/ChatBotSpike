using ChatBot.Application.Queries.Dto;
using MediatR;

namespace ChatBot.Application.Queries;

public class GetChatByIdQuery : IRequest<ChatDetailsDto?>
{
    public required Guid ChatId { get; init; }
}