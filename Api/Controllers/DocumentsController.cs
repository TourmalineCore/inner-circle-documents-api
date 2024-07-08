using Application;
using Application.Services;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

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

    [RequiresPermission(UserClaimsProvider.CanManageDocuments)]
    [HttpPost("sendMailingPayslips")]
    public async Task SendMailingPayslips([FromForm] List<PayslipsItem> payslips)
    {
        List<Employee> employees = new List<Employee>();

        try
        {
            var tenantId = User.GetTenantId();
            employees = await _client.GetEmployeesAsync(tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception("Employees service is not available");
        }

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

    [RequiresPermission(UserClaimsProvider.CanManageDocuments)]
    [HttpGet("getEmployees")]
    public async Task<EmployeesDto> GetEmployees()
    {
        List<Employee> employees = new List<Employee>();

        try
        {
            var tenantId = User.GetTenantId();
            employees = await _client.GetEmployeesAsync(tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception("Employees service is not available");
        }

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
}