using Core;
using System.Globalization;
using System.Text;

namespace Application.Services;

public class PayslipsValidator : IPayslipsValidator
{
    public async Task ValidateAsync(List<PayslipsItem> payslips, List<Employee> employees)
    {
        if (payslips.Count == 0)
        {
            throw new Exception("Payslips list is empty");
        }


        foreach (var payslipsItem in payslips)
        {
            var fileLength = payslipsItem.File.Length;
            var fileName = EncodeToUTF8(payslipsItem.File.FileName);
            var lastNameFromFile = EncodeToUTF8(payslipsItem.LastName);

            if (fileLength == 0)
            {
                throw new Exception($"'{payslipsItem.File.FileName}' is empty");
            }

            else if (fileName is null)
            {
                throw new Exception("Payslips name is null");
            }

            else if (!fileName.Contains(lastNameFromFile))
            {
                throw new Exception($"Last name {lastNameFromFile} not contains in file name '{fileName}'");
            }
        }

        var employeesNames = employees.Select(x => x.LastName).ToList();
        var notExistentEmployees = new List<string>();

        foreach (var payslipsItem in payslips)
        {
            if (!employeesNames.Contains(payslipsItem.LastName))
            {
                notExistentEmployees.Add(payslipsItem.LastName);
            }
        }

        if (notExistentEmployees.Count != 0)
        {
            throw new Exception($"Employees with last Names {string.Join(", ", notExistentEmployees)} doesn't exist");
        }
    }

    private string EncodeToUTF8(string stringToEncode)
    {
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(stringToEncode);
        return Encoding.UTF8.GetString(utf8Bytes);

    }
}

