using System.Text.Json;
using ChatBot.Api.Controllers.Models;
using ChatBot.Application.Queries;
using ChatBot.Application.Queries.Dto;
using ChatBot.Application.UseCases.GenerateChatCompletion;
using ChatBot.Application.UseCases.GenerateChatCompletion.Exceptions;
using ChatBot.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.Controllers;

[ApiController]
[Route("chats")]
public class ChatsController(IMediator mediator, IProvideCurrentUserId userIdProvider) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatListItemDto>>> GetAllChats(CancellationToken cancellationToken)
    {
        var query = new GetAllChatsQuery();
        var chats = await mediator.Send(query, cancellationToken);
        return Ok(chats);
    }

    [HttpGet("{chatId:guid}")]
    public async Task<ActionResult<IEnumerable<ChatListItemDto>>> GetAllChats(Guid chatId,
        CancellationToken cancellationToken)
    {
        var query = new GetChatByIdQuery { ChatId = chatId };
        var chat = await mediator.Send(query, cancellationToken);
        return chat == null ? NotFound() : Ok(chat);
    }

    [HttpPost("completions")]
    [HttpPost("completions/{id:guid}")]
    public async Task CreateChat(Guid? id, ChatCompletionRequest request,
        CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        
        var command = new GenerateChatCompletion
        {
            ChatId = id,
            UserMessage = request.Message,
            UserId = userIdProvider.GetCurrentUserId()
        };

        try
        {
            var stream = mediator.CreateStream(command, cancellationToken);
            var metaDataSent = false;
            
            await foreach (var item in stream)
            {
                if (!metaDataSent)
                {
                    var metadata = JsonSerializer.Serialize(new
                    {
                        chatId = item.ChatId,
                        userMessageId = item.UserMessageId,
                        completionMessageId = item.MessageId
                    });
                    
                    await Response.WriteAsync(
                        $"event: metadata\ndata: {metadata}\n\n",
                        cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                    metaDataSent = true;
                }

                var delta = new
                {
                    content = item.Content
                };

                var json = JsonSerializer.Serialize(delta);
                var sseData = $"event: delta\ndata: {json}\n\n";

                await Response.WriteAsync(sseData, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (ChatNotFound)
        {
            Response.StatusCode = 404;
            await Response.WriteAsync($"event: error\ndata: {JsonSerializer.Serialize(new { error = "Chat not found" })}\n\n",
                cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
            return;
        }

        var content = JsonSerializer.Serialize(new { status = "done" });
        await Response.WriteAsync($"event: finish\ndata: {content}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
}