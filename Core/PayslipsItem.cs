using Microsoft.AspNetCore.Http;

namespace Core;

public class PayslipsItem
{
  public required IFormFile File { get; set; }

  public required string LastName { get; set; }

  public PayslipsItem(string lastName, IFormFile file)
  {
    LastName = lastName;
    File = file;
  }

  public PayslipsItem() { }
}
