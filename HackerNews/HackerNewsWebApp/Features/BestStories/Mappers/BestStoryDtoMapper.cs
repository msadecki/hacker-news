using HackerNewsWebApp.Features.BestStories.Dtos;

namespace HackerNewsWebApp.Features.BestStories.Mappers;

internal interface IBestStoryDtoMapper
{
    BestStoryDto CreateBestStoryDto(HackerNewsItemDto? hackerNewsItemDto);
}

internal sealed class BestStoryDtoMapper : IBestStoryDtoMapper
{
    public BestStoryDto CreateBestStoryDto(HackerNewsItemDto? hackerNewsItemDto)
    {
        return new BestStoryDto(
            Title: hackerNewsItemDto!.Title,
            Uri: hackerNewsItemDto.Url,
            PostedBy: hackerNewsItemDto.By,
            Time: hackerNewsItemDto.Time != null ? DateTimeOffset.FromUnixTimeSeconds(hackerNewsItemDto.Time.Value) : null,
            Score: hackerNewsItemDto.Score,
            CommentCount: hackerNewsItemDto.Descendants);
    }
}
