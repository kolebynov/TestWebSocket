using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Wcf.Common;

namespace Wcf.WsServer;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
internal sealed class RecordEventsService : IRecordEventsService, IDisposable
{
    private ServiceHost? ServiceHost { get; set; }

    public void Start()
    {
        var serviceBinding = CreateServiceBinding();

        ServiceHost = new ServiceHost(this, new Uri("http://localhost:6060/"));
        ServiceHost.AddServiceEndpoint(typeof(IRecordEventsService), serviceBinding, string.Empty);
        ServiceHost.Open();
    }

    private static Binding CreateServiceBinding()
    {
        return new NetHttpBinding
        {
            Name = "RecordEventsService",

            MaxReceivedMessageSize = int.MaxValue,
            MaxBufferSize = int.MaxValue,
            MaxBufferPoolSize = int.MaxValue,
            ReaderQuotas =
            {
                MaxArrayLength = int.MaxValue,
                MaxStringContentLength = int.MaxValue,
                MaxBytesPerRead = int.MaxValue
            },

            Security =
            {
                Mode = BasicHttpSecurityMode.None,
            },

            OpenTimeout = TimeSpan.FromMinutes(10),
            CloseTimeout = TimeSpan.FromMinutes(10),
            SendTimeout = TimeSpan.FromMinutes(10),
            ReceiveTimeout = TimeSpan.FromMinutes(10),
        };
    }

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

    public void Dispose()
    {
        ((IDisposable)ServiceHost)?.Dispose();
        ServiceHost = null;
    }
}