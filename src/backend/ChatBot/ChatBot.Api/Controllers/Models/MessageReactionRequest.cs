using System.ComponentModel.DataAnnotations;

namespace ChatBot.Api.Controllers.Models;

public class MessageReactionRequest
{
    [Required]
    [RegularExpression("like|dislike")]
    public string ReactionType { get; set; } = null!;
}