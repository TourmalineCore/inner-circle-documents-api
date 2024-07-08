using Application;
using Application.Services;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/documents")]
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
        var employees = await GetEmployeesFromEmployeeService();

        await _payslipsValidator.ValidateAsync(payslips, employees);

        try
        {
            await _client.SendMailingPayslips(payslips, employees);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception("Email sender service is not available");
        }
       
    }

    [HttpGet("getEmployees")]
    public async Task<EmployeesDto> GetEmployees()
    {
        var employees = await GetEmployeesFromEmployeeService();

        return new EmployeesDto
        {
            Employees = employees
                .Select(employee => new EmployeeDto
                {
                    LastName = employee.LastName
                })
                .ToList()
        };
    }

    public async Task<List<Employee>> GetEmployeesFromEmployeeService()
    {
        try
        {
            var tenantId = User.GetTenantId();
            return await _client.GetEmployeesAsync(tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception("Employees service is not available");
        }
    }
}