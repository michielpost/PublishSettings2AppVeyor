using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pub2appveyor.Models
{
	public class Value
	{
		public bool IsEncrypted { get; set; }
		public string value { get; set; }
	}

	public class ProviderSetting
	{
		public string Name { get; set; }
		public Value Value { get; set; }
	}

	public class Settings
	{
		public List<ProviderSetting> ProviderSettings { get; set; }
		public List<object> EnvironmentVariables { get; set; }
	}

	public class EnvironmentDetails
	{
		//public int DeploymentEnvironmentId { get; set; }
		public string Name { get; set; }
		public string Provider { get; set; }
		//public string EnvironmentAccessKey { get; set; }
		public Settings Settings { get; set; }
		//public string Created { get; set; }
	}
}
