using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using pub2appveyor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace pub2appveyor
{
	class Program
	{
		private static HttpClient httpClient = new HttpClient();

		static void Main(string[] args)
		{
			MainAsync(args).Wait();
			// or, if you want to avoid exceptions being wrapped into AggregateException:
			//  MainAsync().GetAwaiter().GetResult();

			Console.WriteLine("Press ENTER to exit.");
			Console.ReadLine();
		}


		static async Task MainAsync(string[] args)
		{
			string path = Directory.GetCurrentDirectory();
			if (args.Any())
				path = args.First();

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			Console.WriteLine("Wirte output to: " + path);


			//Configure HttpClient for AppVeyor communication
			var appVeyorKey = ConfigurationManager.AppSettings["AppVeyorKey"];
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appVeyorKey);

			//Foreach PublishSettings file in this directory
			var allFiles = Directory.GetFiles(path, "*.PublishSettings");

			if (!allFiles.Any())
				return;

			Console.WriteLine("Number of files found: " + allFiles.Count());


			var allEnvironments = await GetAllAppVeyorEnvironments();


			foreach (var file in allFiles)
			{
				var fileInfo = new FileInfo(file);
				var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

				//Parse it
				var publishData = ParsePublishSettingsFile(file);

				var webDeployProfile = publishData.PublishProfile.Where(x => x.PublishMethod.ToLowerInvariant() == "msdeploy").FirstOrDefault();

				if(webDeployProfile != null)
				{
					//Check if the environment exists on AppVeyor
					if(allEnvironments.Where(x => x.Name.ToLower() == fileName.ToLower()).Any())
					{
						//Update it
						Console.WriteLine("Skipping environment: " + fileName);
					}
					else
					{
						Console.WriteLine("Creating environment: " + fileName);
						//Create it
						EnvironmentDetails newEnv = CreateEnvironmentDetails(fileName, webDeployProfile);

						await CreateEnvironment(newEnv);
					}

				}


			}


		}

		
		private static EnvironmentDetails CreateEnvironmentDetails(string fileName, PublishProfile webDeployProfile)
		{
			EnvironmentDetails newEnv = new EnvironmentDetails()
			{
				Name = fileName,
				Provider = "WebDeploy",
				//Provider = "FTP",
				Settings = new Settings()
				{
					ProviderSettings = new List<ProviderSetting>()
								  {
									  new ProviderSetting() { Name = "server", Value = new Value() { value = string.Format("https://{0}/MSDeploy.axd?site={1}", webDeployProfile.PublishUrl, webDeployProfile.MsdeploySite) } },
									  new ProviderSetting() { Name = "website", Value = new Value() { value = webDeployProfile.MsdeploySite } },
									  new ProviderSetting() { Name = "username", Value = new Value() { value = webDeployProfile.UserName } },
									  new ProviderSetting() { Name = "password", Value = new Value() { value = webDeployProfile.UserPWD } },
									  new ProviderSetting() { Name = "ntlm", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "remove_files", Value = new Value() { value = "true" } },
									  new ProviderSetting() { Name = "app_offline", Value = new Value() { value = "true" } },
									  new ProviderSetting() { Name = "do_not_use_checksum", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "skip_dirs", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "skip_files", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "pre_sync", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "post_sync", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "sync_wait_attempts", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "sync_wait_interval", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "artifact", Value = new Value() { value = null } },
									  new ProviderSetting() { Name = "aspnet5", Value = new Value() { value = null } },
								  },
					EnvironmentVariables = new List<object>()
				}
			};

			return newEnv;
		}

		static PublishData ParsePublishSettingsFile(string path)
		{
			PublishData result = null;

			XmlSerializer serializer = new XmlSerializer(typeof(PublishData));
			using (StreamReader reader = new StreamReader(path))
			{
				result = (PublishData)serializer.Deserialize(reader);
			}

			return result;

		}

		/// <summary>
		/// https://ci.appveyor.com/api/environments
		/// </summary>
		static async Task<List<AppVeyorEnvironment>> GetAllAppVeyorEnvironments()
		{
			using (var response = await httpClient.GetAsync("https://ci.appveyor.com/api/environments"))
			{
				response.EnsureSuccessStatusCode();

				var resultJson = await response.Content.ReadAsStringAsync();

				var result = JsonConvert.DeserializeObject<List<AppVeyorEnvironment>>(resultJson);

				return result;
			}

		}

		private static async Task CreateEnvironment(EnvironmentDetails newEnv)
		{
			var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

			string envJson = JsonConvert.SerializeObject(newEnv, jsonSerializerSettings);

			using (var response = await httpClient.PostAsync("https://ci.appveyor.com/api/environments", new StringContent(envJson, Encoding.UTF8, "application/json")))
			{
				response.EnsureSuccessStatusCode();

				//var resultJson = await response.Content.ReadAsStringAsync();

			}

		}


	}
}
