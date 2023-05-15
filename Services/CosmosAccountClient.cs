using Microsoft.Azure.Cosmos;
namespace CarManager.Services;

public class CosmosAccountClient
{
    private readonly CosmosClient cosmosClient;

    public CosmosAccountClient(CosmosClient cosmosClient)
    {
        this.cosmosClient = cosmosClient;
    }

    public CosmosClient GetCosmosClient()
    {
        return cosmosClient;
    }

    public async Task<Database> GetOrCreateDatabaseAsync(string databaseId)
    {
        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);

        return databaseResponse.Database;
    }

    public async Task<Container> GetOrCreateContainerAsync(string databaseId, string containerId, string partitionKeyPath)
    {
        var database = await GetOrCreateDatabaseAsync(databaseId);
        return await GetOrCreateContainerAsync(database, containerId, partitionKeyPath);
    }

    public async Task<Container> GetOrCreateContainerAsync(Database database, string containerId, string partitionKeyPath)
    {
        var containerResponse = await database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath);

        return containerResponse.Container;
    }

}