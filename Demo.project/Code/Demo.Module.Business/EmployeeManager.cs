using Demo.Module.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Demo.Module.Entities;
using Demo.Module.Entities.ProductInfo;
using Vanrise.Common.Business;

public class EmployeeManager
{
    SpecialityManager _specialityManager = new SpecialityManager();
    DesksizeManager _desksizeManager = new DesksizeManager();
    ColorManager _colorManager = new ColorManager();

    #region Public Methods
    public IDataRetrievalResult<EmployeeDetails> GetFilteredEmployees(DataRetrievalInput<EmployeeQuery> input)
    {
        var allEmployees = GetCachedEmployees();
        Func<Employee, bool> filterExpression = (employee) =>
        {
            if (input.Query.Name != null && !employee.Name.ToLower().Contains(input.Query.Name.ToLower()))
                return false;

            return true;
        };
        return DataRetrievalManager.Instance.ProcessResult(input, allEmployees.ToBigResult(input, filterExpression, EmployeeDetailMapper));

    }
  
    public InsertOperationOutput<EmployeeDetails> AddEmployee(Employee employee)
    {
        IEmployeeDataManager employeeDataManager = DemoModuleFactory.GetDataManager<IEmployeeDataManager>();
        InsertOperationOutput<EmployeeDetails> insertOperationOutput = new InsertOperationOutput<EmployeeDetails>();
        insertOperationOutput.Result = InsertOperationResult.Failed;
        insertOperationOutput.InsertedObject = null;
        int employeeId = -1;

        bool insertActionSuccess = employeeDataManager.Insert(employee, out employeeId);
        if (insertActionSuccess)
        {
            employee.EmployeeId = employeeId;
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            insertOperationOutput.InsertedObject = EmployeeDetailMapper(employee);
        }
        else
        {
            insertOperationOutput.Result = InsertOperationResult.SameExists;
        }
        return insertOperationOutput;
    }
    public Employee GetEmployeeById(int employeeId)
    {
        var allEmployees = GetCachedEmployees();
        return allEmployees.GetRecord(employeeId);
    }

    public UpdateOperationOutput<EmployeeDetails> UpdateEmployee(Employee employee)
    {
        IEmployeeDataManager employeeDataManager = DemoModuleFactory.GetDataManager<IEmployeeDataManager>();
        UpdateOperationOutput<EmployeeDetails> updateOperationOutput = new UpdateOperationOutput<EmployeeDetails>();
        updateOperationOutput.Result = UpdateOperationResult.Failed;
        updateOperationOutput.UpdatedObject = null;
        bool updateActionSuccess = employeeDataManager.Update(employee);
        if (updateActionSuccess)
        {
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            updateOperationOutput.UpdatedObject = EmployeeDetailMapper(employee);
        }
        else
        {
            updateOperationOutput.Result = UpdateOperationResult.SameExists;
        }
        return updateOperationOutput;
    }
    #endregion

    #region Private Classes
    private class CacheManager : Vanrise.Caching.BaseCacheManager
    {
        IEmployeeDataManager employeeDataManager = DemoModuleFactory.GetDataManager<IEmployeeDataManager>();
        object _updateHandle;
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return employeeDataManager.AreEmployeesUpdated(ref _updateHandle);
        }
    }
    #endregion

    #region Private Methods

    private Dictionary<int, Employee> GetCachedEmployees()
    {
        return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
           .GetOrCreateObject("GetCachedEmployees", () =>
           {
               IEmployeeDataManager employeeDataManager = DemoModuleFactory.GetDataManager<IEmployeeDataManager>();
               List<Employee> employees = employeeDataManager.GetEmployees();
               return employees.ToDictionary(employee => employee.EmployeeId, employee => employee);
           });
    }
    #endregion

    #region Mappers
    public EmployeeDetails EmployeeDetailMapper(Employee employee)
    {
        var employeeDetails = new EmployeeDetails
        {
            Name = employee.Name,
            EmployeeId = employee.EmployeeId,
            SpecialityName = _specialityManager.GetSpecialityName(employee.SpecialityId),
            DesksizeName = _desksizeManager.GetDesksizeName(employee.DesksizeId),
            ColorName = _colorManager.GetColorName(employee.ColorId),


        };
        if (employee.Settings != null)
        {
            if (employee.Settings.Work != null)
            {
                employeeDetails.WorkDescription = employee.Settings.Work.GetWorkDescription();
            }
              
              if (employee.Settings.Contacts != null)
              {
                  employeeDetails.Contacts = new List<Contact>();
                  for (var i = 0; i < employee.Settings.Contacts.Count; i++)
                  {
                     Contact contact=new Contact();
                      contact.Email = employee.Settings.Contacts[i].Email;
                      contact.PhoneNumber = employee.Settings.Contacts[i].PhoneNumber;
                      employeeDetails.Contacts.Add(contact);
                  }
              }
        }

          
        return employeeDetails;
    }
    #endregion

}