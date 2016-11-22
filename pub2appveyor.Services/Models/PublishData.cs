using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace pub2appveyor.Services.Models
{
	[XmlRoot(ElementName = "publishData")]
	public class PublishData
	{
		[XmlElement(ElementName = "publishProfile")]
		public List<PublishProfile> PublishProfile { get; set; }
	}
}
