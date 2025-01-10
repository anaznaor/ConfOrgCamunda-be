using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client;
using Newtonsoft.Json;
using System.Text;
using Camunda.Api.Client.Message;
using Camunda.Api.Client.History;
using Camunda.Api.Client.UserTask;
using System.Threading.Tasks;
using System.Diagnostics;
using WebAPI.Util.Dto;
using WebAPI.Models;

namespace WebAPI.Util
{
    public class CamundaScheduleUtil
    {
        private const string camundaEngineUri = "http://localhost:8080/engine-rest";
        private static CamundaClient client = CamundaClient.Create(camundaEngineUri);
        private const string processKey = "CreateScheduleProcess";
        private const string applyMessage = "RegistrationDeadline";

        public static async Task<string> StartSchedulingProcessWithMsg(int idConference)
        {
            var parameters = new Dictionary<string, object>();
            parameters["IdConference"] = idConference;
            await TriggerMessage(parameters, applyMessage);
            return "";
        }

        private static async Task<string> TriggerMessage(Dictionary<string, object> processParameters, string messageName)
        {
            var client = new HttpClient();
            var messageUrl = $"{camundaEngineUri}/message";

            var variables = new Dictionary<string, object>();
            foreach (var param in processParameters)
            {
                variables.Add(param.Key, new { value = param.Value });
            }

            var requestBody = new
            {
                messageName = messageName,
                businessKey = processKey,
                processVariables = variables
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(messageUrl, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var processInstance = JsonConvert.DeserializeObject<ProcessInstanceWithVariables>(responseString);
                return "";
            }
            else
            {
                throw new Exception($"Failed to trigger message: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }

        public static async Task<List<TaskInfo>> GetUserTasksByBusinessKey()
        {
            var userTasks = await client.UserTasks
                .Query(new TaskQuery
                {
                    ProcessInstanceBusinessKey = processKey
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
                tasks.Add(LoadInstanceVariables(p));
            }
            await Task.WhenAll(tasks);
            return processes.First();
        }

        private static async Task LoadInstanceVariables(ProcessInfo process)
        {
            var list = await client.History.VariableInstances.Query(new HistoricVariableInstanceQuery { ProcessInstanceId = process.PID }).List();

            process.IdConference = list.Where(v => v.Name == "IdConference")
                                            .Select(v => Convert.ToInt32(v.Value))
                                            .First();
        }

        public static async Task SendScheduleToGuests(string taskId, int idConference)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["IdConference"] = VariableValue.FromObject(idConference);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task SendBookOfPapersToGuests(string taskId)
        {
            var variables = new Dictionary<string, VariableValue>();
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }
    }
}
