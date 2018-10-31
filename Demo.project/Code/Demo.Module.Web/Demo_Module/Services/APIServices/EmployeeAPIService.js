(function (appControllers) {
    "use strict";
    employeeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function employeeAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Employee";

        function GetFilteredEmployees(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredEmployees"), input);
        }

        function GetEmployeeById(employeeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetEmployeeById"),
                {
                    employeeId: employeeId
                });
        }

        function UpdateEmployee(employee) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateEmployee"), employee);
        }

        function AddEmployee(employee) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddEmployee"), employee);
        };

        function GetWorkConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetWorkConfigs"));
        }
        

        return {
            GetFilteredEmployees: GetFilteredEmployees,
            GetEmployeeById: GetEmployeeById,
            UpdateEmployee: UpdateEmployee,
            AddEmployee: AddEmployee,
            GetWorkConfigs:GetWorkConfigs
       };
    };
    appControllers.service("Demo_Module_EmployeeAPIService", employeeAPIService);

})(appControllers);