namespace CarManager.Configuration;

public class Azure
{
    public Cosmos? Cosmos { get; set; }
}

public class Cosmos
{
    public CarsDatabase? CarsDatabase { get; set; }
}

public class CarsDatabase
{
    public string? Id { get; set; }
    public CarsContainer? CarsContainer { get; set; }
}

public class CarsContainer
{
    public string? Id { get; set; }
    public string? PartitionKeyPath { get; set; }
}