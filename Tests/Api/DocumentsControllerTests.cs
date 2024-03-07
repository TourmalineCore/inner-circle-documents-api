using Api.Controllers;
using Application;
using Application.Services;
using Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Api;

public class DocumentsControllerTests
{
    private readonly Mock<IInnerCircleHttpClient> _httpClientMock = new();
    private readonly Mock<IPayslipsValidator> _payslipsValidatorMock = new();
    private readonly DocumentsController _controller;
    private readonly List<PayslipsItem> _payslips = It.IsAny<List<PayslipsItem>>();
    private readonly List<Employee> _employees = It.IsAny<List<Employee>>();


    public DocumentsControllerTests()
    {
        var _logger = LoggerFactory
          .Create(_ => { })
          .CreateLogger<DocumentsController>();

        _controller = new DocumentsController(_httpClientMock.Object, _payslipsValidatorMock.Object, _logger);
    }

    [Fact]
    public async Task SendMailingPayslips_WhenEmailSenderServiceIsNotAvailable_ShouldThrowException()
    {
        _httpClientMock
            .Setup(x => x.SendMailingPayslips(_payslips, _employees))
            .ThrowsAsync(new Exception());

        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.SendMailingPayslips(_payslips));

        Assert.Equal("Email sender service is not available", exception.Message);
    }

    [Fact]
    public async Task SendMailingPayslips_WhenEmployeesServiceIsNotAvailable_ShouldThrowException()
    {
        _httpClientMock
            .Setup(x => x.GetEmployeesAsync())
            .ThrowsAsync(new Exception());

        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.SendMailingPayslips(_payslips));

        Assert.Equal("Employees service is not available", exception.Message);
    }

    [Fact]
    public async Task SendMailingPayslips_ShouldNotThrowException()
    {
        var exception = await Record.ExceptionAsync(() => _controller.SendMailingPayslips(_payslips));
        Assert.Null(exception);
    }

    [Fact]
    public async Task SendMailingPayslips_CanCallPayslipsValidator()
    {
        await _controller.SendMailingPayslips(_payslips);
        _payslipsValidatorMock.Verify(x => x.ValidateAsync(_payslips, _employees));
    }
}
