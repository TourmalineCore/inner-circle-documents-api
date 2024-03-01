using Application.Services.Options;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
        var matches = payslips.Join(employees,
                item1 => item1.LastName,
                item2 => item2.LastName,
                (item1, item2) => new { File = item1.File, CorporateEmail = item2.CorporateEmail });

        Console.WriteLine(matches);
    }
}