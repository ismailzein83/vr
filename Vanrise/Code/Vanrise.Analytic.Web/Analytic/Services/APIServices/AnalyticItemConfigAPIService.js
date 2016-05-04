(function (appControllers) {

    "use strict";
    AnalyticItemConfigAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticItemConfigAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticItemConfig';

        function GetDimensionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDimensionsInfo"), {
                filter: filter
            });
        }
        function GetMeasuresInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetMeasuresInfo"), {
                filter: filter
            });
        }
        function GetJoinsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetJoinsInfo"), {
                filter: filter
            });
        }

        function GetFilteredAnalyticItemConfigs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredAnalyticItemConfigs"), input);
        }

        function GetAnalyticItemConfigsById(tableId, itemType, analyticItemConfigId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticItemConfigsById"), {
                tableId: tableId,
                itemType: itemType,
                analyticItemConfigId: analyticItemConfigId
            });
        }
        function AddAnalyticItemConfig(analyticItemConfig) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "AddAnalyticItemConfig"), analyticItemConfig);
        }
        function UpdateAnalyticItemConfig(analyticItemConfig) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "UpdateAnalyticItemConfig"), analyticItemConfig);
        }
        return ({
            GetDimensionsInfo: GetDimensionsInfo,
            GetMeasuresInfo: GetMeasuresInfo,
            GetJoinsInfo: GetJoinsInfo,
            GetFilteredAnalyticItemConfigs: GetFilteredAnalyticItemConfigs,
            GetAnalyticItemConfigsById: GetAnalyticItemConfigsById,
            AddAnalyticItemConfig: AddAnalyticItemConfig,
            UpdateAnalyticItemConfig: UpdateAnalyticItemConfig
        });
    }

    appControllers.service('VR_Analytic_AnalyticItemConfigAPIService', AnalyticItemConfigAPIService);

})(appControllers);