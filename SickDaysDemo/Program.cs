using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SickDaysDemo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var empObj = new EmployeeContainer();
            Console.WriteLine(empObj.GetEmployeesWithAtLeastNSickDays(empObj.Employees, 4).Count);
        }
    }

    public class EmployeeContainer
    {
        public readonly List<Employee> Employees = new List<Employee>();
        public Employee EmpWith3Days;
        public Employee EmpWith5Days;
        public Employee EmpWith100Days;

        public EmployeeContainer()
        {
            LoadEmployees();
        }

        public List<Employee> GetEmployeesWithAtLeastNSickDays(List<Employee> employees, int number)
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

        public void LoadEmployees()
        {
            //EmpWith3Days = new EmployeeModel();
            //EmpWith5Days = new EmployeeModel();
            //EmpWith100Days = new EmployeeModel();

            EmpWith3Days = HydrateEmployee("chris", 3);
            EmpWith5Days = HydrateEmployee("sean", 5);
            EmpWith100Days = HydrateEmployee("NeverSick", 100);

            Employees.Add(EmpWith3Days);
            Employees.Add(EmpWith5Days);
            Employees.Add(EmpWith100Days);
        }

        private Employee HydrateEmployee(string name, int sickDays)
        {
            return new Employee { FirstName = name, SickDays = sickDays};
        }
    }

    public class Employee
    {
        public string FirstName { get; set; }
        public int SickDays { get; set; }


    }


    [TestClass]
    public class TestProgram
    {
        private readonly EmployeeContainer _employee = new EmployeeContainer();

        [TestMethod]
        public void GivenASickDayCount_WhenEmployeesHaveAtLeastThatCount_ReturnEmployeeList()
        {
            var expected = new List<Employee> {_employee.EmpWith5Days, _employee.EmpWith100Days};

            const int testSickDays = 4;

            var actual = _employee.GetEmployeesWithAtLeastNSickDays(_employee.Employees, testSickDays);

            Assert.AreEqual(actual.GetType(), typeof(List<Employee>));
            // Assert.IsTrue(expected.SequenceEqual(actual));
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenASickDayCount_WhenEmployeesHaveAtLeastThatCountUsingOldIteratorMethod_ReturnEmployeeList()
        {
            var expected = new List<Employee> {_employee.EmpWith5Days, _employee.EmpWith100Days};

            const int testSickDays = 4;

            var actual = _employee.GetEmployeesWithAtLeastNSickDaysOldIterator(_employee.Employees, testSickDays);

            Assert.AreEqual(actual.GetType(), typeof(List<Employee>));
            // Assert.IsTrue(expected.SequenceEqual(actual));
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EnsureEmployeeContainerCanBeCreated()
        {
            Assert.IsTrue(_employee.Employees.Count >0);
        }
        [TestMethod]
        public void EnsureEmployeeContainerReturnsAnEmployeeCount()
        {
            Assert.IsTrue(_employee.Employees.Count > 0);
        }
    }
}