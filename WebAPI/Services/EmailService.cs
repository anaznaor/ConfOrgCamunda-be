using System.Net.Mail;
using System.Net;
using System.Text;

namespace WebAPI.Services
{
    public class EmailService
    {
        public static async Task Execute(string UserEmail, string Subject, string Body, byte[]? attachmentBytes = null, string? attachmentFilename = "")
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Timeout = 100000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;

                    // Note: Replace with your actual email and app-specific password
                    client.Credentials = new NetworkCredential("znaor.a@gmail.com", "skyq ciut deyf pihq");

                    using (var message = new MailMessage("znaor.a@gmail.com", UserEmail))
                    {
                        message.Subject = Subject;
                        message.Body = Body;
                        message.IsBodyHtml = true;
                        message.BodyEncoding = Encoding.UTF8;

                        if (attachmentBytes != null && attachmentBytes.Length > 0 && attachmentFilename != null)
                        {
                            using (var attachmentStream = new MemoryStream(attachmentBytes))
                            {
                                var attachment = new Attachment(attachmentStream, attachmentFilename, "application/pdf");
                                message.Attachments.Add(attachment);
                            }
                        }

                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                Console.Error.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}
