using Application;
using Application.Services;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers;

[ApiController]
[Route("api")]
[Consumes("multipart/form-data")]
public class DocumentsController : Controller
{
    private readonly IInnerCircleHttpClient _client;
    private readonly IPayslipsValidator _payslipsValidator;
    private readonly ILogger<DocumentsController> _logger;


    public DocumentsController(IInnerCircleHttpClient client, IPayslipsValidator payslipsValidator, ILogger<DocumentsController> logger)
    {
        _client = client;
        _payslipsValidator = payslipsValidator;
        _logger = logger;
    }


    [HttpPost("sendMailingPayslips")]
    public async Task SendMailingPayslips([FromForm] List<PayslipsItem> payslips)
    {
        List<Employee> employees = new List<Employee>();

        try
        {
            employees = await _client.GetEmployeesAsync();
        }
        catch
        {
            _logger.LogError("Employees service is not available");
            throw new Exception("Employees service is not available");
        }

        await _payslipsValidator.ValidateAsync(payslips, employees);

        try
        {
            await _client.SendMailingPayslips(payslips, employees);
        }
        catch
        {
            _logger.LogError("Email sender service is not available");
            throw new Exception("Email sender service is not available");
        }
       
    }
}