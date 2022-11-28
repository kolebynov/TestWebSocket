using CoreWCF;
using Wcf.Common;

namespace Wcf.CoreWcfWsServer;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
internal sealed class RecordEventsService : IRecordEventsService
{
    public void IsAlive()
    {
        Console.WriteLine($"{nameof(IsAlive)} called");
        var callback = OperationContext.Current.GetCallbackChannel<IRecordEventsServiceCallback>();
        callback.UpdateMonitorList(new MonitorListUpdateDto
            { Monitors = new List<MonitorEndpointDto> { new() { ServerAddress = "Address" } } });
    }

    public void SubmitEvents(Event request)
    {
        Console.WriteLine($"{nameof(SubmitEvents)} called");
    }
}