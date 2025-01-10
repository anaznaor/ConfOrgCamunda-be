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
    [HandlerTopics("ReviewNotifyTopic")]
    public class ReviewNotifyTaskHandler : IExternalTaskHandler
    {
        private readonly IConferenceService _conferenceService;
        private readonly IUserConfService _userConfService;
        private readonly IPaperService _paperService;

        public ReviewNotifyTaskHandler(IConferenceService conferenceService, IUserConfService userConfService, IPaperService paperService)
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
            var guest = "";
            if (externalTask.TryGetVariable<StringVariable>("Guest", out var value2))
            {
                guest = value2.Value;
            }
            var decision = "";
            if (externalTask.TryGetVariable<StringVariable>("Decision", out var value3))
            {
                decision = value3.Value;
            }
            var idConf = 0;
            if (externalTask.TryGetVariable<IntegerVariable>("IdConference", out var value4))
            {
                idConf = value4.Value;
            }

            if (guest != "" && idPaper != 0 && idConf != 0)
            {
                var paper = _paperService.GetPaper(idPaper);
                if (paper != null && decision != "")
                {
                    var descriptions = paper.Reviews?.Select(r => r.Description).ToList();
                    var comment = "";
                    foreach (var d in descriptions)
                    {
                        comment += d + "\n";
                    }
                    if (decision == "Accepted")
                    {
                        await EmailService.Execute(guest, $"Paper accepted", $"Your paper for conference is ACCEPTED.\n" +
                            $"Id of paper is {idPaper}.\n" +
                            $"Please upload the final version on http://localhost:3000/{idConf}/guests/{guest}/paper/upload-full .\n"
                            + $"COMMENTS: {comment}");
                        paper.Decision = PaperDecision.Accepted;
                    }
                    else if (decision == "Correction")
                    {
                        await EmailService.Execute(guest, $"Paper needs correction", $"Your paper for conference needs CORRECTIONS.\n" +
                             $"Please upload the corrected version on http://localhost:3000/{idConf}/guests/{guest}/paper/upload-full .\n"
                             + $"COMMENTS: {comment}");
                        paper.Decision = PaperDecision.Correction;
                        if(paper.Reviews != null)
                        {
                            _paperService.DeleteReviews(paper.Reviews.ToList()); //brisanje starih review-eva
                        }
                        
                    }
                    else
                    {
                        await EmailService.Execute(guest, $"Paper is rejected", $"Your paper for conference is REJECTED.\n"
                             + $"COMMENTS: {comment}");
                        paper.Decision = PaperDecision.Rejected;
                    }
                    _paperService.SavePaper(paper);
                }
            }


            //var process = await CamundaReviewingUtil.GetProcessById(externalTask.ProcessInstanceId, true);
            //if (process != null) 
            //{

            //}

            return new CompleteResult();
        }
    }
}
