
using Blazored.LocalStorage;
using Shared.Constraints;
using Shared.Models;
using Shared.Models.Devices;

namespace Infrastructure.Extensions;

public static class CachingExtensions
{
    public static async Task UpsertItemToList<T>(this ILocalStorageService _storage, string listName, T item) where T : IIdentifiedEntity
    {
        var list = await _storage.GetItemAsync<List<T>>(listName);

        int index = list.FindIndex(x => x.Id == item.Id);

        if (index != -1)
            list[index] = item;
        else
            list.Add(item);

        await _storage.SetItemAsync(listName, list);
    }

    public static async Task RemoveItemFromList<T>(this ILocalStorageService _storage, string listName, string idToRemove) where T : IIdentifiedEntity
    {
        var list = await _storage.GetItemAsync<List<T>>(DeviceConstants.DevicesListName);
        list.RemoveAll(broker => broker.Id == idToRemove);
        await _storage.SetItemAsync(BrokerConstraints.BrokerListName, list);
    }
}
