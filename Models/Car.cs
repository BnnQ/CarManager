namespace CarManager.Models;

public class Car
{
    public string Id { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public double Price { get; set; }
    public int Year { get; set; }
}