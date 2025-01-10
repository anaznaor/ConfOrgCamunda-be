using Camunda.Worker;
using Camunda.Worker.Client;
using Camunda.Worker.Variables;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Util.TaskHandlers
{
    [HandlerTopics("CalcAvgGradeTopic")]
    public class CalcAvgGradeTaskHandler : IExternalTaskHandler
    {
        private readonly IConferenceService _conferenceService;
        private readonly IUserConfService _userConfService;
        private readonly IPaperService _paperService;

        public CalcAvgGradeTaskHandler(IConferenceService conferenceService, IUserConfService userConfService, IPaperService paperService)
        {
            _conferenceService = conferenceService;
            _userConfService = userConfService;
            _paperService = paperService;
        }

        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)  //async pa return new CompleteResult()
        {
            var idPaper = 0;
            if (externalTask.TryGetVariable<IntegerVariable>("IdPaper", out var value1))
            {
                idPaper = value1.Value;
            }

            var reviews = _paperService.GetPaper(idPaper).Reviews?.ToList();
            var sumGrade = 0;
            var completeResult = new CompleteResult();
            if (reviews != null && reviews.Any())
            {
                foreach (var r in reviews)
                {
                    sumGrade += r.Grade;
                }
                var avgGrade = sumGrade / reviews.Count;
                var variables = new Dictionary<string, VariableBase>
                {
                    ["AverageGrade"] = new IntegerVariable(avgGrade)
                };
                completeResult = new CompleteResult
                {
                    Variables = variables
                };
            }
            return completeResult;
        }
    }
}
