using System.Runtime.CompilerServices;
using System.Text.Json;
using HackerNewsWebApp.Features.BestStories.Dtos;
using HackerNewsWebApp.Features.BestStories.Mappers;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsWebApp.Features.BestStories.Repositories;

public interface IBestStoryRepository
{
    Task<IReadOnlyCollection<BestStoryDto>> GetBestStories(int topCount, CancellationToken cancellationToken);
}

internal sealed class BestStoryRepository(
    IMemoryCache memoryCache,
    IBestStoryDtoMapper bestStoryDtoMapper) : IBestStoryRepository
{
    private static class WebJsonHelper
    {
        private static readonly JsonSerializerOptions WebJsonSerializerOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, WebJsonSerializerOptions);
        }
    }

    public async Task<IReadOnlyCollection<BestStoryDto>> GetBestStories(int topCount, CancellationToken cancellationToken)
    {
        var key = $"{nameof(BestStoryRepository)}.{nameof(GetBestStoriesHackerNewsItems)}";
        var hackerNewsItemDtos = await memoryCache.GetOrCreateAsync(
           key,
           cacheEntry =>
           {
               cacheEntry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(60);
               return GetBestStoriesHackerNewsItems(cancellationToken);
           });

        return new ReadOnlyCollectionBuilder<BestStoryDto>(
            hackerNewsItemDtos!.Where(hackerNewsItemDto => hackerNewsItemDto != null)
                               .OrderByDescending(hackerNewsItemDto => hackerNewsItemDto!.Score)
                               .ThenByDescending(hackerNewsItemDto => hackerNewsItemDto!.Id)
                               .Take(topCount)
                               .Select(bestStoryDtoMapper.CreateBestStoryDto)).ToReadOnlyCollection();
    }

    private static async Task<IReadOnlyCollection<HackerNewsItemDto?>> GetBestStoriesHackerNewsItems(CancellationToken cancellationToken)
    {
        using (var httpClient = new HttpClient())
        {
            var bestStoriesIdsApiEndpointUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";

            try
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var bestStoriesIdsResponse = await httpClient.GetAsync(bestStoriesIdsApiEndpointUrl, cancellationToken);
                if (bestStoriesIdsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var bestStoriesIdsResponseJsonString = await bestStoriesIdsResponse.Content.ReadAsStringAsync(cancellationToken);
                    var bestStoriesIds = WebJsonHelper.Deserialize<int[]>(bestStoriesIdsResponseJsonString) ?? Array.Empty<int>();

                    var hackerNewsItemDtos = await Task.WhenAll(bestStoriesIds.Select(id => GetHackerNewsItem(httpClient, id, cancellationToken)));

                    return hackerNewsItemDtos;
                }
                else
                {
                    throw new ApplicationException($"{nameof(GetBestStoriesHackerNewsItems)} - IsSuccessStatusCode false");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    private static async Task<HackerNewsItemDto?> GetHackerNewsItem(HttpClient httpClient, int id, CancellationToken cancellationToken)
    {
        var hackerNewsItemApiEndpointUrl = $"https://hacker-news.firebaseio.com/v0/item/{id}.json";

        var hackerNewsItemResponse = await httpClient.GetAsync(hackerNewsItemApiEndpointUrl, cancellationToken);
        if (hackerNewsItemResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            var hackerNewsItemResponseJsonString = await hackerNewsItemResponse.Content.ReadAsStringAsync(cancellationToken);
            var hackerNewsItemDto = WebJsonHelper.Deserialize<HackerNewsItemDto>(hackerNewsItemResponseJsonString);

            return hackerNewsItemDto;
        }
        else
        {
            throw new ApplicationException($"{nameof(GetHackerNewsItem)} - IsSuccessStatusCode false");
        }
    }
}