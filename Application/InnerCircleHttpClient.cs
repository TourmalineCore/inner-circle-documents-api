using System.Net.Http.Json;
using System.Web;
using Application.Services.Options;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application;

public class InnerCircleHttpClient : IInnerCircleHttpClient
{
    private readonly HttpClient _client;
    private readonly InnerCircleServiceUrls _urls;

    public InnerCircleHttpClient(IOptions<InnerCircleServiceUrls> urls)
    {
        _client = new HttpClient();
        _urls = urls.Value;
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        var link = $"{_urls.SalaryServiceUrl}salary/employees/all";
        var response = await _client.GetStringAsync(link);

        return JsonConvert.DeserializeObject<List<Employee>>(response);
    }


    public async Task SendMailingPayslips(List<PayslipsItem> payslips, List<Employee> employees)
    {
        var mailsData = payslips.Join(employees,
            payslip => payslip.LastName,
            employee => employee.LastName,
            (payslip, employee) => new MailPayslipsModel
            (
                employee.CorporateEmail,
                payslip.File.FileName.Substring(0, payslip.File.FileName.Length - 4),
                " ",
                payslip.File
            ));
       
        var link = $"{_urls.EmailSenderServiceUrl}/send-document";

        foreach (var mailData in mailsData)
        {
            await using var stream = mailData.File.OpenReadStream();
            using var request = new HttpRequestMessage(HttpMethod.Post, link);
            using var content = new MultipartFormDataContent
            {
                { new StringContent(mailData.To), "To" },
                { new StringContent(mailData.Subject), "Subject" },
                { new StringContent(mailData.Body), "Body" },
                { new StreamContent(stream), "File", mailData.File.FileName },
            };

            request.Content = content;

            await _client.SendAsync(request);
        }
    }
}