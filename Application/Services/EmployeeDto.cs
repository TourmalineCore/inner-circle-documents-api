namespace Application.Services;

public class EmployeesDto
{
  public required List<EmployeeDto> Employees { get; set; }
}

public class EmployeeDto
{
  public required string LastName { get; set; }
}
