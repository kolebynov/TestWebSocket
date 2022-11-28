using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wcf.Common;

[DataContract]
public class MonitorListUpdateDto
{
	[DataMember]
	public ICollection<MonitorEndpointDto> Monitors { get; set; }
}