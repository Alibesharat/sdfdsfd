/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System.Collections.Generic;
using System.Xml.Serialization;
namespace ViewModels
{


	[XmlRoot(ElementName = "sco")]
	public class folder
	{
		[XmlElement(ElementName = "domain-name")]
		public string Domainname { get; set; }
		[XmlAttribute(AttributeName = "tree-id")]
		public string Treeid { get; set; }
		[XmlAttribute(AttributeName = "sco-id")]
		public string Scoid { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
	}

	[XmlRoot(ElementName = "shortcuts")]
	public class Shortcuts
	{
		[XmlElement(ElementName = "sco")]
		public List<folder> Folders { get; set; }
	}

	[XmlRoot(ElementName = "results")]
	public class ShortCutViewModel
	{
		[XmlElement(ElementName = "status")]
		public Status Status { get; set; }
		[XmlElement(ElementName = "shortcuts")]
		public Shortcuts Shortcuts { get; set; }
	}

}
