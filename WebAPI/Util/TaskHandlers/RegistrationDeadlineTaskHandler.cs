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
    [HandlerTopics("RegistrationDeadlineTopic")]
    public class RegistrationDeadlineTaskHandler : IExternalTaskHandler
    {
        private readonly IConferenceService _conferenceService;
        private readonly IUserConfService _userConfService;

        public RegistrationDeadlineTaskHandler(IConferenceService conferenceService, IUserConfService userConfService)
        {
            _conferenceService = conferenceService;
            _userConfService = userConfService;
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
                var deadline = conference.LastDateOfRegistration;
                if (externalTask.ProcessDefinitionKey == "ConferenceOrganisationProcess")
                {
                    if (DateTime.Now > deadline) 
                    {
                        await CamundaScheduleUtil.StartSchedulingProcessWithMsg(idConf);
                        await CamundaConfOrgUtil.RegistrationDeadlineMsg();
                    }
                }
                else
                {
                    if (DateTime.Now <= deadline)
                    {
                        var variables = new Dictionary<string, VariableBase>
                        {
                            ["EndOfRegistration"] = new IntegerVariable(0)
                        };
                        return new CompleteResult
                        {
                            Variables = variables
                        };
                    }
                    else
                    {
                        var variables = new Dictionary<string, VariableBase>
                        {

                            ["EndOfRegistration"] = new IntegerVariable(1)
                        };
                        return new CompleteResult
                        {
                            Variables = variables
                        };
                    }
                }  
            }

            return new CompleteResult();
        }
    }
}
