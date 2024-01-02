using Microsoft.AspNetCore.SignalR.Client;

namespace Presentation.DataSync;

public class DataSyncService : ISynchronizer
{
    private string _url;
    private HubConnection _hubConnection;

    public DataSyncService(string basePath)
    {
        _url = $"{basePath}/api/v1/events";
    }

    public async Task Connect()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri(_url))
            .Build();



        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }
}
