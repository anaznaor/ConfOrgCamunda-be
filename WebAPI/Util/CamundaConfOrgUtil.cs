using AutoMapper;
using Camunda.Api.Client;
using Camunda.Api.Client.History;
using Camunda.Api.Client.Message;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client.UserTask;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAPI.Interface;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Util.Dto;

namespace WebAPI.Util
{
    public class CamundaConfOrgUtil
    {
        private const string camundaEngineUri = "http://localhost:8080/engine-rest";
        private static CamundaClient client = CamundaClient.Create(camundaEngineUri);
        private const string processKeyConf = "ConferenceOrganisationProcess";
        private const string processKeyPC = "InviteProgramCommitteeProcess";
        private const string processKeyGL = "InviteGuestLecturersProcess";

        public static async Task<string> StartConfOrgProcess()
        {
            var parameters = new Dictionary<string, object>();
            var processInstanceId = await StartProcess(parameters, processKeyConf, "conforg");
            return processInstanceId;
        }

        private static async Task<string> StartProcess(Dictionary<string, object> processParameters, string processKey, string tenantId)
        {
            var client = new HttpClient();
            var startProcessUrl = $"{camundaEngineUri}/process-definition/key/{processKey}/tenant-id/{tenantId}/start";
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


        public static async Task<TaskInfo> GetTask(string processInstanceId)
        {
            var userTasks = await client.UserTasks
                                        .Query(new TaskQuery
                                        {
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
            return list.First();
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
        }

        public static async Task<List<TaskInfo>> GetTasks(string username)
        {
            var userTasks = await client.UserTasks
                                        .Query(new TaskQuery
                                        {
                                            Assignee = username
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

        public static async Task<string> GetProcess(string processKey)
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
            if(processes != null)
            {
                if(processKey != processKeyConf)
                {
                    var tasks = new List<Task>();
                    foreach (var p in processes)
                    {
                        tasks.Add(LoadInstanceVariables(p, false));
                    }
                    await Task.WhenAll(tasks);
                }
                return processes.First().PID;
            }
            return "";
        }

        public static async Task<ProcessInfo> GetProcessById(string processId)
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
                tasks.Add(LoadInstanceVariables(p, false));
            }
            await Task.WhenAll(tasks);
            return processes.First();
        }

        public static async Task<ProcessInfo> GetProcessWithLists(string processKey)
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
                tasks.Add(LoadInstanceVariables(p, true));
            }
            await Task.WhenAll(tasks);
            return processes.First();
        }

        private static async Task LoadInstanceVariables(ProcessInfo process, bool lists)
        {
            var list = await client.History.VariableInstances.Query(new HistoricVariableInstanceQuery { ProcessInstanceId = process.PID }).List();
            
            if(list.Find(v => v.Name == "IdConference") != null)
            {
                process.IdConference = list.Where(v => v.Name == "IdConference")
                                            .Select(v => Convert.ToInt32(v.Value))
                                            .First();
            }
            
            if (lists)
            {
                if (list.First().ProcessDefinitionKey == processKeyPC)
                {
                    var pc = list.Where(v => v.Name == "ProgComms")
                                 .Select(v => v.Value).FirstOrDefault();
                    if(pc != null)
                    {
                        process.ProgComms = new List<string>([]);
                        foreach (var p in JsonConvert.DeserializeObject<List<string>>(pc.ToString()))
                        {
                            process.ProgComms.Add(p);
                        }
                    }
                }

                if (list.First().ProcessDefinitionKey == processKeyGL)
                {
                    var pc = list.Where(v => v.Name == "GuestLects")
                                 .Select(v => v.Value).FirstOrDefault();
                    if (pc != null)
                    {
                        process.GuestLects = new List<string>([]);
                        foreach (var p in JsonConvert.DeserializeObject<List<string>>(pc.ToString()))
                        {
                            process.GuestLects.Add(p);
                        }
                    }
                }
            }
        }

        public static async Task<string> GetActiveProcess(string processKey)
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
            if (processes.Count > 0)
            {
                foreach(var p in processes)
                {
                    if (p.Ended)
                    {
                        processes.Remove(p);
                    }
                    if (processes.Count == 0)
                        return "";
                }
                if (processes.Count > 0)
                    return processes.First().PID;
                else
                    return "";
            }
            return "null";
        }

        public static async Task<string> GetStates(string processKey)
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
            if (processes.Count > 0)
            {
                var processId = processes.First().PID;
                var activeTasks = await client.UserTasks
                                        .Query(new TaskQuery
                                        {
                                            ProcessInstanceId = processId,
                                        })
                                        .List();
                var task = activeTasks.Find(t => t.Name == "Sending an answer");
                return task != null ? "beforeSend" : "afterSend";
            }
            return "null";
        }

        public static async Task CreateConference(int idConference)
        {
            var processId = await GetProcess(processKeyConf);
            var task = await GetTask(processId);
            var variables = new Dictionary<string, VariableValue>();
            variables["IdConference"] = VariableValue.FromObject(idConference);
            await client.UserTasks[task.TID].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SelectionProgComm(List<string> progComms)
        {
            var processId = await GetProcess(processKeyPC);
            var task = await GetTask(processId);
            var processConfId = await GetProcess(processKeyConf);
            var processConf = await GetProcessById(processConfId);
            var idConf = processConf.IdConference;
            var variables = new Dictionary<string, VariableValue>();
            variables["ProgComms"] = VariableValue.FromObject(progComms);
            variables["IdConference"] = VariableValue.FromObject(idConf);
            await client.UserTasks[task.TID].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SendInviteForProgramCommittee(List<string> progComms)
        {
            var processId = await GetProcess(processKeyPC);
            var task = await GetTask(processId);
            var processConfId = await GetProcess(processKeyConf);
            var processConf = await GetProcessById(processConfId);
            var idConf = processConf.IdConference;
            foreach (var prog in progComms)
            {
                await EmailService.Execute(prog, processKeyPC, $"http://localhost:3000/{idConf}/program-committee/{prog}");
            }
            var variables = new Dictionary<string, VariableValue>();
            variables["ProgComms"] = VariableValue.FromObject(progComms);
            await client.UserTasks[task.TID].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SendAnswerProgComm(bool answer, string user, int idConference)
        {
            var tasks = await GetTasks(user, processKeyPC);
            var taskId = tasks.First().TID;
            var variables = new Dictionary<string, VariableValue>();
            variables["Answer"] = VariableValue.FromObject(answer);
            variables["ProgramCommittee"] = VariableValue.FromObject(user);
            variables["IdConference"] = VariableValue.FromObject(idConference);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SelectionGuestLecturers(List<string> guestsL)
        {
            var processId = await GetProcess(processKeyGL);
            var task = await GetTask(processId);
            var processConfId = await GetProcess(processKeyConf);
            var processConf = await GetProcessById(processConfId);
            var idConf = processConf.IdConference;
            var variables = new Dictionary<string, VariableValue>();
            variables["GuestLects"] = VariableValue.FromObject(guestsL);
            variables["IdConference"] = VariableValue.FromObject(idConf);
            await client.UserTasks[task.TID].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SendInviteForGuestLecturers(List<string> guestLects)
        {
            var processId = await GetProcess(processKeyGL);
            var task = await GetTask(processId);
            var processConfId = await GetProcess(processKeyConf);
            var processConf = await GetProcessById(processConfId);
            var idConf = processConf.IdConference;
            foreach (var guest in guestLects)
            {
                await EmailService.Execute(guest, processKeyGL, $"http://localhost:3000/{idConf}/guest-lecturer/{guest}");
            }
            var variables = new Dictionary<string, VariableValue>();
            variables["GuestLects"] = VariableValue.FromObject(guestLects);
            await client.UserTasks[task.TID].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SendAnswerGuestLect(bool answer, string user, int idConference)
        {
            var tasks = await GetTasks(user);
            var taskId = tasks.First().TID;
            var variables = new Dictionary<string, VariableValue>();
            variables["Answer"] = VariableValue.FromObject(answer);
            variables["GuestLecturer"] = VariableValue.FromObject(user);
            variables["IdConference"] = VariableValue.FromObject(idConference);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task CreateRegistration(int idRegistration, int idPaper, string user)
        {
            var tasks = await GetTasks(user);
            var taskId = tasks.First().TID;
            var variables = new Dictionary<string, VariableValue>();
            variables["IdRegistration"] = VariableValue.FromObject(idRegistration);
            variables["GuestLecturer"] = VariableValue.FromObject(user);
            variables["IdPaper"] = VariableValue.FromObject(idPaper);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SendCallForPaperSubmissionAndRegistration()
        {
            var processId = await GetProcess(processKeyConf);
            var task = await GetTask(processId);

            var variables = new Dictionary<string, VariableValue>();
            await client.UserTasks[task.TID].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task RegistrationDeadlineMsg()
        {
            var client = new HttpClient();
            var camundaEngineUri = "http://localhost:8080/engine-rest";

            var messageUrl = $"{camundaEngineUri}/message";

            var processId = await GetProcess(processKeyConf);
            
            var requestBody = new
            {
                messageName = "RegistrationDeadline",
                processInstanceId = processId
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(messageUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to correlate message: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
