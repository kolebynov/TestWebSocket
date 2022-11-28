using System.ServiceModel;

namespace Wcf.Common;

[ServiceContract(CallbackContract = typeof(IRecordEventsServiceCallback))]
public interface IRecordEventsService
{
	[OperationContract]
	void IsAlive();

	[OperationContract]
	void SubmitEvents(Event request);
}