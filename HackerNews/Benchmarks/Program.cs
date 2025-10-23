using BenchmarkDotNet.Running;

namespace HackerNewsWebApp.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BestStoryRepositoryBenchmark>();
    }
}
