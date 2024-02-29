namespace Core;

public class Employee
{
    public long EmployeeId { get; }

    public string LastName { get; }

    public string CorporateEmail { get; }

    public Employee(long employeeId, string fullName, string corporateEmail)
    {
        EmployeeId = employeeId;
        LastName = fullName.Split(' ')[0];
        CorporateEmail = corporateEmail;
    }
}
