using Microsoft.Azure.Cosmos;

namespace CarManager.Utils.Extensions;

public static class FeedIteratorExtensions
{
    public static async Task<T> GetSingleValueAsync<T>(this FeedIterator<T> iterator)
    {
        while (iterator.HasMoreResults)
        {
            var readResponse = await iterator.ReadNextAsync();
            return readResponse.Single();
        }

        throw new InvalidOperationException("Data in iterator is null");
    }

    public static async Task<T?> GetSingleValueOrDefaultAsync<T>(this FeedIterator<T> iterator) where T : class
    {
        while (iterator.HasMoreResults)
        {
            var readResponse = await iterator.ReadNextAsync();
            return readResponse.SingleOrDefault();
        }

        return null;
    }

    public static async Task<IEnumerable<T>> GetRangeValueAsync<T>(this FeedIterator<T> iterator)
    {
        var resultCollection = new List<T>();
        while (iterator.HasMoreResults)
        {
            var readResponse = await iterator.ReadNextAsync();
            resultCollection.AddRange(readResponse);
        }

        return resultCollection;
    }
}