using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Web.UI.WebControls;

namespace RunSimioPortalExpConsole
{
    public static class SimioPortalWebAPIHelper
    {
        internal static string Token = String.Empty;
        internal static ICredentials Credentials;
        internal static bool UseDefaultCredentials;
        internal static Uri Uri;
        internal static string Url = Properties.Settings.Default.URL;
        internal static string PersonalAccessToken = Properties.Settings.Default.PersonalAccessToken;
        internal static Int32 RetryResultsIntervalSeconds = Properties.Settings.Default.RetryResultsIntervalSeconds;
        internal static Int32 RetryResultsMaxAttempts = Properties.Settings.Default.RetryResultsMaxAttempts;
        internal static string AuthenticationType = Properties.Settings.Default.AuthenticationType;
        internal static string Domain = Properties.Settings.Default.Domain;
        internal static string UserName = Properties.Settings.Default.UserName;
        internal static string Password = Properties.Settings.Default.Password;
        internal static string ProjectName = Properties.Settings.Default.ProjectName;
        internal static string RunSchedulePlanScenarioName = Properties.Settings.Default.RunSchedulePlanScenarioName;
        internal static string RunExperimentRunDesc = Properties.Settings.Default.RunExperimentRunDesc;
        internal static string publishScheduleName = Properties.Settings.Default.PublishScheduleName;
        internal static string PublishScheduleDescription = Properties.Settings.Default.PublishScheduleDescription;
        internal static bool ImportAllTables = Properties.Settings.Default.ImportAllTables;
        internal static bool RunSchedule = Properties.Settings.Default.RunSchedule;
        internal static bool RunScheduleRiskAnalysis = Properties.Settings.Default.RunScheduleRiskAnalysis;
        internal static string RunExperimentScenariosJSON = Properties.Settings.Default.RunExperimentScenariosJSON;
        internal static bool PublishScheduleRun = Properties.Settings.Default.PublishSchduleRun;
        internal static bool WaitAtEnd = Properties.Settings.Default.WaitAtEnd;
        internal static string RunScheduleControlValuesArray = Properties.Settings.Default.RunScheduleControlValuesArray;
        internal static bool RunExperiment = Properties.Settings.Default.RunExperiment;
        internal static bool PublishExperimentRun = Properties.Settings.Default.PublishExperimentRun;
        internal static string PublishExperimentRunName = Properties.Settings.Default.PublishExperimentRunName;
        internal static string PublishExperimentRunDescription = Properties.Settings.Default.PublishExperimetnRunDescription;
        internal static bool ExportAllTablesAndLogs = Properties.Settings.Default.ExportAllTablesAndLogs;
        internal static Int32 RunLengthDays = Properties.Settings.Default.RunLengthDays;
        internal static string StartTimeSelection = Properties.Settings.Default.StartTimeSelection;
        internal static Int32 BearerTokenRefreshIntervalMinutes = Properties.Settings.Default.BearerTokenRefreshIntervalMinutes;
        internal static DateTime BearerTokenRetrievalTime = DateTime.MinValue;

        internal static void setCredentials()
        {
            //
            // Resolve values to objects where necessary
            //
            var cache = new CredentialCache();
            Credentials = null;

            if (AuthenticationType.ToLower() == "currentuser")
            {
                // Not sure if we need both lines, but we'll do it anyway
                UseDefaultCredentials = true;
                Credentials = CredentialCache.DefaultCredentials;
            }
            else if (String.IsNullOrWhiteSpace(UserName) == false)
            {
                Credentials = cache;
                var rootUrl = new Uri(Uri.GetLeftPart(UriPartial.Authority));
                if (String.IsNullOrWhiteSpace(Domain) == false)
                    cache.Add(rootUrl, AuthenticationType, new NetworkCredential(UserName, Password ?? String.Empty));
                else
                    cache.Add(rootUrl, AuthenticationType, new NetworkCredential(UserName, Password ?? String.Empty, Domain));
            }
        }

        internal static void checkAndObtainBearerToken()
        {
            if (Token.Length == 0 || BearerTokenRetrievalTime.AddMinutes(BearerTokenRefreshIntervalMinutes) <= DateTime.Now)
            {
                var client = new RestClient(Uri + "/api/RequestToken");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
                {
                    if (UseDefaultCredentials)
                    {
                        request.UseDefaultCredentials = true;
                    }
                    else
                    {
                        client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                    }
                }
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\n    \"PersonalAccessToken\": \"" + PersonalAccessToken + "\",\n    \"Purpose\": \"PublicAPI\"\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                    else throw new Exception(response.Content);
                }
                var xmlDoc = responseToXML(response.Content);
                XmlNodeList node = xmlDoc.GetElementsByTagName("Token");
                Token = node[0].InnerText;
                BearerTokenRetrievalTime = DateTime.Now;
                Console.WriteLine("Bearer Token Received Successfully");
            }
        }

        internal static Int32[] findExperimentIds(bool forSchedules)
        {
            checkAndObtainBearerToken();
            var client = new RestClient(Uri + "/api/Query");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer " + Token);
            if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
            {
                if (UseDefaultCredentials)
                {
                    request.UseDefaultCredentials = true;
                }
                else
                {
                    client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                }
            }
            request.AlwaysMultipartFormData = true;
            request.AddParameter("Type", "GetExperimentRuns");
            request.AddParameter("Query", "{\"ReturnNonOwnedRuns\":false}");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                else throw new Exception(response.Content);
            }
            var xmlDoc = responseToXML(response.Content);
            Int32[] returnInt = new int[2];
            var dataNodes = xmlDoc.SelectSingleNode("data");
            foreach (XmlNode itemNodes in dataNodes)
            {
                XmlNodeList projectNode = ((XmlElement)itemNodes).GetElementsByTagName("ProjectName");
                XmlNodeList expRunDescriptionNode = ((XmlElement)itemNodes).GetElementsByTagName("Description");
                XmlNodeList scenarioNamesNode = ((XmlElement)itemNodes).GetElementsByTagName("ScenarioNames");
                if (ProjectName == projectNode[0].InnerText &&
                    (forSchedules == true && RunSchedulePlanScenarioName == scenarioNamesNode[0].InnerXml) ||
                     (forSchedules == false && RunExperimentRunDesc == expRunDescriptionNode[0].InnerXml))
                {
                    XmlNodeList idNode = ((XmlElement)itemNodes).GetElementsByTagName("Id");
                    XmlNodeList experimentIdNode = ((XmlElement)itemNodes).GetElementsByTagName("ExperimentId");
                    returnInt[0] = Convert.ToInt32(idNode[0].InnerXml);
                    returnInt[1] = Convert.ToInt32(experimentIdNode[0].InnerXml);
                    break;
                }
            }
            if (returnInt[1] == 0)
            {
                throw new Exception("Experiment Run Cannot Be Found");
            }
            return returnInt;
        }

        internal static void exportAllExperimentRunScenarioTableAndLogData(Int32 existingExperimentRuntId)
        {
            checkAndObtainBearerToken();
            var client = new RestClient(Uri + "/api/Command");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer " + Token);
            if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
            {
                if (UseDefaultCredentials)
                {
                    request.UseDefaultCredentials = true;
                }
                else
                {
                    client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                }
            }
            request.AlwaysMultipartFormData = true;
            request.AddParameter("Type", "ExportTablesAndLogs");
            request.AddParameter("Command", "{\"ExperimentRunId\": " + existingExperimentRuntId.ToString() + ",\"ExportAllLogs\":true,\"ExportAllTables\":true, \"ScenarioName\": \"" + RunSchedulePlanScenarioName + "\"}");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                else throw new Exception(response.Content);
            }
            var xmlDoc = responseToXML(response.Content);
            var successedNode = xmlDoc.SelectSingleNode("data/Succeeded");
            if (successedNode.InnerText.ToLower() == "false")
            {
                var failureMessageNode = xmlDoc.SelectSingleNode("data/FailureMessage");
                throw new Exception(failureMessageNode.InnerText);
            }
        }

        internal static void importAllExperimentRunScenarioTableData(Int32 existingExperimentRuntId, string correlationId)
        {
            checkAndObtainBearerToken();
            var client = new RestClient(Uri + "/api/Command");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer " + Token);
            if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
            {
                if (UseDefaultCredentials)
                {
                    request.UseDefaultCredentials = true;
                }
                else
                {
                    client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                }
            }
            request.AlwaysMultipartFormData = true;
            request.AddParameter("Type", "ImportExperimentRunScenarioTableData");
            request.AddParameter("Command", "{\"ExperimentRunId\": " + existingExperimentRuntId.ToString() + ", \"ScenarioName\": \"" + RunSchedulePlanScenarioName + "\", \"CorrelationId\": \"" + correlationId + "\"}");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                else throw new Exception(response.Content);
            }
            var xmlDoc = responseToXML(response.Content);
            var successedNode = xmlDoc.SelectSingleNode("data/Succeeded");
            if (successedNode.InnerText.ToLower() == "false")
            {
                var failureMessageNode = xmlDoc.SelectSingleNode("data/FailureMessage");
                throw new Exception(failureMessageNode.InnerText);
            }
        }

        internal static void setExperimentRunScenarioControlValue(Int32 existingExperimentRuntId, string controlName, string controlValue)
        {
            checkAndObtainBearerToken();
            var client = new RestClient(Uri + "/api/Command");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer " + Token);
            if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
            {
                if (UseDefaultCredentials)
                {
                    request.UseDefaultCredentials = true;
                }
                else
                {
                    client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                }
            }
            request.AlwaysMultipartFormData = true;
            request.AddParameter("Type", "SetExperimentRunScenarioControlValue");
            request.AddParameter("Command", "{\"ExperimentRunId\": " + existingExperimentRuntId.ToString() + ", \"ScenarioName\": \"" + RunSchedulePlanScenarioName + "\",\"ControlName\": \"" + controlName + "\",\"Value\": \"" + controlValue + "\"}");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                else throw new Exception(response.Content);
            }
            var xmlDoc = responseToXML(response.Content);
            var successedNode = xmlDoc.SelectSingleNode("data/Succeeded");
            if (successedNode.InnerText.ToLower() == "false")
            {
                var failureMessageNode = xmlDoc.SelectSingleNode("data/FailureMessage");
                throw new Exception(failureMessageNode.InnerText);
            }
        }

        internal static void setExperimentRunScenarioStartTimeSelectionRunLengthDays(Int32 existingExperimentRuntId)
        {
            checkAndObtainBearerToken();
            var client = new RestClient(Uri + "/api/Command");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer " + Token);
            if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
            {
                if (UseDefaultCredentials)
                {
                    request.UseDefaultCredentials = true;
                }
                else
                {
                    client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                }
            }
                
            request.AlwaysMultipartFormData = true;
            request.AddParameter("Type", "SetExperimentRunScenarioStartEndType");
            request.AddParameter("Command", "{\"ExperimentRunId\": " + existingExperimentRuntId.ToString() + ", \"TimeOptions\": {\"IsSpecificStartTime\": false,\"StartTimeSelection\": \"" + StartTimeSelection + "\", \"IsSpecificEndTime\": false,\"EndTimeSelection\": \"Days\",\"EndTimeRunValue\": " + RunLengthDays.ToString() + "}}");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                else throw new Exception(response.Content);
            }
            var xmlDoc = responseToXML(response.Content);
            var successedNode = xmlDoc.SelectSingleNode("data/Succeeded");
            if (successedNode.InnerText.ToLower() == "false")
            {
                var failureMessageNode = xmlDoc.SelectSingleNode("data/FailureMessage");
                throw new Exception(failureMessageNode.InnerText);
            }
        }

        internal static void startExpimentRun(Int32 existingExperimentRuntId, Int32 experimentId, bool forSchedules)
        {
            checkAndObtainBearerToken();
            var client = new RestClient(Uri + "/api/Command");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer " + Token);
            if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
            {
                if (UseDefaultCredentials)
                {
                    request.UseDefaultCredentials = true;
                }
                else
                {
                    client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                }
            }

            request.AlwaysMultipartFormData = true;
            request.AddParameter("Type", "StartExperimentRun");
            if (forSchedules) request.AddParameter("Command", "{\"ExistingExperimentRunId\": " + existingExperimentRuntId.ToString() + ",\"RunPlan\":true,\"RunReplications\":" + RunScheduleRiskAnalysis.ToString().ToLower() + "}");
            else request.AddParameter("Command", "{\"Description\": \"" + RunExperimentRunDesc + "\", \"ExperimentId\": " + experimentId.ToString() + ",\"AllowExportAtEndOfReplication\":true,\"RunReplications\":true, \"CreateInfo\":" + RunExperimentScenariosJSON.ToString() + "}");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                else throw new Exception(response.Content);
            }
            var xmlDoc = responseToXML(response.Content);
            var successedNode = xmlDoc.SelectSingleNode("data/Succeeded");
            if (successedNode.InnerText.ToLower() == "false")
            {
                var failureMessageNode = xmlDoc.SelectSingleNode("data/FailureMessage");
                throw new Exception(failureMessageNode.InnerText);
            }
        }

        internal static Int32 getExperimentRunScenarioTableImports(Int32 experimentRunId, string correlationId)
        {
            Int32 numberOfQueries = 1;
            do
            { 
                checkAndObtainBearerToken();
                var client = new RestClient(Uri + "/api/Query");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "multipart/form-data");
                request.AddHeader("Authorization", "Bearer " + Token);
                if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
                {
                    if (UseDefaultCredentials)
                    {
                        request.UseDefaultCredentials = true;
                    }
                    else
                    {
                        client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                    }
                }
                request.AlwaysMultipartFormData = true;
                request.AddParameter("Type", "GetExperimentRunScenarioTableImports");
                request.AddParameter("Query", "{\"ExperimentRunId\": " + experimentRunId.ToString() + ", \"CorrelationId\": \"" + correlationId + "\"}");

                Console.WriteLine("Get Table Import Status Attempt Number = " + numberOfQueries.ToString());
                IRestResponse response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                    else throw new Exception(response.Content);
                }
                else
                {
                    var xmlDoc = responseToXML(response.Content);
                    XmlNodeList runStatusList = xmlDoc.SelectNodes("data/items");
                    var lastRunStatus = runStatusList[runStatusList.Count - 1];
                    int statusInt = Convert.ToInt32(lastRunStatus["Status"].InnerText);
                    // still running
                    if (statusInt < 2) numberOfQueries++;
                    // success
                    else if (statusInt == 2)
                    {
                        break;
                    }
                    // failure
                    else
                    {
                        throw new Exception(lastRunStatus["StatusMessage"].InnerText);
                    }
                }
                System.Threading.Thread.Sleep(RetryResultsIntervalSeconds * 1000);
            } while (numberOfQueries <= RetryResultsMaxAttempts);

            if (numberOfQueries > RetryResultsMaxAttempts)
            {
                throw new Exception("Number of Retry Results Max Attemps Reached");
            }

            return experimentRunId;
        }

        internal static Int32 findExperimentResults(Int32 experimentId, bool forSchedules)
        {
            Int32 numberOfQueries = 1;
            Int32 experimentRunId = -1;
            do
            {
                checkAndObtainBearerToken();
                var client = new RestClient(Uri + "/api/Query");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "multipart/form-data");
                request.AddHeader("Authorization", "Bearer " + Token);
                if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
                {
                    if (UseDefaultCredentials)
                    {
                        request.UseDefaultCredentials = true;
                    }
                    else
                    {
                        client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                    }
                }
                request.AlwaysMultipartFormData = true;
                request.AddParameter("Type", "GetExperimentRuns");
                request.AddParameter("Query", "{\"ExperimentId\": " + experimentId.ToString() + ",\"ReturnNonOwnedRuns\":false}");

                Console.WriteLine("Get Experiment Results Attempt Number = " + numberOfQueries.ToString());
                IRestResponse response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                    else throw new Exception(response.Content);
                }
                else
                {
                    var xmlDoc = responseToXML(response.Content);
                    XmlNodeList runStatusList;
                    if (forSchedules) runStatusList = xmlDoc.SelectNodes("data/items/AdditionalRunsStatus");
                    else runStatusList = xmlDoc.SelectNodes("data/items");
                    var lastRunStatus = runStatusList[runStatusList.Count - 1];
                    int statusInt = Convert.ToInt32(lastRunStatus["Status"].InnerText);
                    // still running
                    if (statusInt < 2) numberOfQueries++;
                    // success
                    else if (statusInt == 2)
                    {
                        experimentRunId = Convert.ToInt32(lastRunStatus["Id"].InnerText);
                        break;
                    }
                    // failure
                    else
                    {
                        throw new Exception(lastRunStatus["StatusMessage"].InnerText);
                    }
                }
                System.Threading.Thread.Sleep(RetryResultsIntervalSeconds * 1000);
            } while (numberOfQueries <= RetryResultsMaxAttempts);

            if (numberOfQueries > RetryResultsMaxAttempts)
            {
                throw new Exception("Number of Retry Results Max Attemps Reached");
            }

            return experimentRunId;
        }

        internal static void publishResults(Int32 experimenRuntId, bool forSchedules)
        {
            checkAndObtainBearerToken();
            var client = new RestClient(Uri + "/api/Command");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer " + Token);
            if (String.IsNullOrWhiteSpace(AuthenticationType) == false && AuthenticationType.ToLower() != "none")
            {
                if (UseDefaultCredentials)
                {
                    request.UseDefaultCredentials = true;
                }
                else
                {
                    client.Authenticator = new RestSharp.Authenticators.NtlmAuthenticator(Credentials);
                }
            }
            request.AlwaysMultipartFormData = true;
            if (forSchedules)
            {
                request.AddParameter("Type", "PublishScenarioPlanResults");
                request.AddParameter("Command", "{\"ExperimentRunId\": " + experimenRuntId.ToString() + ",\"PublishDescription\": \"" + PublishScheduleDescription + "\",\"PublishName\": \"" + publishScheduleName + "\"}");
            }
            else
            {
                request.AddParameter("Type", "PublishExperimentRun");
                request.AddParameter("Command", "{\"ExperimentRunId\": " + experimenRuntId.ToString() + ",\"PublishDescription\": \"" + PublishExperimentRunDescription + "\",\"PublishName\": \"" + PublishExperimentRunName + "\"}");
            }
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage != null) throw new Exception(response.ErrorMessage);
                else throw new Exception(response.Content);
            }
            else
            {
                var xmlDoc = responseToXML(response.Content);
                var successedNode = xmlDoc.SelectSingleNode("data/Succeeded");
                if (successedNode.InnerText.ToLower() == "false")
                {
                    var failureMessageNode = xmlDoc.SelectSingleNode("data/FailureMessage");
                    throw new Exception(failureMessageNode.InnerText);
                }
            }
        }

        internal static XmlDocument responseToXML(string responseContent)
        {
            string resultString;
            var isProbablyJSONObject = false;
            var isXMLResponse = false;
            using (var stream = GenerateStreamFromString(responseContent))
            using (var reader = new StreamReader(stream))
            {
                resultString = reader.ReadToEnd();
            }

            // We are looking for the first non-whitespace character (and are specifically not Trim()ing here
            //  to eliminate memory allocations on potentially large (we think?) strings
            foreach (var theChar in resultString)
            {
                if (Char.IsWhiteSpace(theChar))
                    continue;

                if (theChar == '{')
                {
                    isProbablyJSONObject = true;
                    break;
                }
                else if (theChar == '<')
                {
                    isXMLResponse = true;
                    break;
                }
                else
                {
                    // Any other character?
                    break;
                }
            }

            XmlDocument xmlDoc;
            if (isProbablyJSONObject == false)
            {
                var prefix = "{ items: ";
                var postfix = "}";

                using (var combinedReader = new StringReader(prefix)
                                            .Concat(new StringReader(resultString))
                                            .Concat(new StringReader(postfix)))
                {
                    var settings = new JsonSerializerSettings
                    {
                        Converters = { new Newtonsoft.Json.Converters.XmlNodeConverter() { DeserializeRootElementName = "data" } },
                        DateParseHandling = DateParseHandling.None,
                    };
                    using (var jsonReader = new JsonTextReader(combinedReader) { CloseInput = false, DateParseHandling = DateParseHandling.None })
                    {
                        xmlDoc = JsonSerializer.CreateDefault(settings).Deserialize<XmlDocument>(jsonReader);
                    }
                }
            }
            else
            {
                xmlDoc = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(resultString, "data");
            }

            return xmlDoc;
        }

        internal static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
