using Core;

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
            if (payslipsItem.File.Length == 0)
            {
                throw new Exception($"'{payslipsItem.File.FileName}' is empty");
            }

            else if (payslipsItem.File.FileName is null)
            {
                throw new Exception("Payslips name is null");
            }

            else if (!payslipsItem.File.FileName.Contains(payslipsItem.LastName))
            {
                throw new Exception($"Last name {payslipsItem.LastName} not contains in file name '{payslipsItem.File.FileName}'");
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
}

