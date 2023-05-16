using AutoMapper;
using CarManager.Models;
using CarManager.Models.Dto;
using CarManager.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CarManager.Controllers;

[Route(template: "api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly IRepository<Car, string> carRepository;
    private readonly ILogger<CarController> logger;
    private readonly IMapper mapper;

    public CarController(IRepository<Car, string> carRepository, IMapper mapper, ILoggerFactory loggerFactory)
    {
        this.carRepository = carRepository;
        this.mapper = mapper;
        logger = loggerFactory.CreateLogger<CarController>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars()
    {
        var cars = await carRepository.GetItemsAsync();

        logger.LogInformation("[GET] GetCars: returning JSON cars from database");
        return cars.ToList();
    }

    [HttpGet(template: "{id}")]
    public async Task<ActionResult<Car>> GetCar(string id)
    {
        var car = await carRepository.GetItemAsync(id);
        if (car is null)
        {
            logger.LogWarning("[GET] GetCar: car with ID '{CarId}' not found, returning 404 Not Found", id);
            return NotFound();
        }

        logger.LogInformation("[GET] GetCar: returning JSON car with ID '{CarId}' from database", id);
        return car;
    }

    [HttpPost]
    public async Task<ObjectResult> PostCar(CarDto carDto, [FromServices] IIdentifierGenerator identifierGenerator)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("[POST] PostCar: model is not valid, returning 400 Bad Request");
            return BadRequest(ModelState);
        }

        var car = mapper.Map<Car>(carDto);
        car.Id = identifierGenerator.GetNextIdentifier();

        await carRepository.AddItemsAsync(car);
        logger.LogInformation("[POST] PostCar: sucessfully created car with ID '{CarId}', returning JSON of created car", car.Id);
        return CreatedAtAction(value: car, actionName: nameof(GetCar), routeValues: new { id = car.Id });
    }

    [HttpPut(template: "{id}")]
    public async Task<IActionResult> PutCar(string id, CarDto carDto)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("[PUT] PostCar: model is not valid, returning 400 Bad Request");
            return BadRequest(ModelState);
        }

        var car = mapper.Map<Car>(carDto);
        car.Id = id;

        await carRepository.EditItemAsync(car.Id, car);
        logger.LogInformation("[PUT] PutCar: sucessfully edited car with ID '{CarId}', returning 200 OK", car.Id);
        return NoContent();
    }

    [HttpDelete(template: "{id}")]
    public async Task<IActionResult> DeleteCar(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("[DELETE] DeleteCar: passed 'id' is not valid, returning 400 Bad Request");
            return BadRequest("Passed 'id' is not valid");
        }

        try
        {
            await carRepository.DeleteItemAsync(id);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                "[DELETE] DeleteCar: error occurred when deleting car with id '{CarId}' ({ErrorMessage}), returning 404 Not Found", id,
                exception.Message);
            return NotFound();
        }

        logger.LogInformation("[DELETE] DeleteCar: sucessfully deleted car with ID '{CarId}', returning 204 No Content", id);
        return NoContent();
    }
}