
using System.Xml.Serialization;
namespace ViewModels
{


	[XmlRoot(ElementName = "results")]
	public class stateViewModel
	{
		[XmlElement(ElementName = "status")]
		public Status Status { get; set; }
	}

}