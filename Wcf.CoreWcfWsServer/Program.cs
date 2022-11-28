using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using Wcf.Common;
using Wcf.CoreWcfWsServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServiceModelServices();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder
        .AddService<RecordEventsService>(_ => { })
        .AddServiceEndpoint<RecordEventsService, IRecordEventsService>(CreateServiceBinding(), string.Empty);
});

app.Run();

static Binding CreateServiceBinding()
{
    return new NetHttpBinding
    {
        Name = "RecordEventsService",

        MaxReceivedMessageSize = int.MaxValue,
        MaxBufferSize = int.MaxValue,
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