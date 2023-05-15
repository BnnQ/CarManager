using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CarManager.Utils.Extensions;

public static class ContainerExtensions
{
    public static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(this Container container, Predicate<T>? querySelector = null)
    {
        var iterator = container.GetItemLinqQueryable<T>()
            .Where(entity => querySelector == null || querySelector.Invoke(entity))
            .ToFeedIterator();

        var resultCollection = new List<T>();
        while (iterator.HasMoreResults)
        {
            var readResponse = await iterator.ReadNextAsync();
            resultCollection.AddRange(readResponse);
        }

        return resultCollection;
    }
}