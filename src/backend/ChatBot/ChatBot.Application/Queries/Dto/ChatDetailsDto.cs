namespace ChatBot.Application.Queries.Dto;

public record ChatDetailsDto
{
    public required Guid Id { get; init; }
    public required string? Title { get; init; }
    public required IEnumerable<ChatMessageDto> Messages { get; init; }
    
    public record ChatMessageDto
    {
        public required Guid Id { get; init; }
        public required string? Content { get; init; }
        public required string MessageType { get; init; }
        public string? Reaction { get; init; }
        public DateTime CreatedAt { get; set; }
    }
}