
using System.Xml.Serialization;
namespace ViewModels
{


	[XmlRoot(ElementName = "principal")]
	public class User
	{
		[XmlElement(ElementName = "login")]
		public string Login { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "ext-login")]
		public string Extlogin { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "has-children")]
		public string Haschildren { get; set; }
		[XmlAttribute(AttributeName = "principal-id")]
		public string Principalid { get; set; }
		[XmlAttribute(AttributeName = "account-id")]
		public string Accountid { get; set; }
	}

	[XmlRoot(ElementName = "results")]
	public class UserViewModel
	{
		[XmlElement(ElementName = "status")]
		public Status Status { get; set; }
		[XmlElement(ElementName = "principal")]
		public Principal Principal { get; set; }
	}

}
