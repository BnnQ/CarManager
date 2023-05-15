namespace CarManager.Services.Abstractions;

public interface IIdentifierGenerator
{
    public string GetNextIdentifier();
}