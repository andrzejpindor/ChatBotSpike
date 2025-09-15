using System.Runtime.CompilerServices;
using ChatBot.Application.Services.ChatCompletion;
using ChatBot.Application.UseCases.GenerateChatCompletion.Exceptions;
using ChatBot.Domain.Entities.Chats;
using ChatBot.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Application.UseCases.GenerateChatCompletion;

public class GenerateChatCompletionHandler(
    ChatBotDbContext dbContext,
    IChatCompletion chatCompletion
) : IStreamRequestHandler<GenerateChatCompletion, ChatCompletionChunk>
{
    public async IAsyncEnumerable<ChatCompletionChunk> Handle(GenerateChatCompletion request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chat = await GetOrCreateChat(request, cancellationToken);

        var userMessage = chat.CreateForUser(request.UserMessage);
        await dbContext.ChatMessages.AddAsync(userMessage, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var chatId = chat.Id;

        var completionContext = chat.Messages.Select(x => x.Content).ToArray();

        var systemMessage = chat.CreateForSystem();
        await dbContext.ChatMessages.AddAsync(systemMessage, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await foreach (var chunk in chatCompletion.GetCompletion(completionContext, cancellationToken))
        {
            systemMessage.ConcatenateContent(chunk);
            await dbContext.SaveChangesAsync(cancellationToken);
            yield return new ChatCompletionChunk {
                ChatId = chatId, 
                MessageId = systemMessage.Id,
                UserMessageId = userMessage.Id,
                Content = chunk };
        }
    }

    private async Task<Chat> GetOrCreateChat(GenerateChatCompletion request, CancellationToken cancellationToken)
    {
        Chat? chat;
        if (request.ChatId.HasValue)
        {
            chat = await dbContext.Chats
                .Include(x => x.Messages)
                .FirstOrDefaultAsync(x => x.Id == request.ChatId.Value,
                    cancellationToken: cancellationToken);

            if (chat == null)
            {
                throw new ChatNotFound(request.ChatId.Value);
            }
        }
        else
        {
            chat = new Chat(request.UserId);
            await dbContext.Chats.AddAsync(chat, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return chat;
    }
}