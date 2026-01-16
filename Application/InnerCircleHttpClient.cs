using Application.Services.Options;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace Application;

public class InnerCircleHttpClient : IInnerCircleHttpClient
{
  private readonly HttpClient _client;
  private readonly InnerCircleServiceUrls _urls;
  private readonly AuthenticationOptions _authOptions;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public InnerCircleHttpClient(
    IOptions<InnerCircleServiceUrls> urls,
    IOptions<AuthenticationOptions> authOptions,
    IHttpContextAccessor httpContextAccessor
  )
  {
    _client = new HttpClient();
    _urls = urls.Value;
    _authOptions = authOptions.Value;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<List<Employee>> GetEmployeesAsync()
  {
    var link = $"{_urls.EmployeesServiceUrl}/internal/get-employees";

    var headerName = _authOptions.IsDebugTokenEnabled
      ? "X-DEBUG-TOKEN"
      : "Authorization";

    var token = _httpContextAccessor
      .HttpContext!
      .Request
      .Headers[headerName]
      .ToString();

    _client.DefaultRequestHeaders.Add(headerName, token);

    var response = await _client.GetStringAsync(link);

    return JsonConvert.DeserializeObject<List<Employee>>(response);
  }

  public async Task SendMailingPayslips(List<PayslipsItem> payslips, List<Employee> employees)
  {
    var mailsData = payslips.Join(
      employees,
      payslip => payslip.LastName,
      employee => employee.LastName,
      (payslip, employee) => new MailPayslipsModel
      (
        employee.CorporateEmail,
        payslip.File.FileName.Substring(0, payslip.File.FileName.Length - 4),
        " ",
        payslip.File
      )
    );

    var link = $"{_urls.EmailSenderServiceUrl}/mail/send-document";

    foreach (var mailData in mailsData)
    {
      await using var stream = mailData.File.OpenReadStream();
      using var request = new HttpRequestMessage(HttpMethod.Post, link);
      using var content = new MultipartFormDataContent
      {
        {
          new StringContent(mailData.To),
          "To"
        },
        {
          new StringContent(mailData.Subject),
          "Subject"
        },
        {
          new StringContent(mailData.Body),
          "Body"
        },
        {
          new StreamContent(stream),
          "File",
          mailData.File.FileName
        },
      };

      request.Content = content;

      await _client.SendAsync(request);
    }
  }
}
