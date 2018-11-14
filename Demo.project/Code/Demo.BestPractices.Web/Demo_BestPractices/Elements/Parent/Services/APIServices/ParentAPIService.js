(function (appControllers) {

    "use strict";

    parentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_BestPractices_ModuleConfig', 'SecurityService'];

    function parentAPIService(BaseAPIService, UtilsService, Demo_BestPractices_ModuleConfig, SecurityService) {

        var controller = "Demo_Parent";

        function GetFilteredParents(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetFilteredParents"), input);
        }

        function GetParentById(parentId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetParentById"), {
                parentId: parentId
            });
        }

        function AddParent(parent) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "AddParent"), parent);
        };

        function UpdateParent(parent) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "UpdateParent"), parent);
        }

        function GetParentsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetParentsInfo"), {
                filter: filter
            });
        };

        return {
            GetFilteredParents: GetFilteredParents,
            GetParentById: GetParentById,
            GetParentsInfo: GetParentsInfo,
            AddParent: AddParent,
            UpdateParent: UpdateParent
        };
    };

    appControllers.service("Demo_BestPractices_ParentAPIService", parentAPIService);
})(appControllers);