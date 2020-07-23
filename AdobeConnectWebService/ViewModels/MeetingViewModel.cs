/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System.Xml.Serialization;
namespace ViewModels
{


	[XmlRoot(ElementName = "sco")]
	public class Sco
	{
		[XmlElement(ElementName = "date-begin")]
		public string Datebegin { get; set; }
		[XmlElement(ElementName = "date-created")]
		public string Datecreated { get; set; }
		[XmlElement(ElementName = "date-end")]
		public string Dateend { get; set; }
		[XmlElement(ElementName = "date-modified")]
		public string Datemodified { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "url-path")]
		public string Urlpath { get; set; }
		[XmlAttribute(AttributeName = "disabled")]
		public string Disabled { get; set; }
		[XmlAttribute(AttributeName = "lang")]
		public string Lang { get; set; }
		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "source-sco-id")]
		public string Sourcescoid { get; set; }
		[XmlAttribute(AttributeName = "sco-id")]
		public string Scoid { get; set; }
		[XmlAttribute(AttributeName = "max-retries")]
		public string Maxretries { get; set; }
		[XmlAttribute(AttributeName = "icon")]
		public string Icon { get; set; }
		[XmlAttribute(AttributeName = "folder-id")]
		public string Folderid { get; set; }
		[XmlAttribute(AttributeName = "display-seq")]
		public string Displayseq { get; set; }
		[XmlAttribute(AttributeName = "account-id")]
		public string Accountid { get; set; }
	}

	[XmlRoot(ElementName = "results")]
	public class MeetingViewModel
	{
		[XmlElement(ElementName = "status")]
		public Status Status { get; set; }
		[XmlElement(ElementName = "sco")]
		public Sco Sco { get; set; }
	}

}
