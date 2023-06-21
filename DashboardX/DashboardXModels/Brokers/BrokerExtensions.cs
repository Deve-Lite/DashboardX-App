

namespace DashboardXModels.Brokers;

public static class BrokerExtensions
{
    public static DateTime CreatedAt(this Broker broker) => new(broker.CreatedAtTicks);

    public static DateTime EditedAt(this Broker broker) => new(broker.EditedAtTicks);
}

