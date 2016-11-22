using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pub2appveyor.Services.Models
{
	public class AppVeyorEnvironment
	{
		public int DeploymentEnvironmentId { get; set; }
		public string Name { get; set; }
		public string Provider { get; set; }
		public string Created { get; set; }
		public string Updated { get; set; }

	}
}
