namespace ChatBot.Application.UseCases.GenerateChatCompletion.Exceptions;

public class ChatNotFound(Guid guid) : Exception($"Chat with id '{guid}' was not found.");