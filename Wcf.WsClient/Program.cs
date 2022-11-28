using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Wcf.Common;
using Wcf.WsClient;

var client = CreateClient();
client.IsAlive();
Console.WriteLine("IsAlive ok");
client.SubmitEvents(new Event { Data = "eventData" });
Console.WriteLine("SubmitEvents ok");
Console.ReadLine();

IRecordEventsService CreateClient()
{
	var callbackInstance = new InstanceContext(new WcfRecordEventsServiceCallback());

	var serviceEndpointAddress = new EndpointAddress("http://VITALIK-WIN-BLR:6060");
	var serviceEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(IRecordEventsService)), CreateClientBinding(), serviceEndpointAddress);

	var client = new WcfRecordEventsServiceProxy(callbackInstance, serviceEndpoint);
	return client.ChannelFactory.CreateChannel();
}

static Binding CreateClientBinding()
{
	return new NetHttpBinding
	{
		Name = "RecordEventsClient",

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
		UseDefaultWebProxy = true,
	};
}