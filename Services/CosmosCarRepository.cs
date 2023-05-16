using System.Net;
using CarManager.Models;
using CarManager.Services.Abstractions;
using CarManager.Utils.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;

namespace CarManager.Services;

public class CosmosCarRepository : IRepository<Car, string>
{
    private readonly string containerId;
    private readonly CosmosAccountClient cosmosClient;
    private readonly string databaseId;
    private readonly string partitionKeyPath;

    public CosmosCarRepository(CosmosAccountClient cosmosClient, IOptions<Configuration.Azure> azureOptions)
    {
        this.cosmosClient = cosmosClient;

        databaseId = azureOptions.Value.Cosmos?.CarsDatabase?.Id ??
                     throw new InvalidOperationException("'Azure.Cosmos.CarsDatabase.Id' configuration value is not provided");

        containerId = azureOptions.Value.Cosmos.CarsDatabase.CarsContainer?.Id ??
                      throw new InvalidOperationException(
                          "'Azure.Cosmos.CarsDatabase.CarsContainer.Id' configuration value is not provided");

        partitionKeyPath = azureOptions.Value.Cosmos.CarsDatabase.CarsContainer.PartitionKeyPath ??
                           throw new InvalidOperationException(
                               "'Azure.Cosmos.CarsDatabase.CarsContainer.PartitionKeyPath' configuration value is not provided");
    }

    public async Task<IEnumerable<Car>> GetItemsAsync()
    {
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        var carsIterator = container.GetItemQueryIterator<Car>();

        return await carsIterator.GetRangeValueAsync();
    }

    public async Task<Car?> GetItemAsync(string id)
    {
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        var iterator = container.GetItemLinqQueryable<Car>()
            .Where(car => car.Id.Equals(id))
            .ToFeedIterator();

        return await iterator.GetSingleValueOrDefaultAsync();
    }

    public async Task AddItemsAsync(params Car[] items)
    {
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);

        foreach (var car in items)
            await container.CreateItemAsync(car, new PartitionKey(car.Id));
    }

    public async Task EditItemAsync(string id, Car editedItem)
    {
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        await container.ReplaceItemAsync(editedItem, editedItem.Id, new PartitionKey(editedItem.Id));
    }

    public async Task DeleteItemAsync(string id)
    {
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        var response = await container.DeleteItemAsync<Car>(id, new PartitionKey(id));

        if (response.StatusCode is not HttpStatusCode.NoContent and HttpStatusCode.OK)
        {
            throw new HttpRequestException($"Failed to delete item with ID: {id}");
        }
    }
}