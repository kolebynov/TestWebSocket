using System.ServiceModel;

namespace Wcf.Common;

[ServiceContract]
public interface IRecordEventsServiceCallback
{
	[OperationContract(IsOneWay = true)]
	void UpdateMonitorList(MonitorListUpdateDto request);
}