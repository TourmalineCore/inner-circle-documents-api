using Core;

namespace Application;

public interface IInnerCircleHttpClient
{
    Task<List<Employee>> GetEmployeesAsync();
    Task SendMailingPayslips(List<PayslipsItem> payslips, List<Employee> employees);
}
