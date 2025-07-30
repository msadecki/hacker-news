namespace HackerNewsWebApp.Features.BestStories.Dtos;

public sealed record BestStoryDto(
    string? Title,
    string? Uri,
    string? PostedBy,
    DateTimeOffset? Time,
    int? Score,
    int? CommentCount);