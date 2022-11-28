using System.Runtime.Serialization;

namespace Wcf.Common;

[DataContract]
public class Event
{
	[DataMember]
	public string Data { get; set; }
}