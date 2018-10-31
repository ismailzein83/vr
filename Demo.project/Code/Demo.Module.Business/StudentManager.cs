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

   public  class StudentManager
    {
       SchoolManager _schoolManager = new SchoolManager();
       
       DemoCountryManager _demoCountryManager = new DemoCountryManager();

       DemoCityManager _demoCityManager = new DemoCityManager();




       #region Public Methods
       public IDataRetrievalResult<StudentDetails> GetFilteredStudents(DataRetrievalInput<StudentQuery> input)
        {
            var allStudents = GetCachedStudents();
            Func<Student, bool> filterExpression = (student) =>
            {
                if (input.Query.Name != null && !student.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.Age.HasValue && !(student.Age==input.Query.Age))
                    return false;
                if (input.Query.SchoolIds != null && !input.Query.SchoolIds.Contains(student.SchoolId))
                    return false;
                if (input.Query.DemoCountryIds != null && !input.Query.DemoCountryIds.Contains(student.DemoCountryId))
                    return false;
                if (input.Query.DemoCityIds != null && !input.Query.DemoCityIds.Contains(student.DemoCityId))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allStudents.ToBigResult(input, filterExpression, StudentDetailMapper));

        }

       public IEnumerable<PaymentMethodConfig> GetPaymentMethodConfigs()
       {
           var extensionConfigurationManager = new ExtensionConfigurationManager();
           return extensionConfigurationManager.GetExtensionConfigurations<PaymentMethodConfig>(PaymentMethodConfig.EXTENSION_TYPE);
       }
       

       public InsertOperationOutput<StudentDetails> AddStudent(Student student)
        {
            IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
            InsertOperationOutput<StudentDetails> insertOperationOutput = new InsertOperationOutput<StudentDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int studentId = -1;

            bool insertActionSuccess = studentDataManager.Insert(student, out studentId);
            if (insertActionSuccess)
            {
                student.StudentId = studentId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = StudentDetailMapper(student);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
       public Student GetStudentById(int studentId)
        {
            var allStudents = GetCachedStudents();
            return allStudents.GetRecord(studentId);
        }

       public UpdateOperationOutput<StudentDetails> UpdateStudent(Student student)
        {
            IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
            UpdateOperationOutput<StudentDetails> updateOperationOutput = new UpdateOperationOutput<StudentDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = studentDataManager.Update(student);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = StudentDetailMapper(student);
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
            IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return studentDataManager.AreStudentsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, Student> GetCachedStudents()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedStudents", () =>
               {
                   IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
                   List<Student> students      = studentDataManager.GetStudents();
                   return students.ToDictionary(student => student.StudentId, student => student);
               });
        }
        #endregion

        #region Mappers
        public StudentDetails StudentDetailMapper(Student student)
        {
            var studentDetails = new StudentDetails
            {   
                Age=student.Age,
                Name = student.Name,
                StudentId = student.StudentId,
                SchoolName = _schoolManager.GetSchoolName(student.SchoolId),
                DemoCountryName = _demoCountryManager.GetDemoCountryName(student.DemoCountryId),
                DemoCityName = _demoCityManager.GetDemoCityName(student.DemoCityId),

            };
            if (student.Settings != null && student.Settings.PaymentMethod != null)
            {

                studentDetails.PaymentMethodDescription = student.Settings.PaymentMethod.GetPaymentMethodDescription();
            }

            return studentDetails;
        }
        #endregion 

    }
