using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Student")]
    [JSONWithTypeAttribute]
    public class Demo_Module_StudentController : BaseAPIController
    {
        StudentManager studentManager = new StudentManager();
        [HttpPost]
        [Route("GetFilteredStudents")]
        public object GetFilteredStudents(DataRetrievalInput<StudentQuery> input)
        {
            StudentManager studentManager = new StudentManager();
            return GetWebResponse(input,studentManager.GetFilteredStudents(input));
        }

        [HttpGet]
        [Route("GetStudentById")]
        public Student GetStudentById(int studentId)
        {
            return studentManager.GetStudentById(studentId);
        }

        [HttpPost]
        [Route("AddStudent")]
        public InsertOperationOutput<StudentDetails> AddStudent(Student student)
        {
            return studentManager.AddStudent(student);
        }

        [HttpPost]
        [Route("UpdateStudent")]
        public UpdateOperationOutput<StudentDetails> UpdateStudent(Student student)
        {
            return studentManager.UpdateStudent(student);
        }

        [HttpGet]
        [Route("DeleteStudent")]
        public DeleteOperationOutput<StudentDetails> DeleteStudent(int studentId)
        {
            return studentManager.Delete(studentId);
        }
    }
}