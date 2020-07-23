using System.Xml.Serialization;

namespace ViewModels
{
	[XmlRoot(ElementName = "status")]
	public partial class Status
	{
		[XmlAttribute(AttributeName = "code")]
		public string Code { get; set; }

		[XmlAttribute(AttributeName = "subcode")]
		public string Subcode { get; set; }
	}

	[XmlRoot(ElementName = "OWASP_CSRFTOKEN")]
	public class OWASP_CSRFTOKEN
	{
		[XmlElement(ElementName = "token")]
		public string Token { get; set; }
	}

	[XmlRoot(ElementName = "results")]
	public class TokenViewModel
	{
		[XmlElement(ElementName = "status")]
		public Status Status { get; set; }
		[XmlElement(ElementName = "OWASP_CSRFTOKEN")]
		public OWASP_CSRFTOKEN OWASP_CSRFTOKEN { get; set; }
	}

}
