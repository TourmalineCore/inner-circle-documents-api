using Microsoft.AspNetCore.Http;

namespace Core;

public class MailPayslipsModel
{
    public string To { get; set; }
    public string Subject { get; set; }
    public IFormFile File { get; set; }

    public MailPayslipsModel(string to, string subject, IFormFile file)
    {
        To = to;
        Subject = subject;
        File = file;
    }
}

