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
    class Program 
    {       

        static void Main(string[] args)
        {
            int arrayIdx = -1;
            bool parametersQuestioned = false;

            Console.WriteLine("Reading Parameters");
            try
            {
                foreach (string arrayArg in args)
                {
                    arrayIdx++;
                    switch (arrayArg)
                    {
                        case "-url":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.Url = args[arrayIdx + 1];
                            }
                            break;
                        case "-pat":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.PersonalAccessToken = args[arrayIdx + 1];
                            }
                            break;
                        case "-ris":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RetryResultsIntervalSeconds = Convert.ToInt32(args[arrayIdx + 1]);
                            }
                            break;
                        case "-rma":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RetryResultsMaxAttempts = Convert.ToInt32(args[arrayIdx + 1]);
                            }
                            break;
                        case "-at":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.AuthenticationType = args[arrayIdx + 1];
                            }
                            break;
                        case "-d":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.Domain = args[arrayIdx + 1];
                            }
                            break;
                        case "-u":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.UserName = args[arrayIdx + 1];
                            }
                            break;
                        case "-p":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.Password = args[arrayIdx + 1];
                            }
                            break;
                        case "-prj":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.ProjectName = args[arrayIdx + 1];
                            }
                            break;
                        case "-rsn":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunSchedulePlanScenarioName = args[arrayIdx + 1];
                            }
                            break;
                        case "-erd":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunExperimentRunDesc = args[arrayIdx + 1];
                            }
                            break;
                        case "-psn":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.publishScheduleName = args[arrayIdx + 1];
                            }
                            break;
                        case "-psd":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.PublishScheduleDescription = args[arrayIdx + 1];
                            }
                            break;
                        case "-rs":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunSchedule = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-rra": 
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunScheduleRiskAnalysis = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-rej":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunExperimentScenariosJSON = args[arrayIdx + 1];
                            }
                            break;
                        case "-psr":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.PublishScheduleRun = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-w":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.WaitAtEnd = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-rsc":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunScheduleControlValuesArray = args[arrayIdx + 1];
                            }
                            break;
                        case "-re":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunExperiment = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-per":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.PublishExperimentRun = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-pen":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.PublishExperimentRunName = args[arrayIdx + 1];
                            }
                            break;
                        case "-ped":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.PublishExperimentRunDescription = args[arrayIdx + 1];
                            }
                            break;
                        case "-rld":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.RunLengthDays = Convert.ToInt32(args[arrayIdx + 1]);
                            }
                            break;
                        case "-iat":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.ImportAllTables = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-eat":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.ExportAllTablesAndLogs = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-sts":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.StartTimeSelection = args[arrayIdx + 1];
                            }
                            break;
                        case "-sst":
                            if (arrayIdx < args.Length - 1)
                            {
                                if (DateTime.TryParse(args[arrayIdx + 1], out SimioPortalWebAPIHelper.SpecificStartingTime) == false)
                                    throw new Exception("Invalid Date for -sst:" + args[arrayIdx + 1]);
                            }
                            break;
                        case "-btr":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.StartTimeSelection = args[arrayIdx + 1];
                            }
                            break;
                        case "-crn":
                            if (arrayIdx < args.Length - 1)
                            {
                                SimioPortalWebAPIHelper.CreatePlanExperimentRunIfNotFound = Convert.ToBoolean(args[arrayIdx + 1]);
                            }
                            break;
                        case "-?":
                            System.Console.WriteLine("-url = Portal url  (default = " + SimioPortalWebAPIHelper.Url + ")");
                            System.Console.WriteLine("-pat = Personal Access Token  (default = " + SimioPortalWebAPIHelper.PersonalAccessToken.ToString() + ")");
                            System.Console.WriteLine("-ris = Retry Results Interval Seconds  (default = " + SimioPortalWebAPIHelper.RetryResultsIntervalSeconds.ToString() + ")");
                            System.Console.WriteLine("-rma = Retry Results Max Attemps  (default = " + SimioPortalWebAPIHelper.RetryResultsMaxAttempts.ToString() + ")");
                            System.Console.WriteLine("-at = Authentication Type  (default = " + SimioPortalWebAPIHelper.AuthenticationType + ")");
                            System.Console.WriteLine("-d = Domain  (default = " + SimioPortalWebAPIHelper.Domain + ")");
                            System.Console.WriteLine("-u = User Name  (default = " + SimioPortalWebAPIHelper.UserName + ")");
                            System.Console.WriteLine("-p = Password  (default = " + SimioPortalWebAPIHelper.Password + ")");
                            System.Console.WriteLine("-prj = Project Name  (default = " + SimioPortalWebAPIHelper.ProjectName + ")");
                            System.Console.WriteLine("-rsn = Run Schedule Plan Scenario Name  (default = " + SimioPortalWebAPIHelper.RunSchedulePlanScenarioName + ")");
                            System.Console.WriteLine("-rsc = Run Schedule Control Values Array  (default = " + SimioPortalWebAPIHelper.RunScheduleControlValuesArray + ")");
                            System.Console.WriteLine("-rs = Run Schedule  (default = " + SimioPortalWebAPIHelper.RunSchedule.ToString() + ")");
                            System.Console.WriteLine("-rra = Run RiskAnalysis  (default = " + SimioPortalWebAPIHelper.RunScheduleRiskAnalysis.ToString() + ")");
                            System.Console.WriteLine("-psr = Publish Schedule Plan Run (default = " + SimioPortalWebAPIHelper.PublishScheduleRun.ToString() + ")");
                            System.Console.WriteLine("-psn = Publish Schedule Name  (default = " + SimioPortalWebAPIHelper.publishScheduleName + ")");
                            System.Console.WriteLine("-psd = Publish Schedule Description  (default = " + SimioPortalWebAPIHelper.PublishScheduleDescription + ")");
                            System.Console.WriteLine("-erd = Run Experiment Run Desc  (default = " + SimioPortalWebAPIHelper.RunExperimentRunDesc + ")");
                            System.Console.WriteLine("-rej = Run Experiment Scenareios JSON  (default = " + SimioPortalWebAPIHelper.RunExperimentScenariosJSON + ")");
                            System.Console.WriteLine("-re = Run Experiment  (default = " + SimioPortalWebAPIHelper.RunExperiment.ToString() + ")");
                            System.Console.WriteLine("-per = Publish Experiment Run (default = " + SimioPortalWebAPIHelper.PublishExperimentRun.ToString() + ")");
                            System.Console.WriteLine("-pen = Publish Experiment Run Name  (default = " + SimioPortalWebAPIHelper.PublishExperimentRunName + ")");
                            System.Console.WriteLine("-ped = Publish Experiment Run Description  (default = " + SimioPortalWebAPIHelper.PublishExperimentRunDescription + ")");
                            System.Console.WriteLine("-rld = Run Length Days (Scheduling Only) (default = " + SimioPortalWebAPIHelper.RunLengthDays.ToString() + ")");
                            System.Console.WriteLine("-iat = Import All Tables (Scheduling Only)  (default = " + SimioPortalWebAPIHelper.ImportAllTables.ToString() + ")");
                            System.Console.WriteLine("-eat = Export All Tables And Logs (Scheduling Only)  (default = " + SimioPortalWebAPIHelper.ExportAllTablesAndLogs.ToString() + ")");
                            System.Console.WriteLine("-sts = Start Time Selection (Scheduling Only)  (default = " + SimioPortalWebAPIHelper.StartTimeSelection + ")");
                            System.Console.WriteLine("-sst = Specific Starting Time (Scheduling Only)  (default = " + SimioPortalWebAPIHelper.SpecificStartingTime.ToString() + ")");
                            System.Console.WriteLine("-btr = Bearer Token Refresh Interval Minutes (default = " + SimioPortalWebAPIHelper.BearerTokenRefreshIntervalMinutes.ToString() + ")");
                            System.Console.WriteLine("-crn = Create Plan Experiment Run If Not Found (default = " + SimioPortalWebAPIHelper.CreatePlanExperimentRunIfNotFound.ToString() + ")");
                            System.Console.WriteLine("-w = wait (pause) at end  (default = " + SimioPortalWebAPIHelper.WaitAtEnd.ToString() + ")");
                            parametersQuestioned = true;
                            break;
                    }
                }
                if (parametersQuestioned == false) runPortalExperimentMoninitorAndPublishResults();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.Message);
            }
            finally
            {
                if (SimioPortalWebAPIHelper.WaitAtEnd)
                {
                    Console.WriteLine("Press 'Enter' To End");
                    Console.ReadLine();
                }
            }
        }
       
        private static void runPortalExperimentMoninitorAndPublishResults()
        {
            if (Uri.TryCreate(SimioPortalWebAPIHelper.Url, UriKind.Absolute, out SimioPortalWebAPIHelper.Uri) == false)
            {
                throw new Exception("URL Setting in an invalid format");
            }

            if (String.IsNullOrWhiteSpace(SimioPortalWebAPIHelper.AuthenticationType) == false && SimioPortalWebAPIHelper.AuthenticationType.ToLower() != "none")
            {
                Console.WriteLine("Set Credentials");
                SimioPortalWebAPIHelper.setCredentials();
            }

            Console.WriteLine("Obtain Bearer Token");
            SimioPortalWebAPIHelper.checkAndObtainBearerToken();

            // for schedules
            if (SimioPortalWebAPIHelper.ImportAllTables || SimioPortalWebAPIHelper.RunSchedule || SimioPortalWebAPIHelper.ExportAllTablesAndLogs || SimioPortalWebAPIHelper.PublishScheduleRun)
            {
                Console.WriteLine("Find Experiment Ids");
                Int32[] returnInt32 = SimioPortalWebAPIHelper.findExperimentIds(true);

                Int32 experimentRunId = returnInt32[0];
                Int32 experimentId = returnInt32[1];
                if (experimentId == 0)
                {
                    if (SimioPortalWebAPIHelper.CreatePlanExperimentRunIfNotFound == false) throw new Exception("Experiment Run Cannot Be Found");
                    else
                    {
                        // Get Model Id
                        Console.WriteLine("Find Model Id");
                        Int32 modelId = SimioPortalWebAPIHelper.findModelId();
                        if (modelId == 0) throw new Exception("Model Id Cannot Be Found");

                        // Create Experiment Run
                        Console.WriteLine("Create Experiment Run");
                        SimioPortalWebAPIHelper.createExperimentRun(modelId);

                        Console.WriteLine("Find Experiment Ids For New Experiment Run");
                        returnInt32 = SimioPortalWebAPIHelper.findExperimentIds(true);

                        experimentRunId = returnInt32[0];
                        experimentId = returnInt32[1];
                        if (experimentId == 0) throw new Exception("New Experiment Run Cannot Be Found");
                    }
                }
                
                Console.WriteLine("ExperimentRunId:" + experimentRunId.ToString() + "|ExperimentId:" + experimentId.ToString());

                // Valid Example of Control Values : WorkersQty=3|VehiclesQty=1
                if (SimioPortalWebAPIHelper.RunScheduleControlValuesArray.Length > 0)
                {
                https://stackoverflow.com/questions/11673731/parse-a-string-with-name-value-pairs

                    Console.WriteLine("Set Experiment Run Scenario Control Values for ScheduleControlValuesArray : " + SimioPortalWebAPIHelper.RunScheduleControlValuesArray);

                    Dictionary<string, string> keyValuePairs = SimioPortalWebAPIHelper.RunScheduleControlValuesArray.Split('|')
                    .Select(value => value.Split('='))
                    .ToDictionary(pair => pair[0], pair => pair[1]);

                    foreach (KeyValuePair<string, string> p in keyValuePairs)
                    {
                        Console.WriteLine("Set Experiment Run Scenario Control Value For Name=Value : " + p.Key + "=" + p.Value);
                        SimioPortalWebAPIHelper.setExperimentRunScenarioControlValue(experimentRunId, p.Key, p.Value);
                    }
                }

                if (SimioPortalWebAPIHelper.RunSchedule)
                {
                    if (SimioPortalWebAPIHelper.ImportAllTables)
                    {
                        var correlationId = Guid.NewGuid().ToString();
                        Console.WriteLine("Import All Experiment Run Scenario Table Data....CorrelationId:" + correlationId);
                        SimioPortalWebAPIHelper.importAllExperimentRunScenarioTableData(experimentRunId, correlationId);
                        SimioPortalWebAPIHelper.getExperimentRunScenarioTableImports(experimentRunId, correlationId);
                        Console.WriteLine("Get Table Import Status Success");
                    }

                    if (SimioPortalWebAPIHelper.RunLengthDays > 0 && (SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "second" || 
                        SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "minute" || SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "hour" || 
                        SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "day" || SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "week" || 
                        SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "month" || SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "year"))
                    {
                        Console.WriteLine("Set Start Time Selection and Experiment Run Length Days....Start Time Selection:" + SimioPortalWebAPIHelper.StartTimeSelection + "|Run Length Days: " + SimioPortalWebAPIHelper.RunLengthDays.ToString());
                        SimioPortalWebAPIHelper.setExperimentRunScenarioStartTimeSelectionRunLengthDays(experimentRunId);
                    }

                    if (SimioPortalWebAPIHelper.RunLengthDays > 0 && SimioPortalWebAPIHelper.StartTimeSelection.ToLower() == "none" &&
                        SimioPortalWebAPIHelper.SpecificStartingTime > DateTime.MinValue)
                    {
                        Console.WriteLine("Set Specific Starting Time and Experiment Run Length Days....Specific Starting Start:" + SimioPortalWebAPIHelper.SpecificStartingTime.ToString("yyyy-MM-ddTHH:mm:ss") + "|Run Length Days: " + SimioPortalWebAPIHelper.RunLengthDays.ToString());
                        SimioPortalWebAPIHelper.setExperimentRunScenarioSpecificStartingTimeRunLengthDays(experimentRunId);
                    }

                    Console.WriteLine("Start Experiment Run For Schedule");
                    SimioPortalWebAPIHelper.startExpimentRun(experimentRunId, experimentId, true);

                    experimentRunId = SimioPortalWebAPIHelper.findExperimentResults(experimentId, true);
                    Console.WriteLine("ExperimentRunId:" + experimentRunId.ToString());
                }

                if (SimioPortalWebAPIHelper.ExportAllTablesAndLogs)
                {
                    Console.WriteLine("Export All Experiment Run Scenario Table And Log Data");
                    SimioPortalWebAPIHelper.exportAllExperimentRunScenarioTableAndLogData(experimentRunId);
                }

                if (SimioPortalWebAPIHelper.PublishScheduleRun)
                {
                    Console.WriteLine("Publish Schedule Results");
                    SimioPortalWebAPIHelper.publishResults(experimentRunId, true);
                }
            }

            // for experiments
            if (SimioPortalWebAPIHelper.RunExperiment || SimioPortalWebAPIHelper.PublishExperimentRun)
            {
                Console.WriteLine("Find Experiment Ids");
                Int32[] returnInt32 = SimioPortalWebAPIHelper.findExperimentIds(false);
                Int32 experimentRunId = returnInt32[0];
                Int32 experimentId = returnInt32[1];
                Console.WriteLine("ExperimentRunId:" + experimentRunId.ToString() + "|ExperimentId:" + experimentId.ToString());

                if (SimioPortalWebAPIHelper.RunExperiment)
                {
                    Console.WriteLine("Start Experiment Run For Experiment");
                    SimioPortalWebAPIHelper.startExpimentRun(experimentRunId, experimentId, false);
                }

                experimentRunId = SimioPortalWebAPIHelper.findExperimentResults(experimentId, false);

                if (SimioPortalWebAPIHelper.PublishExperimentRun)
                {
                    Console.WriteLine("Publish Experiment Run Results");
                    SimioPortalWebAPIHelper.publishResults(experimentRunId, false);
                }
            }

            Console.WriteLine("Success");
        }       
    }
}
