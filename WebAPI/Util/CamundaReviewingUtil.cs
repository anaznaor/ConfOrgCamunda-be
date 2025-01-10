using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client;
using Camunda.Api.Client.Message;
using Camunda.Api.Client.UserTask;
using Camunda.Api.Client.User;
using Newtonsoft.Json;
using System.Text;
using Camunda.Api.Client.History;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NgrokExtensions;
using System;
using WebAPI.Util.Dto;

namespace WebAPI.Util
{
    public class CamundaReviewingUtil
    {
        private const string camundaEngineUri = "http://localhost:8080/engine-rest";
        private static CamundaClient client = CamundaClient.Create(camundaEngineUri);
        private const string processKey = "ReviewingProcess";


        public static async Task<string> StartReviewingProcess(string guest, string coordinator, int idConference)
        {
            var parameters = new Dictionary<string, object>();
            parameters["Guest"] = guest;
            parameters["Coordinator"] = coordinator;
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

        public static async Task<List<ProcessInfo>> GetProcess(string processKey, bool details)
        {

            var historyList = await client.History.ProcessInstances.Query(new HistoricProcessInstanceQuery { ProcessDefinitionKey = processKey }).List();
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
            return processes;
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
            process.IdPaper = list.Where(v => v.Name == "IdPaper")
                                .Select(v => Convert.ToInt32(v.Value))
                                .First();
            process.Coordinator = list.Where(v => v.Name == "Coordinator")
                                            .Select(v => v.Value.ToString())
                                            .First();
            process.Guest = list.Where(v => v.Name == "Guest")
                                .Select(v => v.Value.ToString())
                                .First();
            

            if (details)
            {
                if(list.Find(v => v.Name == "PaperSubmissionEnd") != null)
                {
                    process.PaperSubmissionEnd = list.Where(v => v.Name == "PaperSubmissionEnd")
                                            .Select(v => v.Value.ToString())
                                            .First();
                }
                
                if (list.Find(v => v.Name == "AverageGrade") != null)
                {
                    process.AverageGrade = list.Where(v => v.Name == "AverageGrade")
                                                .Select(v => Convert.ToInt32(v.Value))
                                                .First();
                }

                if (list.First().ProcessDefinitionKey == processKey)
                {
                   
                    var reviewers = list.Where(v => v.Name == "Reviewers")
                                 .Select(v => v.Value).FirstOrDefault();
                    if (reviewers != null)
                    {
                        process.Reviewers = new List<string>([]);
                        foreach (var r in JsonConvert.DeserializeObject<List<string>>(reviewers.ToString()))
                        {
                            process.Reviewers.Add(r);
                        }
                    }
                }
            }

            var timePassed = list.Where(v => v.Name == "TimePassed")
                                  .Select(v => v.Value)
                                  .FirstOrDefault();
            process.TimePassed = timePassed == null || !Convert.ToBoolean(timePassed);
        }

        public static async Task<List<TaskInfo>> GetTasks(string username)
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

            if (variables.TryGetValue("Reviewer", out value))
            {
                task.Reviewer = value.GetValue<string>();
            }
        }

        public static async Task<List<TaskInfo>> GetTasksByPID(string username, string processInstanceId)
        {
            var userTasks = await client.UserTasks
                                        .Query(new TaskQuery
                                        {
                                            Assignee = username,
                                            ProcessInstanceId = processInstanceId
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

        public static async Task UploadPaper(int idPaper, string guest, string taskId, int NoOfUploads)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["Guest"] = VariableValue.FromObject(guest);
            variables["IdPaper"] = VariableValue.FromObject(idPaper);
            variables["NoOfUploads"] = VariableValue.FromObject(NoOfUploads);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task PreliminaryCheckOfPaper(bool preliminaryCheck, string processId)
        {
            var process = await GetProcessById(processId, false);
            if(process.Coordinator != null)
            {
                var tasks = await GetTasksByPID(process.Coordinator, processId);
                var taskId = tasks.Find(t => t.TaskName == "Preliminary check of paper")?.TID;
                if(taskId != null)
                {
                    var variables = new Dictionary<string, VariableValue>();
                    variables["PreliminaryCheck"] = VariableValue.FromObject(preliminaryCheck);
                    variables["IdPaper"] = VariableValue.FromObject(process.IdPaper);
                    await client.UserTasks[taskId].Complete(new CompleteTask()
                    {
                        Variables = variables
                    });
                }
            }
        }

        public static async Task AssignePaperReviewers(List<string> emails, string processId)
        {
            var process = await GetProcessById(processId, false);
            if (process.Coordinator != null)
            {
                var tasks = await GetTasksByPID(process.Coordinator, processId);
                var taskId = tasks.Find(t => t.TaskName == "Assign paper reviewers")?.TID;
                if (taskId != null)
                {
                    var variables = new Dictionary<string, VariableValue>();
                    variables["Coordinator"] = VariableValue.FromObject(process.Coordinator);
                    variables["IdPaper"] = VariableValue.FromObject(process.IdPaper);
                    variables["Reviewers"] = VariableValue.FromObject(emails);
                    await client.UserTasks[taskId].Complete(new CompleteTask()
                    {
                        Variables = variables
                    });
                }
            }
        }

        public static async Task CreateReview(string taskId, int idPaper, string reviewer, int idReview)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["IdPaper"] = VariableValue.FromObject(idPaper);
            variables["IdReview"] = VariableValue.FromObject(idReview);
            variables["Reviewer"] = VariableValue.FromObject(reviewer);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

    }
}
