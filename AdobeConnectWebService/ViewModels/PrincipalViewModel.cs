
using System.Xml.Serialization;
using System.Collections.Generic;
namespace ViewModels
{
	

	[XmlRoot(ElementName = "principal")]
	public class Principal
	{
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "login")]
		public string Login { get; set; }
		[XmlElement(ElementName = "display-uid")]
		public string Displayuid { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlAttribute(AttributeName = "training-group-id")]
		public string Traininggroupid { get; set; }
		[XmlAttribute(AttributeName = "is-ecommerce")]
		public string Isecommerce { get; set; }
		[XmlAttribute(AttributeName = "is-hidden")]
		public string Ishidden { get; set; }
		[XmlAttribute(AttributeName = "is-primary")]
		public string Isprimary { get; set; }
		[XmlAttribute(AttributeName = "has-children")]
		public string Haschildren { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "account-id")]
		public string Accountid { get; set; }
		[XmlAttribute(AttributeName = "principal-id")]
		public string Principalid { get; set; }
		[XmlElement(ElementName = "email")]
		public string Email { get; set; }
	}

	[XmlRoot(ElementName = "principal-list")]
	public class Principallist
	{
		[XmlElement(ElementName = "principal")]
		public List<Principal> Principal { get; set; }
	}

	[XmlRoot(ElementName = "results")]
	public class PrincipalViewModel
	{
		[XmlElement(ElementName = "status")]
		public Status Status { get; set; }
		[XmlElement(ElementName = "principal-list")]
		public Principallist Principallist { get; set; }

		
	}

}
