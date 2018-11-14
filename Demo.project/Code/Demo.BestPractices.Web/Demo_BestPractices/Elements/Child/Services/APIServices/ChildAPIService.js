﻿(function (appControllers) {

    "use strict";

    childAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_BestPractices_ModuleConfig', 'SecurityService'];

    function childAPIService(BaseAPIService, UtilsService, Demo_BestPractices_ModuleConfig, SecurityService) {

        var controller = "Demo_Child";

        function GetFilteredChilds(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetFilteredChilds"), input);
        }

        function GetChildById(childId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetChildById"), {
                childId: childId
            });
        }

        function GetChildShapeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "GetChildShapeConfigs"));
        }

        function AddChild(child) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "AddChild"), child);
        };

        function UpdateChild(child) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_BestPractices_ModuleConfig.moduleName, controller, "UpdateChild"), child);
        }

        return {
            GetFilteredChilds: GetFilteredChilds,
            GetChildById: GetChildById,
            GetChildShapeConfigs: GetChildShapeConfigs,
            AddChild: AddChild,
            UpdateChild: UpdateChild
        };
    };

    appControllers.service("Demo_BestPractices_ChildAPIService", childAPIService);
})(appControllers);