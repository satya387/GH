using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;
using System.Diagnostics;
using System.IO;
using log4net.Config;


namespace GHAPIServices 
{
    public class GreenhouseApiClient
    {
        private readonly CommonGHAPIService _commonService;
        private readonly RestClient _restClient;
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GreenhouseApiClient));

        public GreenhouseApiClient()
        {
            _commonService = new CommonGHAPIService();
            _restClient = new RestClient();
        }

        public async Task PostAccountDataToGHApiAsync(string apiUrl, string onBehalfOf)
        {
            try
            {
                var customFields = _commonService.GetAccountData();
                var json = _commonService.ConvertAccountDataToJson(customFields);
                if (customFields == null || customFields.Count == 0 || customFields.All(cf => string.IsNullOrEmpty(cf.Name)))
                {
                    var accountHistory = new AccountHistory
                    {
                        Name = null,
                        StatusCode = null,
                        Content = "No Account are created."
                    };

                    _commonService.AddAccountHistory(accountHistory);

                    return;
                }
                else
                {
                    var request = new RestRequest(apiUrl, Method.Post)
                     .AddHeader("Authorization", "Basic NWM2ODhjMDNlNTIzMTNhYWMxYjgxMmFmZTg2YzkyODYtMzo=")
                     .AddHeader("On-Behalf-Of", onBehalfOf)
                     .AddHeader("Content-Type", "application/json")
                     .AddJsonBody(json);
                    var response = await _restClient.ExecuteAsync(request);
                    var jsonResponse = JObject.Parse(json);
                    var options = jsonResponse["options"];

                    if (options != null && options.HasValues)
                    {
                        var name = jsonResponse.ToString();

                        if (!string.IsNullOrEmpty(name))
                        {
                            var statusCode = response.StatusCode.ToString();
                            var content = response.Content;
                            var accountHistory = new AccountHistory
                            {
                                Name = name,
                                StatusCode = statusCode,
                                Content = content
                            };
                            _commonService.AddAccountHistory(accountHistory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex + "Error on PostAccountDataToGHApiAsync Method");
            }
            
        }

        public async Task PostProjectDataToGHApiAsync(string apiUrl1, string onBehalfOf)
        {
            try
            {
                var customFields = _commonService.GetProjectData();
                var json = _commonService.ConvertProjectDataToJson(customFields);
                Logger.Info("getting Project json data " + json);
                if (customFields == null || customFields.Count == 0 || customFields.All(cf => string.IsNullOrEmpty(cf.Name)))
                {
                    var projetcHistory = new ProjectHistory
                    {
                        Name = null,
                        StatusCode = null,
                        Content = "No Project are created."
                    };

                    _commonService.AddProjectHistory(projetcHistory);
                    Logger.Info("no Project data on this day added that empty fields to AddProjectHistory");
                    return;
                }
                else
                {
                    var request = new RestRequest(apiUrl1, Method.Post)
                         .AddHeader("Authorization", "Basic NWM2ODhjMDNlNTIzMTNhYWMxYjgxMmFmZTg2YzkyODYtMzo=")
                         .AddHeader("On-Behalf-Of", onBehalfOf)
                         .AddHeader("Content-Type", "application/json")
                         .AddJsonBody(json);
                    Logger.Info("Requsting Post data sending to Green house " + apiUrl1);
                    var response = await _restClient.ExecuteAsync(request);
                    Logger.Info("Getting repomse back from Green house " + response.Content);
                    var jsonResponse = JObject.Parse(json);
                    var options = jsonResponse["options"];

                    if (options != null && options.HasValues)
                    {
                        var name = jsonResponse.ToString();

                        if (!string.IsNullOrEmpty(name))
                        {
                            var statusCode = response.StatusCode.ToString();
                            var content = response.Content;
                            var projetcHistory = new ProjectHistory
                            {
                                Name = name,
                                StatusCode = statusCode,
                                Content = content
                            };
                            _commonService.AddProjectHistory(projetcHistory);
                            Logger.Info("Response from Green house and data sending to sp " + name + " - " + statusCode + " - " + content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex + " Error on PostProjectDataToGHApiAsync Method");
            }
            
        }

        public static async Task Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure(new FileInfo("app.config"));
                Logger.Info("Application is starting...");
                string apiUrl = "https://harvest.greenhouse.io/v1/custom_field/12607913003/custom_field_options";
                Logger.Info("Assiging GreenHouse post api for Account Data " + apiUrl);
                string apiUrl1 = "https://harvest.greenhouse.io/v1/custom_field/14427798003/custom_field_options";
                Logger.Info("Assiging GreenHouse post api for Project Data " + apiUrl1);
                string onBehalfOf = "4065322003";

                var client = new GreenhouseApiClient();
                await client.PostAccountDataToGHApiAsync(apiUrl, onBehalfOf);
                Logger.Info("Sending Accout Api to PostAccountDataToGHApiAsync "+ apiUrl);
                await client.PostProjectDataToGHApiAsync(apiUrl1, onBehalfOf);
                Logger.Info("Sending Project Api to PostProjectDataToGHApiAsync "+ apiUrl1);
            }
            catch (Exception ex)
            {
                Logger.Error(ex + " Error on Main Method");
            }
            Logger.Info("Application is ending...");
        }
    }
}
