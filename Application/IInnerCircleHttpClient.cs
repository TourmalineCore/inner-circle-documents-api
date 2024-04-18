using Core;

namespace Application;

public interface IInnerCircleHttpClient
{
    Task<List<Employee>> GetEmployeesAsync(long tenantId);
    Task SendMailingPayslips(List<PayslipsItem> payslips, List<Employee> employees);
}
