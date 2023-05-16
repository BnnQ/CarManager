namespace CarManager.Services.Abstractions;

public interface IRepository<TItem, in TIdentifier>
{
    public Task<IEnumerable<TItem>> GetItemsAsync();
    public Task<TItem?> GetItemAsync(TIdentifier id);
    public Task AddItemsAsync(params TItem[] items);
    public Task EditItemAsync(TIdentifier id, TItem editedItem);
    public Task DeleteItemAsync(TIdentifier id);
}