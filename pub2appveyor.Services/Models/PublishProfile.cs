using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace pub2appveyor.Services.Models
{
	[XmlRoot(ElementName = "publishProfile")]
	public class PublishProfile
	{
		[XmlAttribute(AttributeName = "profileName")]
		public string ProfileName { get; set; }
		[XmlAttribute(AttributeName = "publishMethod")]
		public string PublishMethod { get; set; }
		[XmlAttribute(AttributeName = "publishUrl")]
		public string PublishUrl { get; set; }
		[XmlAttribute(AttributeName = "msdeploySite")]
		public string MsdeploySite { get; set; }
		[XmlAttribute(AttributeName = "userName")]
		public string UserName { get; set; }
		[XmlAttribute(AttributeName = "userPWD")]
		public string UserPWD { get; set; }
		[XmlAttribute(AttributeName = "destinationAppUrl")]
		public string DestinationAppUrl { get; set; }
		[XmlAttribute(AttributeName = "SQLServerDBConnectionString")]
		public string SQLServerDBConnectionString { get; set; }
		[XmlAttribute(AttributeName = "mySQLDBConnectionString")]
		public string MySQLDBConnectionString { get; set; }
		[XmlAttribute(AttributeName = "hostingProviderForumLink")]
		public string HostingProviderForumLink { get; set; }
		[XmlAttribute(AttributeName = "controlPanelLink")]
		public string ControlPanelLink { get; set; }
		[XmlAttribute(AttributeName = "webSystem")]
		public string WebSystem { get; set; }
		[XmlAttribute(AttributeName = "ftpPassiveMode")]
		public string FtpPassiveMode { get; set; }
	}

	

}
