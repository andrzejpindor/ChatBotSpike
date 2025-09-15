namespace ChatBot.Application.Services.ChatCompletion;

public interface IChatCompletion
{
    IAsyncEnumerable<string> GetCompletion(
        IEnumerable<string?> messageHistory,
        CancellationToken cancellationToken);
}
