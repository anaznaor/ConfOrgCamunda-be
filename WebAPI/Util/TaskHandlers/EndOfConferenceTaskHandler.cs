using Camunda.Worker;
using Camunda.Worker.Variables;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Util.TaskHandlers
{
    [HandlerTopics("CheckEndOfConference")]
    public class EndOfConferenceTaskHandler : IExternalTaskHandler
    {
        private readonly IConferenceService _conferenceService;

        public EndOfConferenceTaskHandler(IConferenceService conferenceService)
        {
            _conferenceService = conferenceService;
        }

        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            var idConf = 0;
            if (externalTask.TryGetVariable<IntegerVariable>("IdConference", out var value1))
            {
                idConf = value1.Value;
            }

            Conference conference = _conferenceService.GetConference(idConf);
            if (conference != null)
            {
                var end = conference.ConferenceDate;
                while (DateTime.Now <= end) { } //waiting


                var completeResult = new CompleteResult();
                var variables = new Dictionary<string, VariableBase>
                {
                    ["IdConference"] = new IntegerVariable(idConf)
                };
                completeResult = new CompleteResult
                {
                    Variables = variables
                };
                return completeResult;
            }
            return new CompleteResult();
        }
    }
}
