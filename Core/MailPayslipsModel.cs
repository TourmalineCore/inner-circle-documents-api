using Microsoft.AspNetCore.Http;

namespace Core;

public class MailPayslipsModel
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public IFormFile File { get; set; }

    public MailPayslipsModel(string to, string subject, string body, IFormFile file)
    {
        To = to;
        Subject = subject;
        Body = body;
        File = file;
    }
}

