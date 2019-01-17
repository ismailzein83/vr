(function (appControllers) {

    "use strict";

    branchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function branchAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Demo_Branch";

        function GetFilteredBranches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredBranches"), input);
        };

        function GetBranchById(branchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetBranchById"), {
                branchId: branchId
            });
        };

        function GetBranchTypeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetBranchTypeConfigs"));
        }

        function GetDepartmentSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDepartmentSettingsConfigs"));
        }

        function GetEmployeeSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetEmployeeSettingsConfigs"));
        }

        function AddBranch(branch) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddBranch"), branch);
        };

        function UpdateBranch(branch) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateBranch"), branch);
        };

        return {
            GetFilteredBranches: GetFilteredBranches,
            GetBranchById: GetBranchById,
            GetBranchTypeConfigs: GetBranchTypeConfigs,
            GetDepartmentSettingsConfigs: GetDepartmentSettingsConfigs,
            GetEmployeeSettingsConfigs: GetEmployeeSettingsConfigs,
            AddBranch: AddBranch,
            UpdateBranch: UpdateBranch
        };
    };

    appControllers.service("Demo_Module_BranchAPIService", branchAPIService);
})(appControllers);







