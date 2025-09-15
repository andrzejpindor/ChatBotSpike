namespace ChatBot.Infrastructure;

public interface IProvideCurrentUserId
{
    Guid GetCurrentUserId();
}