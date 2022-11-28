using System.ServiceModel.Channels;
using System.Xml;

namespace Wcf.PureWsServer;

public class ResponseBodyWriter : BodyWriter
{
    private readonly string _responseName;

    public ResponseBodyWriter(string responseName, bool isBuffered)
        : base(isBuffered)
    {
        _responseName = responseName;
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
        writer.WriteStartElement(null, _responseName, "http://tempuri.org/");
        writer.WriteEndElement();
    }
}