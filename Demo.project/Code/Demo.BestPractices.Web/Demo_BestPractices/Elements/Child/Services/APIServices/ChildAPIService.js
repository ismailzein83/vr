(function (appControllers) {
    "use strict";
    childAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_BestPractices_ModuleConfig', 'SecurityService'];
    function childAPIService(BaseAPIService, UtilsService, Demo_BestPractices_ModuleConfig, SecurityService) {

        var controller = "Demo_Child";

        function GetFilteredChilds(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetFilteredChilds"), input);
        }
        function GetChildById(childId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetChildById"),
                {
                    childId: childId
                });
        }

        function UpdateChild(child) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "UpdateChild"), child);
        }
        function AddChild(child) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "AddChild"), child);
        };
        function GetChildShapeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetChildShapeConfigs"));
        }
        return {
            GetFilteredChilds: GetFilteredChilds,
            GetChildById: GetChildById,
            UpdateChild: UpdateChild,
            AddChild: AddChild,
            GetChildShapeConfigs: GetChildShapeConfigs
        };
    };
    appControllers.service("Demo_BestPractices_ChildAPIService", childAPIService);

})(appControllers);