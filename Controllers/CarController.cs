using System.Net;
using System.Text.Json;
using AutoMapper;
using CarManager.Models;
using CarManager.Models.Dto;
using CarManager.Services;
using CarManager.Services.Abstractions;
using CarManager.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace CarManager.Controllers;

public class CarController : Controller
{
    private readonly CosmosAccountClient cosmosClient;
    private readonly JsonSerializerOptions serializerOptions;
    private readonly IMapper mapper;
    private readonly ILogger<CarController> logger;

    private readonly string databaseId;
    private readonly string containerId;
    private readonly string partitionKeyPath;
    private readonly PartitionKey partitionKey;

    public CarController(ILoggerFactory loggerFactory,
        CosmosAccountClient cosmosClient,
        IOptions<Configuration.Azure> azureOptions,
        JsonSerializerOptions serializerOptions,
        IMapper mapper)
    {
        this.cosmosClient = cosmosClient;
        this.serializerOptions = serializerOptions;
        this.mapper = mapper;
        logger = loggerFactory.CreateLogger<CarController>();

        databaseId = azureOptions.Value.Cosmos?.CarsDatabase?.Id ??
                     throw new InvalidOperationException("'Azure.Cosmos.CarsDatabase.Id' configuration value is not provided");

        containerId = azureOptions.Value.Cosmos.CarsDatabase.CarsContainer?.Id ??
                      throw new InvalidOperationException(
                          "'Azure.Cosmos.CarsDatabase.CarsContainer.Id' configuration value is not provided");

        partitionKeyPath = "/" + (azureOptions.Value.Cosmos.CarsDatabase.CarsContainer.PartitionKeyValue ??
                                  throw new InvalidOperationException(
                                      "'Azure.Cosmos.CarsDatabase.CarsContainer.PartitionKeyValue' configuration value is not provided"));

        partitionKey = new PartitionKey(azureOptions.Value.Cosmos.CarsDatabase.CarsContainer.PartitionKeyValue);
    }

    [HttpGet(Name = "api")]
    public async Task<IActionResult> GetCars()
    {
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        var cars = await container.ExecuteQueryAsync<Car>();

        logger.LogInformation("[GET] GetCars: returning JSON cars from database");
        return Json(data: cars, serializerSettings: serializerOptions);
    }

    [HttpGet(Name = "api")]
    public async Task<IActionResult> GetCar(string id)
    {
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        var car = (await container.ExecuteQueryAsync<Car>(car => car.Id.Equals(id))).SingleOrDefault();

        if (car is null)
        {
            logger.LogWarning("[GET] GetCar: car with ID '{CarId}' not found, returning 404 Not Found", id);
            return NotFound();
        }

        logger.LogInformation("[GET] GetCar: returning JSON car with ID '{CarId}' from database", id);
        return Json(data: car, serializerSettings: serializerOptions);
    }

    [HttpPost(Name = "api")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCar(CarDto carDto, [FromServices] IIdentifierGenerator identifierGenerator)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("[POST] CreateCar: model is not valid, returning 400 Bad Request");
            return BadRequest(ModelState);
        }
        
        var car = mapper.Map<Car>(carDto);
        car.Id = identifierGenerator.GetNextIdentifier();

        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        await container.CreateItemAsync(car, partitionKey);

        logger.LogInformation("[POST] CreateCar: sucessfully created car with ID '{CarId}' returning JSON of created car", car.Id);
        return CreatedAtAction(value: car, actionName: nameof(GetCar), routeValues: new { id = car.Id });
    }

    [HttpPut(Name = "api")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCar(Car car)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("[PUT] CreateCar: model is not valid, returning 400 Bad Request");
            return BadRequest(ModelState);
        }
        
        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        await container.ReplaceItemAsync(car, car.Id, partitionKey);
        
        logger.LogInformation("[PUT] EditCar: sucessfully edited car with ID '{CarId}', returning 200 OK", car.Id);
        return Ok();
    }

    [HttpDelete(Name = "api")]
    public async Task<IActionResult> DeleteCar(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("[DELETE] DeleteCar: passed 'id' is not valid, returning 400 Bad Request");
            return BadRequest("Passed 'id' is not valid");
        }

        var container = await cosmosClient.GetOrCreateContainerAsync(databaseId, containerId, partitionKeyPath);
        var response = await container.DeleteItemAsync<Car>(id, partitionKey);

        if (response.StatusCode is not HttpStatusCode.OK)
        {
            logger.LogWarning("[DELETE] DeleteCar: car with passed ID {CarId} not found, returning 404 Not Found", id);
            return NotFound();
        }
        
        logger.LogInformation("[DELETE] DeleteCar: sucessfully deleted car with ID '{CarId}', returning 204 No Content", id);
        return NoContent();
    }

}