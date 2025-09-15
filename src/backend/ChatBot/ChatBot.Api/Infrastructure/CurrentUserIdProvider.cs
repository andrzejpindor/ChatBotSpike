using ChatBot.Infrastructure;

namespace ChatBot.Api.Infrastructure;

internal class CurrentUserIdProvider : IProvideCurrentUserId
{
    private static readonly Guid FakeUserId = new("d290f1ee-6c54-4b01-90e6-d701748f0851");
    public Guid GetCurrentUserId() => FakeUserId;
}