(function (appControllers) {
    "use strict";
    studentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function studentAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Student";

        function GetFilteredStudents(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredStudents"), input);
        }

        function GetStudentById(studentId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetStudentById"),
                {
                    studentId: studentId
                });
        }

        function UpdateStudent(student) {
            console.log(student);
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateStudent"), student);
        }

        function AddStudent(student) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddStudent"), student);
        };
        function GetPaymentMethodConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetPaymentMethodConfigs"));
        }

        return {
            GetFilteredStudents: GetFilteredStudents,
            GetStudentById: GetStudentById,
            UpdateStudent: UpdateStudent,
            AddStudent: AddStudent,
            GetPaymentMethodConfigs: GetPaymentMethodConfigs,
        };
    };
    appControllers.service("Demo_Module_StudentAPIService", studentAPIService);

})(appControllers);