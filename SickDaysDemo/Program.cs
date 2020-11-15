using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Console;
using Microsoft.Extensions.DependencyInjection;

ServiceProvider serviceProvider;

ConfigureDependencyInjection();

ProcessRecords(serviceProvider.GetService<IEmployeeService>());

DisposeDependencyInjection();

void ProcessRecords(IEmployeeService employeeService)
{

    var employeeList = employeeService.LoadEmployeeList();

    WriteLine($"Employee List Count: {employeeList.Count}");

    var filteredEmployeeList = employeeService.GetEmployeesWithAtLeastNSickDays(employeeList, 4);

    employeeService.WriteRecordsToConsole(filteredEmployeeList);
}

void ConfigureDependencyInjection()
{

    var collection = new ServiceCollection();
    collection.AddScoped<IEmployeeService, EmployeeService>();
    serviceProvider = collection.BuildServiceProvider();

}

void DisposeDependencyInjection()
{

    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }

}


//TODO: Move To EmployeeService file - leaving here for demo
public interface IEmployeeService
{
    List<Employee> GetEmployeesWithAtLeastNSickDays(IEnumerable<Employee> employees, int number);

    List<Employee> GetEmployeesWithAtLeastNSickDaysOldIterator(List<Employee> employees, int number);

    public List<Employee> LoadEmployeeList();

    public string WriteRecordsToConsole<T>(IEnumerable<T> list);

}
public class EmployeeService : IEmployeeService
{
    public EmployeeService()
    {

    }

    public List<Employee> GetEmployeesWithAtLeastNSickDays(IEnumerable<Employee> employees, int number)
    {
        return employees.Where(e => e.SickDays >= number).ToList();
    }

    public List<Employee> GetEmployeesWithAtLeastNSickDaysOldIterator(List<Employee> employees, int number)
    {
        var result = new List<Employee>();

        foreach (var e in employees)
            if (e.SickDays >= number)
                result.Add(e);

        return result;
    }

    public List<Employee> LoadEmployeeList()
    {

        var employeeList = new List<Employee> {
            new Employee { FirstName = "chris", SickDays = 5, Salary = 15.00M },
            new Employee { FirstName = "joe", SickDays = 3, Salary = 17.00M },
            new Employee { FirstName = "harryNeverSick", SickDays = 100, Salary = 10.00M }
        };

        return employeeList;

    }

    public string WriteRecordsToConsole<T>(IEnumerable<T> list)
    {
        if (!list.Any())
        {
            WriteLine($"Count of {typeof(T)}: 0");
        }

        WriteLine($"Count of {typeof(T)}: { list.Count() }");

        var sb = new StringBuilder();

        foreach (var record in list)
        {
            sb.AppendLine($"{record}");
        }

        WriteLine(sb.ToString());

        return sb.ToString();
    }
}

public record Employee : Person
{

    public int SickDays { get; init; }
    public decimal Salary { get; init; }

}

public record Person
{
    
    public string FirstName { get; init; }

}

//TODO: Move to test file
[ExcludeFromCodeCoverage]
[TestClass]
public class TestProgram
{
    private readonly EmployeeService _employeeContainer = new();
    private const int TestSickDays = 4;
    private List<Employee> expected = new();
    private readonly List<Employee> fullListOfEmployees = new();

    [TestInitialize]
    public void Init()
    {

        Employee EmpWith3Days = new() { FirstName = "chris", SickDays = 3, Salary = 15M };
        Employee EmpWith5Days = new() { FirstName = "sean", SickDays = 5 };
        Employee EmpWith100Days = new() { FirstName = "NeverSick", SickDays = 100 };

        fullListOfEmployees.Add(EmpWith3Days);
        fullListOfEmployees.Add(EmpWith5Days);
        fullListOfEmployees.Add(EmpWith100Days);

        expected = new List<Employee> { EmpWith5Days, EmpWith100Days };

    }

    [TestMethod]
    public void GivenListOfEmployees_WhenWriteRecordsToConsole_ThenRecordsAreWritten()
    {
        var result = _employeeContainer.WriteRecordsToConsole(fullListOfEmployees);

        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.IsTrue(result.Contains("FirstName"));
    }

    [TestMethod]
    public void GivenASickDayCount_WhenEmployeesHaveAtLeastThatCount_ReturnEmployeeList()
    {
        var actual = _employeeContainer.GetEmployeesWithAtLeastNSickDays(fullListOfEmployees, TestSickDays);

        Assert.AreNotEqual(actual, expected); //ensure they are different value object
        Assert.ReferenceEquals(actual, expected); //ensure objects are reference objects
        Assert.AreEqual(actual.GetType(), typeof(List<Employee>));
        Assert.IsTrue(expected.SequenceEqual(actual));

    }

    [TestMethod]
    public void GivenASickDayCount_WhenEmployeesHaveAtLeastThatCountUsingOldIteratorMethod_ReturnEmployeeList()
    {
        var actual = _employeeContainer.GetEmployeesWithAtLeastNSickDaysOldIterator(fullListOfEmployees, TestSickDays);

        Assert.AreNotEqual(actual, expected); //ensure they are different value object
        Assert.ReferenceEquals(actual, expected); //ensure objects are reference objects
        Assert.AreEqual(actual.GetType(), typeof(List<Employee>));
        Assert.IsTrue(expected.SequenceEqual(actual));

    }

    [TestMethod]
    public void EnsureLoadEmployeesHasData()
    {
        var results = _employeeContainer.LoadEmployeeList();
        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);

    }

    [TestMethod]
    public void EnsureEmployeeContainerCanBeCreated()
    {
        Assert.IsNotNull(_employeeContainer);
    }

    [TestMethod]
    public void EnsureEmployeeContainerReturnsAnEmployeeCount()
    {
        Assert.IsTrue(fullListOfEmployees.Count > 0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException), "A null value was inappropriately allowed.")]
    public void GivenNullList_WriteRecordsToConsole_ExceptionThrown()
    {
        /*
        //Null Defense Gate
        if (list is null)
        {
            throw new ArgumentNullException($"expected a IEnumerable<{typeof(T)}> but null was received");
        }
        */
        //_employeeContainer.WriteRecordsToConsole(null); //with c# 9.0 this is a private method call

        throw new ArgumentNullException($"expected an arguement but null was received"); //just to show how this is done
    }

    [TestMethod]
    public void GivenEmptyList_WriteRecordsToConsole_ThenNoExceptionThrown()
    {
        var test = new List<Employee>();

        try
        {
            _employeeContainer.WriteRecordsToConsole(test); //with c# 9.0 this is a private method call
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }

    }

}
