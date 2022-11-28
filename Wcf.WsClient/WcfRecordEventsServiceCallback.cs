using System;
using System.Linq;
using Wcf.Common;

namespace Wcf.WsClient;

public class WcfRecordEventsServiceCallback : IRecordEventsServiceCallback
{
    public void UpdateMonitorList(MonitorListUpdateDto request)
    {
        Console.WriteLine($"{nameof(UpdateMonitorList)} called, address - {request.Monitors.First().ServerAddress}");
    }
}