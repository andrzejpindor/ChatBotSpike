using ChatBot.Domain.Entities.Reactions;
using ChatBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Application.UseCases.Reactions;

public class SetMessageReactionHandler(ChatBotDbContext dbContext) : IRequestHandler<SetMessageReaction>
{
    public async Task Handle(SetMessageReaction request, CancellationToken cancellationToken)
    {
        var reaction = await dbContext
            .Reactions
            .AsTracking()
            .FirstOrDefaultAsync(r => r.MessageId == request.MessageId, cancellationToken);

        if (reaction == null)
        {
            reaction = new MessageReaction(request.MessageId, request.UserId, request.ReactionType);
            dbContext.Reactions.Add(reaction);
        }
        else
        {
            reaction.ReactionType = request.ReactionType;
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}