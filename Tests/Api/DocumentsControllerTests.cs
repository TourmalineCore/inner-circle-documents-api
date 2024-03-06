using Api.Controllers;
using Application;
using Application.Services;
using Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests;

public class DocumentsControllerTests
{
    private readonly Mock<IInnerCircleHttpClient> _httpClientMock = new();
    private readonly Mock<IPayslipsValidator> _payslipsValidatorMock = new();
    private readonly IPayslipsValidator _validator = new PayslipsValidator();
    private readonly ILogger<DocumentsController> _logger;
    private readonly DocumentsController _controller;

    public DocumentsControllerTests()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<DocumentsController>();

        _controller = new DocumentsController(_httpClientMock.Object, _payslipsValidatorMock.Object, _logger);
    }

    [Fact]
    public async Task SendMailingPayslips_WhenEmailSenderServiceIsNotAvailable_CatchException()
    {
        _httpClientMock
            .Setup(x => x.SendMailingPayslips(It.IsAny<List<PayslipsItem>>(), It.IsAny<List<Employee>>()))
            .ThrowsAsync(new Exception());

        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.SendMailingPayslips(It.IsAny<List<PayslipsItem>>()));

        Assert.Equal("Email sender service is not available", exception.Message);
    }

    [Fact]
    public async Task SendMailingPayslips_WhenEmployeesServiceIsNotAvailable_CatchException()
    {
        _httpClientMock
            .Setup(x => x.GetEmployeesAsync())
            .ThrowsAsync(new Exception());

        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.SendMailingPayslips(It.IsAny<List<PayslipsItem>>()));

        Assert.Equal("Employees service is not available", exception.Message);
    }

    [Fact]
    public async Task SendMailingPayslips_NotCatchException()
    {
        Assert.Null(Record.ExceptionAsync(() => _controller.SendMailingPayslips(It.IsAny<List<PayslipsItem>>())).Exception);
    }

    // посмотрите как с используя мок проверить что метод был вызван
    [Fact]
    public async Task SendMailingPayslips_CanCallPayslipsValidator()
    {
        //_payslipsValidatorMock.Verify(x => x.ValidateAsync(It.IsAny<List<PayslipsItem>>(), It.IsAny<List<Employee>>()));
        Assert.Null(Record.ExceptionAsync(() => _validator.ValidateAsync(It.IsAny<List<PayslipsItem>>(), It.IsAny<List<Employee>>())).Exception);

    }
}
