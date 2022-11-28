using System.Runtime.Serialization;

namespace Wcf.Common;

[DataContract]
public class MonitorEndpointDto
{
	[DataMember]
	public string ServerAddress { get; set; }
}