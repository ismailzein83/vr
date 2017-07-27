
(function (appControllers) {

    'use strict';

    StudentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function StudentAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = 'Student';

        function GetStudentById(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetStudentById'),
                { studentId: Id }
                );
        }

        function GetFilteredStudents(input) {
           return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredStudents"), input);
        }

        function AddStudent(student) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddStudent"), student);
        }
        function UpdateStudent(student) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateStudent"), student);
        }
        function DeleteStudent(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteStudent'), {
                studentId: Id
            });
        }

        return ({
            GetFilteredStudents: GetFilteredStudents,
            UpdateStudent: UpdateStudent,
            AddStudent: AddStudent,
            GetStudentById: GetStudentById,
            DeleteStudent: DeleteStudent
        });
    }


    appControllers.service('Demo_Module_StudentAPIService', StudentAPIService);
})(appControllers);