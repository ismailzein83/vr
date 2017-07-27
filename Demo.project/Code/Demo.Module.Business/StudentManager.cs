using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Demo.Module.Data;
using Vanrise.Entities;
using Vanrise.Common;
using Demo.Module.Entities.Room;
using Vanrise.Common.Business;
using Demo.Module.Entities.Building;

namespace Demo.Module.Business
{
    public class StudentManager
    {

        #region Public Methods
        public IDataRetrievalResult<StudentDetails> GetFilteredStudents(DataRetrievalInput<StudentQuery> input)
        {
            /*IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
            if (input.Query.Name == null || input.Query.Name=="undefined")
                input.Query.Name = "";
            var allStudents = studentDataManager.GetStudents(input.Query.Name);
            Func<Student, bool> filterExpression = (student) =>
            {
                return true;
            };*/
            var allStudents = GetCachedStudents();

            Func<Student, bool> filterExpression = (student) =>
            {
                if (input.Query.Name != null && !student.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.RoomIds != null && !input.Query.RoomIds.Contains(student.RoomId))
                    return false;
                if (input.Query.BuildingIds != null && !input.Query.BuildingIds.Contains(student.BuildingId))
                    return false;
                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allStudents.ToBigResult(input, filterExpression, StudentDetailMapper));
        }
        public Student GetStudentById(int studentId)
        {
            var allStudents = GetCachedStudents();
            return allStudents.GetRecord(studentId);
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
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = StudentDetailMapper(student);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
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
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = StudentDetailMapper(student);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public DeleteOperationOutput<StudentDetails> Delete(int Id)
        {
            IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
            DeleteOperationOutput<StudentDetails> deleteOperationOutput = new DeleteOperationOutput<StudentDetails>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            bool deleteActionSuccess = studentDataManager.Delete(Id);
            if (deleteActionSuccess)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        }
        #endregion

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return studentDataManager.AreStudentsUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, Student> GetCachedStudents()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCacheStudents", () =>
                {
                    IStudentDataManager studentDataManager = DemoModuleFactory.GetDataManager<IStudentDataManager>();
                    List<Student> students = studentDataManager.GetStudents();
                    return students.ToDictionary(student => student.StudentId, student => student);
                });
        }
        public StudentDetails StudentDetailMapper(Student student)
        {
            StudentDetails studentDetails = new StudentDetails();
            studentDetails.Entity = student;
            studentDetails.PaymentDescription = student.Payment.GetDescription();

            RoomManager roomManager = new RoomManager();
            Room room = roomManager.GetRoomById(student.RoomId);
            studentDetails.RoomName = room.Name;

            BuildingManager buildingManager = new BuildingManager();
            Building building = buildingManager.GetBuildingById(student.BuildingId);
            studentDetails.BuildingName = building.Name;

            return studentDetails;
        }
        public IEnumerable<PaymentConfig> GetPaymentTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<PaymentConfig>(PaymentConfig.EXTENSION_TYPE);
        }
    }

    public class PaymentConfigsManager
    {
        public IEnumerable<PaymentConfig> GetPaymentTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<PaymentConfig>(PaymentConfig.EXTENSION_TYPE);
        }
    }
}
