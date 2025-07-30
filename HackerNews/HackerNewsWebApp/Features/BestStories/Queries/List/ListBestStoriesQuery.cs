using HackerNewsWebApp.Features.BestStories.Dtos;
using HackerNewsWebApp.Features.BestStories.Repositories;
using MediatR;

namespace HackerNewsWebApp.Features.BestStories.Queries.List;

internal sealed record ListBestStoriesQuery(int TopCount) : IRequest<IReadOnlyCollection<BestStoryDto>>;

internal sealed class ListBestStoriesQueryHandler(IBestStoryRepository bestStoryRepository) : IRequestHandler<ListBestStoriesQuery, IReadOnlyCollection<BestStoryDto>>
{
    public async Task<IReadOnlyCollection<BestStoryDto>> Handle(ListBestStoriesQuery request, CancellationToken cancellationToken)
    {
        return await bestStoryRepository.GetBestStories(request.TopCount, cancellationToken);
    }
}