namespace CarManager.Models.Dto;

public class CarDto
{
    public string Model { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public double Price { get; set; }
    public int Year { get; set; }
}