using Core;

namespace Application;

public interface IInnerCircleHttpClient
{
    Task<List<Employee>> GetEmployeesAsync();
}
