namespace Presentation.Brokers;

public class BrokerService : IBrokerService
{
    private readonly IFetchBrokerService _brokerService;
    private readonly IClientManager _clientManager;

    public BrokerService(IFetchBrokerService brokerService, IClientManager clientManager)
    {
        _brokerService = brokerService;
        _clientManager = clientManager;
    }

    public async Task<IResult> UpdateBroker(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO)
    {
        var result = await _brokerService.UpdateBroker(broker);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var credResult = await _brokerService.UpdateBrokerCredentials(result.Data.Id, brokerCredentialsDTO);

        var updateResult = await _clientManager.UpdateClient(result.Data);

        if (!credResult.Succeeded && updateResult.Succeeded)
            return Result.Warning(message: "Failed to update broker credentilas");

        return updateResult;
    }
    public async Task<IResult> CreateBroker(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO)
    {
        var result = await _brokerService.CreateBroker(broker);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var credResult = await _brokerService.UpdateBrokerCredentials(result.Data.Id, brokerCredentialsDTO);

        var creartedBroker = result.Data;
        var addResult = _clientManager.AddClient(creartedBroker);

        if (!credResult.Succeeded && addResult.Succeeded)
            return Result.Warning(message: "Failed to create broker credentilas but creaded broker.");

        return addResult;
    }
    public async Task<IResult> RemoveBroker(string brokerId)
    {
        var result = await _brokerService.RemoveBroker(brokerId);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var deleteResult = await _clientManager.RemoveClient(brokerId);

        return deleteResult;
    }
}
