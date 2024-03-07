using Application.Services;
using Core;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Tests.Application;

public class PayslipsValidatorTests
{
    private readonly IPayslipsValidator _validator = new PayslipsValidator();
    private readonly List<Employee> _emptyEmployeeList = new();

    [Fact]
    public async Task PayslipsListMustNotBeEmpty()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(new List<PayslipsItem>(), _emptyEmployeeList));
        Assert.Equal("Payslips list is empty", exception.Message);
    }

    [Fact]
    public async Task PayslipFilesMustNotBeEmpty()
    {
        var formFile = await GetFormFileAsync("Расчетный листок Иванов за ноябрь 2023 года.pdf", "");

        var payslipsItems = new List<PayslipsItem> {
            new PayslipsItem("", formFile),
        };

        var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, _emptyEmployeeList));

        Assert.Equal($"'{formFile.FileName}' is empty", exception.Message);
    }


    [Fact]
    public async Task PayslipFileNameMustNotBeNull()
    {
        var formFile = await GetFormFileAsync(null, "Something");

        var payslipsItems = new List<PayslipsItem> {
            new PayslipsItem("", formFile),
        };

        var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, _emptyEmployeeList));

        Assert.Equal("Payslips name is null", exception.Message);
    }

    [Fact]
    public async Task LastNameMustBeIncludedInPayslipFileName()
    {
        const string lastName = "Петров";

        var formFile = await GetFormFileAsync("Расчетный листок Иванов за ноябрь 2023 года.pdf", "Something");

        var payslipsItems = new List<PayslipsItem> {
            new PayslipsItem(lastName, formFile),
        };

        var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, _emptyEmployeeList));

        Assert.Equal($"Last name {lastName} not contains in file name '{formFile.FileName}'", exception.Message);
    }

    [Fact]
    public async Task EmployesWithLastNamesShouldBeExist()
    {
        const string lastName = "Иванов";

        var formFile = await GetFormFileAsync("Расчетный листок Иванов за ноябрь 2023 года.pdf", "Something");

        var payslipsItems = new List<PayslipsItem> {
            new PayslipsItem(lastName, formFile),
        };

        var employees = new List<Employee>
        {
            new Employee(21, "Петров Иван Иванович", "test@mail.com")
        };

        var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, employees));

        Assert.Equal($"Employees with last Names {string.Join(", ", lastName)} doesn't exist", exception.Message);
    }

    [Fact]
    public async Task NoException()
    {
        const string lastName = "Иванов";

        var formFile = await GetFormFileAsync("Расчетный листок Иванов за ноябрь 2023 года.pdf", "Something");

        var payslipsItems = new List<PayslipsItem> {
            new PayslipsItem(lastName, formFile),
        };

        var employees = new List<Employee>
        {
            new Employee(21, "Иванов Иван Иванович", "test@mail.com")
        };

        Assert.Null(Record.Exception(() => _validator.ValidateAsync(payslipsItems, employees).Exception));
    }

    private static async Task<IFormFile> GetFormFileAsync(string fileName, string content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        stream.Position = 0;

        return new FormFile(stream, 0, stream.Length, "file", fileName);
    }
}

