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
        var link = $"{_urls.SalaryServiceUrl}/employees/all";
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
                payslip.File
            ));

        var link = $"{_urls.EmailSenderServiceUrl}/send-document";

        foreach(var mailData in mailsData)
        {
            await _client.PostAsJsonAsync(link,
            new
            {
                To = mailData.To,
                Subject = mailData.Subject,
                File = mailData.File
            });
        }
    }
}