using Blazored.LocalStorage;
using Shared.Constraints;
using Shared.Models;

namespace Infrastructure.Extensions;

public static class CachingExtensions
{
    public static async Task UpsertItemToList<T>(this ILocalStorageService _storage, string storageListName, T item) where T : IIdentifiedEntity
    {
        var list = await _storage.GetItemAsync<List<T>>(storageListName);

        int index = list.FindIndex(x => x.Id == item.Id);

        if (index != -1)
            list[index] = item;
        else
            list.Add(item);

        await _storage.SetItemAsync(storageListName, list);
    }

    public static async Task RemoveItemFromList<T>(this ILocalStorageService _storage, string storageListName, string idToRemove) where T : IIdentifiedEntity
    {
        try
        {
            var list = await _storage.GetItemAsync<List<T>>(storageListName);
            list.RemoveAll(x => x.Id == idToRemove);
            await _storage.SetItemAsync(storageListName, list);
        }
        catch (Exception e)
        {
            //TODO: Observed weird exception here when removing broker
            Console.WriteLine(e);   
        }
    }

    public static async Task RemoveItemsFromList<T>(this ILocalStorageService _storage, string storageListName, List<string> idsToRemove) where T : IIdentifiedEntity
    {
        try
        {
            var list = await _storage.GetItemAsync<List<T>>(storageListName);

            list = list.Where(x => !idsToRemove.Contains(x.Id))
                .ToList();

            await _storage.SetItemAsync(storageListName, list);
        }
        catch (Exception e)
        {
            //TODO: Observed weird exception here when removing broker
            Console.WriteLine(e);
        }
    }
}
