using Microsoft.AspNetCore.Http;

namespace Core;

public class PayslipsItem
{
    public IFormFile File { get; set; }
    public string LastName { get; set; }

    public PayslipsItem(string lastName, IFormFile file)
    {
        LastName = lastName;
        File = file;
    }
}


