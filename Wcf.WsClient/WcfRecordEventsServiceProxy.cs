using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using Wcf.Common;

namespace Wcf.WsClient;

internal class WcfRecordEventsServiceProxy : ClientBase<IRecordEventsService>
{
    static WcfRecordEventsServiceProxy()
    {
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

        ServicePointManager.ServerCertificateValidationCallback = delegate
        {
            return true;
        };
    }

    public WcfRecordEventsServiceProxy(InstanceContext callbackInstance, ServiceEndpoint endpoint)
        : base(callbackInstance, endpoint)
    {
    }
}