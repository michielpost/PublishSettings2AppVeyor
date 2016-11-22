using pub2appveyor.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace pub2appveyor.Services
{
    public class AppVeyorService
    {
		private HttpClient httpClient = new HttpClient();

		public AppVeyorService(string appVeyorKey)
        {
			//Configure HttpClient for AppVeyor communication
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appVeyorKey);

		}

		public async Task<string> Process(List<IFormFile> allFiles)
		{
			StringBuilder result = new StringBuilder();

			var allEnvironments = await GetAllAppVeyorEnvironments();


			foreach (var file in allFiles)
			{
				var fileInfo = new FileInfo(file.FileName);
				var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

				//Parse it
				var publishData = ParsePublishSettingsFile(file);

				var webDeployProfile = publishData.PublishProfile.Where(x => x.PublishMethod.ToLowerInvariant() == "msdeploy").FirstOrDefault();

				if (webDeployProfile != null)
				{
					//Check if the environment exists on AppVeyor
					if (allEnvironments.Where(x => x.Name.ToLower() == fileName.ToLower()).Any())
					{
						//Update it
						result.AppendLine("Skipping environment: " + fileName);
					}
					else
					{
						result.AppendLine("Creating environment: " + fileName);
						//Create it
						EnvironmentDetails newEnv = CreateEnvironmentDetails(fileName, webDeployProfile);

						await CreateEnvironment(newEnv);
					}

				}


			}

			return result.ToString();
		}


		private EnvironmentDetails CreateEnvironmentDetails(string fileName, PublishProfile webDeployProfile)
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
									  new ProviderSetting() { Name = "aspnet_core", Value = new Value() { value = "true" } },
									  new ProviderSetting() { Name = "aspnet_core_force_restart", Value = new Value() { value = "true" } },
								  },
					EnvironmentVariables = new List<object>()
				}
			};

			return newEnv;
		}

		PublishData ParsePublishSettingsFile(IFormFile file)
		{
			PublishData result = null;

			XmlSerializer serializer = new XmlSerializer(typeof(PublishData));
			using (StreamReader reader = new StreamReader(file.OpenReadStream()))
			{
				result = (PublishData)serializer.Deserialize(reader);
			}

			return result;

		}

		/// <summary>
		/// https://ci.appveyor.com/api/environments
		/// </summary>
		async Task<List<AppVeyorEnvironment>> GetAllAppVeyorEnvironments()
		{
			using (var response = await httpClient.GetAsync("https://ci.appveyor.com/api/environments"))
			{
				response.EnsureSuccessStatusCode();

				var resultJson = await response.Content.ReadAsStringAsync();

				var result = JsonConvert.DeserializeObject<List<AppVeyorEnvironment>>(resultJson);

				return result;
			}

		}

		private async Task CreateEnvironment(EnvironmentDetails newEnv)
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
