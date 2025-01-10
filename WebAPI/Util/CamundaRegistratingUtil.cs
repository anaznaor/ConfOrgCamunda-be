using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Camunda.Api.Client.UserTask;
using Camunda.Api.Client.History;
using System.Diagnostics;
using System;
using WebAPI.Util.Dto;

namespace WebAPI.Util
{
    public class CamundaRegistratingUtil
    {
        private const string camundaEngineUri = "http://localhost:8080/engine-rest";
        private static CamundaClient client = CamundaClient.Create(camundaEngineUri);
        private const string processKey = "GuestRegistrationProcess";

        public static async Task<string> StartRegistratingProcess(string guest, int idConference)
        {
            var parameters = new Dictionary<string, object>();
            parameters["Guest"] = guest;
            parameters["IdConference"] = idConference;
            var processInstanceId = await StartProcess(parameters, processKey);
            return processInstanceId;
        }

        private static async Task<string> StartProcess(Dictionary<string, object> processParameters, string processKey)
        {
            var client = new HttpClient();
            var startProcessUrl = $"{camundaEngineUri}/process-definition/key/{processKey}/start";
            var variables = new Dictionary<string, object>();
            foreach (var param in processParameters)
            {
                variables.Add(param.Key, new { value = param.Value });
            }
            var requestBody = new
            {
                variables = variables
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(startProcessUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var processInstance = JsonConvert.DeserializeObject<ProcessInstanceWithVariables>(responseString);
                return processInstance.Id;
            }
            else
            {
                throw new Exception($"Failed to start process: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }

        public static async Task<ProcessInfo> GetProcessById(string processId, bool details)
        {
            var historyList = await client.History.ProcessInstances.Query(new HistoricProcessInstanceQuery { ProcessInstanceId = processId }).List();
            var processes = historyList.OrderBy(p => p.StartTime)
                                     .Select(p => new ProcessInfo
                                     {
                                         StartTime = p.StartTime,
                                         EndTime = p.State == ProcessInstanceState.Completed ? p.EndTime : new DateTime?(),
                                         Ended = p.State == ProcessInstanceState.Completed,
                                         PID = p.Id
                                     })
                                     .ToList();
            var tasks = new List<Task>();
            foreach (var p in processes)
            {
                tasks.Add(LoadInstanceVariables(p, details));
            }
            await Task.WhenAll(tasks);
            return processes.First();
        }

        private static async Task LoadInstanceVariables(ProcessInfo process, bool details)
        {
            var list = await client.History.VariableInstances.Query(new HistoricVariableInstanceQuery { ProcessInstanceId = process.PID }).List();

            process.IdConference = list.Where(v => v.Name == "IdConference")
                                            .Select(v => Convert.ToInt32(v.Value))
                                            .First();
            if(list.Find(v => v.Name == "IdPaper") != null)
            {
                process.IdPaper = list.Where(v => v.Name == "IdPaper")
                                .Select(v => Convert.ToInt32(v.Value))
                                .First();
            }
            process.Guest = list.Where(v => v.Name == "Guest")
                                .Select(v => v.Value.ToString())
                                .First();

            if (details)
            {
                process.DaysTillConferenceTime = list.Where(v => v.Name == "DaysTillConferenceTime")
                                            .Select(v => Convert.ToInt32(v.Value))
                                            .First();
                process.Accomodation = list.Where(v => v.Name == "Accomodation")
                                    .Select(v => (bool)v.Value)
                                    .First();
                process.EndOfRegistration = list.Where(v => v.Name == "EndOfRegistration")
                                                .Select(v => v.Value.ToString())
                                                .First();
            }
        }

        public static async Task<List<TaskInfo>> GetTasks(string username, string processKey)
        {
            var userTasks = await client.UserTasks
                                        .Query(new TaskQuery
                                        {
                                            Assignee = username,
                                            ProcessDefinitionKey = processKey
                                        })
                                        .List();

            var list = userTasks.OrderBy(t => t.Created)
                                .Select(t => new TaskInfo
                                {
                                    TID = t.Id,
                                    TaskName = t.Name,
                                    TaskKey = t.TaskDefinitionKey,
                                    PID = t.ProcessInstanceId,
                                    StartTime = t.Created,
                                })
                                .ToList();
            var tasks = new List<Task>();
            foreach (var task in list)
            {
                tasks.Add(LoadTaskVariables(task));
            }
            await Task.WhenAll(tasks);
            return list;
        }

        private static async Task LoadTaskVariables(TaskInfo task)
        {
            var variables = await client.UserTasks[task.TID].Variables.GetAll();

            if (variables.TryGetValue("IdConference", out VariableValue value))
            {
                task.IdConference = value.GetValue<int>();
            }
            if (variables.TryGetValue("IdPaper", out VariableValue v))
            {
                task.IdPaper = v.GetValue<int>();
            }
            if (variables.TryGetValue("ProgramCommittee", out value))
            {
                task.ProgramCommittee = value.GetValue<string>();
            }

            if (variables.TryGetValue("GuestLecturer", out value))
            {
                task.GuestLecturer = value.GetValue<string>();
            }

            if (variables.TryGetValue("Guest", out value))
            {
                task.Guest = value.GetValue<string>();
            }

            //if (variables.TryGetValue("Reviewer", out value))
            //{
            //    task.Guest = value.GetValue<string>();
            //}
        }

        public static async Task ChooseTypeOfRegistration(string guest, bool registerWithPaper)
        {
            var tasks = await GetTasks(guest, processKey);
            if (tasks.Count > 0)
            {
                var taskId = tasks.First().TID;
                var processId = tasks.First().PID;
                var process = await GetProcessById(processId, false);
                var variables = new Dictionary<string, VariableValue>();
                variables["Guest"] = VariableValue.FromObject(guest);
                variables["IdConference"] = VariableValue.FromObject(process.IdConference);
                variables["RegisterWithPaper"] = VariableValue.FromObject(registerWithPaper);
                variables["IdPaper"] = VariableValue.FromObject(0);
                await client.UserTasks[taskId].Complete(new CompleteTask()
                {
                    Variables = variables
                });
            }
        }

        public static async Task InputIdOfReviewedPaper(string guest, int idPaper)
        {
            var tasks = await GetTasks(guest, processKey);
            if (tasks.Count > 0)
            {
                var taskId = tasks.First().TID;
                var variables = new Dictionary<string, VariableValue>();
                variables["Guest"] = VariableValue.FromObject(guest);
                variables["IdPaper"] = VariableValue.FromObject(idPaper);
                await client.UserTasks[taskId].Complete(new CompleteTask()
                {
                    Variables = variables
                });
            }
        }

        public static async Task CreateGuestRegistration(string taskId, string guest, int idPaper, bool accomodation, int daysTillConferenceTime)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["Guest"] = VariableValue.FromObject(guest);
            variables["IdPaper"] = VariableValue.FromObject(idPaper);
            variables["Accomodation"] = VariableValue.FromObject(accomodation);
            variables["DaysTillConferenceTime"] = VariableValue.FromObject(daysTillConferenceTime);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task UploadFeePaymentFile(string taskId, int idRegistration)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["IdRegistration"] = VariableValue.FromObject(idRegistration);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }
    }
}
