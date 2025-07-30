namespace HackerNewsWebApp.Features.BestStories.Dtos;

/// <summary>
/// Story mapped from "Hacker News Item".
/// </summary>
/// <param name="Title">The title of the story, poll or job. HTML.</param>
/// <param name="Uri">The URL of the story.</param>
/// <param name="PostedBy">The username of the item's author.</param>
/// <param name="Time">Creation date of the item.</param>
/// <param name="Score">The story's score, or the votes for a pollopt.</param>
/// <param name="CommentCount">In the case of stories or polls, the total comment count.</param>
public sealed record BestStoryDto(
    string? Title,
    string? Uri,
    string? PostedBy,
    DateTimeOffset? Time,
    int? Score,
    int? CommentCount);