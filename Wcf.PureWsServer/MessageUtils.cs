using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;

namespace Wcf.PureWsServer;

public static class MessageUtils
{
    private static readonly ConstructorInfo BodyWriterMessageConstructor = GetBodyWriterMessageConstructor();

    public static Message CreateResponseMessage(Message requestMessage)
    {
        var responseAction = $"{requestMessage.Headers.Action}Response";
        var responseName = responseAction.Split('/', StringSplitOptions.RemoveEmptyEntries)[^1];
        var bodyWriter = new ResponseBodyWriter(responseName, true);
        var responseMessage = (Message)BodyWriterMessageConstructor.Invoke(new object[] { requestMessage.Version, responseAction, bodyWriter });
        responseMessage.Headers.RelatesTo = requestMessage.Headers.MessageId;
        responseMessage.Headers.To = requestMessage.Headers.ReplyTo.Uri;

        return responseMessage;
    }

    private static ConstructorInfo GetBodyWriterMessageConstructor()
    {
        var type = typeof(Message).Assembly.DefinedTypes
            .First(x => x.FullName == "System.ServiceModel.Channels.BodyWriterMessage");
        return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
            .First(x =>
            {
                var parameters = x.GetParameters();
                return parameters.Length == 3
                    && (parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType) == (typeof(MessageVersion), typeof(string), typeof(BodyWriter));
            });
    }
}