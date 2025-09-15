using ChatBot.Api.Controllers.Models;
using ChatBot.Application.UseCases.Reactions;
using ChatBot.Domain.Entities.Reactions;
using ChatBot.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.Controllers;

[Route("messages/{messageId:guid:required}/reaction")]
[ApiController]
public class ReactionsController(IMediator mediator, IProvideCurrentUserId userIdProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SetReaction(Guid messageId, MessageReactionRequest request)
    {
        var command = new SetMessageReaction
        {
            MessageId = messageId, 
            UserId = userIdProvider.GetCurrentUserId(),
            ReactionType = MapReaction(request.ReactionType)
        };
        await mediator.Send(command);
        return Accepted();
    }

    private static ReactionType MapReaction(string requestReactionType)
    {
        return requestReactionType.ToLowerInvariant() switch
        {
            "like" => ReactionType.Like,
            "dislike" => ReactionType.Dislike,
            _ => throw new ArgumentOutOfRangeException(nameof(requestReactionType),
                $"Unsupported reaction type: {requestReactionType}")
        };
    }
}