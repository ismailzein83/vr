(function (appControllers) {
    "use strict";
    parentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_BestPractices_ModuleConfig', 'SecurityService'];
    function parentAPIService(BaseAPIService, UtilsService, Demo_BestPractices_ModuleConfig, SecurityService) {

        var controller = "Demo_Parent";

        function GetFilteredParents(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetFilteredParents"), input);
        }
        function GetParentById(parentId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetParentById"),
                {
                    parentId: parentId
                });
        }

        function UpdateParent(parent) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "UpdateParent"), parent);
        }
        function AddParent(parent) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "AddParent"), parent);
        };

        return {
            GetFilteredParents: GetFilteredParents,
            GetParentById: GetParentById,
            UpdateParent: UpdateParent,
            AddParent: AddParent,
        };
    };
    appControllers.service("Demo_BestPractices_ParentAPIService", parentAPIService);

})(appControllers);