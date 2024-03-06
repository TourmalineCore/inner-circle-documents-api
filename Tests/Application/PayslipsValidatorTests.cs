using Application.Services;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Application
{
    public class PayslipsValidatorTests
    {
        private readonly IPayslipsValidator _validator = new PayslipsValidator();
        private readonly List<Employee> _emptyEmployeeList = new();

        [Fact]
        public async Task PayslipsValidator_WhenPayslipsListIsEmpty_CatchException()
        {
            var payslipsItems = new List<PayslipsItem>();

            var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, _emptyEmployeeList));

            Assert.Equal("Payslips list is empty", exception.Message);
        }

        [Fact]
        public async Task PayslipsValidator_WhenSomeFileIsEmpty_CatchException()
        {
            var content = "";
            var fileName = "Расчетный листок Иванов за ноябрь 2023 года.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            IFormFile fileMock = new FormFile(stream, 0, stream.Length, "file", fileName);

            var payslipsItems = new List<PayslipsItem> {
                new PayslipsItem("", fileMock)
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, _emptyEmployeeList));

            Assert.Equal($"'{fileName}' is empty", exception.Message);
        }


        [Fact]
        public async Task PayslipsValidator_WhenSomeFileNameIsNull_CatchException()
        {
            var content = "Something";
            string fileName = null;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            IFormFile fileMock = new FormFile(stream, 0, stream.Length, "file", fileName);

            var payslipsItems = new List<PayslipsItem> {
                new PayslipsItem("", fileMock)
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, _emptyEmployeeList));

            Assert.Equal("Payslips name is null", exception.Message);
        }

        [Fact]
        public async Task PayslipsValidator_WhenLastNameNotContainsInFileName_CatchException()
        {
            var lastNameMock = "Петров";

            var content = "Something";
            var fileName = "Расчетный листок Иванов за ноябрь 2023 года.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            IFormFile fileMock = new FormFile(stream, 0, stream.Length, "file", fileName);

            var payslipsItems = new List<PayslipsItem> {
                new PayslipsItem(lastNameMock, fileMock)
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, _emptyEmployeeList));

            Assert.Equal($"Last name {lastNameMock} not contains in file name '{fileName}'", exception.Message);
        }

        [Fact]
        public async Task PayslipsValidator_WhenEmployesWithLastNamesDoesNotExist_CatchException()
        {
            var lastNameMock = "Иванов";

            var content = "Something";
            var fileName = "Расчетный листок Иванов за ноябрь 2023 года.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            IFormFile fileMock = new FormFile(stream, 0, stream.Length, "file", fileName);

            var payslipsItems = new List<PayslipsItem> {
                new PayslipsItem(lastNameMock, fileMock)
            };

            var employees = new List<Employee>
            {
                new Employee(21, "Петров Иван Иванович", "test@mail.com")
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => _validator.ValidateAsync(payslipsItems, employees));

            Assert.Equal($"Employees with last Names {string.Join(", ", lastNameMock)} doesn't exist", exception.Message);
        }

        [Fact]
        public async Task PayslipsValidator_NotCatchException()
        {
            var lastNameMock = "Иванов";

            var content = "Something";
            var fileName = "Расчетный листок Иванов за ноябрь 2023 года.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            IFormFile fileMock = new FormFile(stream, 0, stream.Length, "file", fileName);

            var payslipsItems = new List<PayslipsItem> {
                new PayslipsItem(lastNameMock, fileMock)
            };

            var employees = new List<Employee>
            {
                new Employee(21, "Иванов Иван Иванович", "test@mail.com")
            };

            Assert.Null(Record.Exception(() => _validator.ValidateAsync(payslipsItems, employees).Exception));
        }
    }
}

