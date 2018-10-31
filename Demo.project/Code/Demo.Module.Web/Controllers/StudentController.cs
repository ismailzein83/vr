using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Student")]
    [JSONWithTypeAttribute]
    public class StudentController : BaseAPIController
    {
        StudentManager studentManager = new StudentManager();
        [HttpPost]
        [Route("GetFilteredStudents")]
        public object GetFilteredStudents(DataRetrievalInput<StudentQuery> input)
        {
            return GetWebResponse(input, studentManager.GetFilteredStudents(input));
        }

        [HttpGet]
        [Route("GetStudentById")]
        public Student GetStudentById(int studentId)
        {
            return studentManager.GetStudentById(studentId);
        }

        [HttpPost]
        [Route("UpdateStudent")]
        public UpdateOperationOutput<StudentDetails> UpdateStudent(Student student)
        {
            return studentManager.UpdateStudent(student);
        }

        [HttpPost]
        [Route("AddStudent")]
        public InsertOperationOutput<StudentDetails> AddStudent(Student student)
        {
            return studentManager.AddStudent(student);
        }
        [HttpGet]

        [Route("GetPaymentMethodConfigs")]
        public IEnumerable<PaymentMethodConfig> GetPaymentMethodConfigs()
        {
            return studentManager.GetPaymentMethodConfigs();
        }

    }
}