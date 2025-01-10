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
    [HandlerTopics("SendReviewTopic")]
    public class SendReviewReminderTaskHandler : IExternalTaskHandler
    {
        private readonly IConferenceService _conferenceService;
        private readonly IUserConfService _userConfService;
        private readonly IPaperService _paperService;

        public SendReviewReminderTaskHandler(IConferenceService conferenceService, IUserConfService userConfService, IPaperService paperService)
        {
            _conferenceService = conferenceService;
            _userConfService = userConfService;
            _paperService = paperService;
        }

        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)  
        {
            var idPaper = 0;
            if (externalTask.TryGetVariable<IntegerVariable>("IdPaper", out var value1))
            {
                idPaper = value1.Value;
            }
            var reviewer = "";
            if (externalTask.TryGetVariable<StringVariable>("Reviewer", out var value2))
            {
                reviewer = value2.Value;
            }

            if (reviewer != "" && idPaper != 0)
            {
                await EmailService.Execute(reviewer, $"Reminder to create review", $"You have to create review of paper with IdPaper = {idPaper}.");
            }

            return new CompleteResult();
        }
    }
}
