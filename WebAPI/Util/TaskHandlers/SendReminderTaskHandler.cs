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
    [HandlerTopics("SendReminderTopic")]
    public class SendReminderTaskHandler : IExternalTaskHandler
    {
        private readonly IConferenceService _conferenceService;
        private readonly IUserConfService _userConfService;

        public SendReminderTaskHandler(IConferenceService conferenceService, IUserConfService userConfService)
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

            var process = await CamundaConfOrgUtil.GetProcessWithLists(externalTask.ProcessDefinitionKey);
            while (process == null) { }
            List<string> progCommsEmails = process.ProgComms != null ? process.ProgComms.ToList() : null;
            List<string> guestLectsEmails = process.GuestLects != null ? process.GuestLects.ToList() : null;

            Conference conference = _conferenceService.GetConference(idConf);

            List<int> progCommUsers = new List<int>();
            if (progCommsEmails != null)
            {
                foreach (var p in progCommsEmails)
                {
                    var user = _userConfService.GetUser(p);
                    progCommUsers.Add(user.Id);
                }
            }
            List<int> guestLectUsers = new List<int>();
            if (guestLectsEmails != null)
            {
                foreach (var p in guestLectsEmails)
                {
                    var user = _userConfService.GetUser(p);
                    guestLectUsers.Add(user.Id);
                }
            }

            if (progCommsEmails != null)
            {
                if (conference.ProgramCommittee.Count != progCommsEmails.Count)
                {
                    foreach (var pc in conference.ProgramCommittee)
                    {
                        if (progCommUsers.Contains(pc.IdUser))
                        {
                            var user = _userConfService.GetUser(pc.IdUser);
                            progCommsEmails.Remove(user.Email);
                        }
                    }

                    if (progCommsEmails.Count > 0)
                    {
                        foreach (var prog in progCommsEmails)
                        {
                            await EmailService.Execute(prog, "Reminder to answer on conference invitation", $"http://localhost:3000/{idConf}/program-committee/{prog}");
                        }
                    }
                }
            }

            if (guestLectsEmails != null)
            {
                if (conference.GuestLecturers.Count != guestLectsEmails.Count)
                {
                    foreach (var gl in conference.GuestLecturers)
                    {
                        if (guestLectUsers.Contains(gl.IdUser))
                        {
                            var user = _userConfService.GetUser(gl.IdUser);
                            guestLectsEmails.Remove(user.Email);
                        }

                    }
                }
                if (guestLectsEmails.Count > 0)
                {
                    foreach (var gl in guestLectsEmails)
                    {
                        await EmailService.Execute(gl, "Reminder to answer on conference invitation", $"http://localhost:3000/{idConf}/guest-lecturer/{gl}");
                    }
                }
            }

            var completeResult = new CompleteResult();
            return completeResult;
        }
    }
}
