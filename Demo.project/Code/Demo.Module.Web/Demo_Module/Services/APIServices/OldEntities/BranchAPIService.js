(function (appControllers) {
    "use strict";
    branchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function branchAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Branch";

        function AddBranch(branch) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddBranch"), branch);
        };

        function GetFilteredBranches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredBranches"), input);
        }
        function GetBranchById(branchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetBranchById"),
                {
                    branchId: branchId
                });
        }

        function UpdateBranch(branch) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateBranch"), branch);
        }


        return {

            AddBranch: AddBranch,
            GetFilteredBranches: GetFilteredBranches,
            GetBranchById: GetBranchById,
            UpdateBranch: UpdateBranch
           
        };
    };
    appControllers.service("Demo_Module_BranchAPIService", branchAPIService);

})(appControllers);