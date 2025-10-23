using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace HackerNewsWebApp.Benchmarks
{
    [MemoryDiagnoser]
    public class BestStoryRepositoryBenchmark
    {
        [Params(1000, 10000)]
        public int ItemsCount { get; set; }

        [Params(100)]
        public int TopCount { get; set; }

        private HackerNewsItemDto[] items = Array.Empty<HackerNewsItemDto>();
        private static readonly Func<HackerNewsItemDto, BestStoryDto> Factory = CreateDto;

        [GlobalSetup]
        public void Setup()
        {
            var rnd = new Random(42);
            items = Enumerable.Range(1, ItemsCount)
                              .Select(i => new HackerNewsItemDto { Id = i, Score = rnd.Next(0, 10000) })
                              .ToArray();
        }

        [Benchmark(Baseline = true)]
        public BestStoryDto[] LinqPipeline()
        {
            return items.OrderByDescending(x => x.Score)
                        .ThenByDescending(x => x.Id)
                        .Take(TopCount)
                        .Select(Factory)
                        .ToArray();
        }

        [Benchmark]
        public BestStoryDto[] ArraySortPipeline()
        {
            var buffer = (HackerNewsItemDto[])items.Clone();
            Array.Sort(buffer, ItemComparer.Instance);

            var take = Math.Min(TopCount, buffer.Length);
            var result = new BestStoryDto[take];
            for (var i = 0; i < take; i++)
                result[i] = Factory(buffer[i]);

            return result;
        }

        private static BestStoryDto CreateDto(HackerNewsItemDto src)
        {
            return new BestStoryDto { Id = src.Id, Score = src.Score };
        }

        public sealed class HackerNewsItemDto
        {
            public int Id { get; set; }
            public int Score { get; set; }
        }

        public sealed class BestStoryDto
        {
            public int Id { get; set; }
            public int Score { get; set; }
        }

        private sealed class ItemComparer : System.Collections.Generic.IComparer<HackerNewsItemDto>
        {
            public static readonly ItemComparer Instance = new ItemComparer();
            private ItemComparer() { }

            public int Compare(HackerNewsItemDto a, HackerNewsItemDto b)
            {
                if (ReferenceEquals(a, b)) return 0;
                if (a is null) return -1;
                if (b is null) return 1;

                var c = b.Score.CompareTo(a.Score);
                return c != 0 ? c : b.Id.CompareTo(a.Id);
            }
        }
    }
}
