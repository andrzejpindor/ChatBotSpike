using ChatBot.Domain.Entities.Reactions;
using MediatR;

namespace ChatBot.Application.UseCases.Reactions;

public class SetMessageReaction : IRequest
{
    public required Guid MessageId { get; init; }
    public required ReactionType ReactionType { get; init; }
    public required Guid UserId { get; init; }
}