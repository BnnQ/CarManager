using CarManager.Services.Abstractions;

namespace CarManager.Services;

public class GuidIdentifierGenerator : IIdentifierGenerator
{
    public string GetNextIdentifier()
    {
        return Guid.NewGuid()
            .ToString();
    }
}