using Core;

namespace Application.Services;

public interface IPayslipsValidator
{
  public Task ValidateAsync(List<PayslipsItem> payslips, List<Employee> employees);
}
