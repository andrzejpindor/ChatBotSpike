using MediatR;

namespace ChatBot.Application.UseCases.GenerateChatCompletion;

public class ChatCompletionChunk
{
    public required Guid ChatId { get; init; }
    public required string Content { get; init; }
    public Guid MessageId { get; set; }
    public Guid UserMessageId { get; set; }
}

public class GenerateChatCompletion : IStreamRequest<ChatCompletionChunk>
{
    public Guid? ChatId { get; init; }
    public Guid UserId { get; init; }
    public required string UserMessage { get; init; }
}