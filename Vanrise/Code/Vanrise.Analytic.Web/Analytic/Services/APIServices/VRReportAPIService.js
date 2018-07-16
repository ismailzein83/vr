
//(function (appControllers) {

//    "use strict";

//    DataAnalysisDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig', 'SecurityService'];

//    function DataAnalysisDefinitionAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig, SecurityService) {

//        var controllerName = "DataAnalysisDefinition";


//        function GetFilteredDataAnalysisDefinitions(input) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFilteredDataAnalysisDefinitions'), input);
//        }

//        function GetDataAnalysisDefinition(dataAnalysisDefinitionId) {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetDataAnalysisDefinition'), {
//                dataAnalysisDefinitionId: dataAnalysisDefinitionId
//            });
//        }

//        function AddDataAnalysisDefinition(dataAnalysisDefinitionItem) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddDataAnalysisDefinition'), dataAnalysisDefinitionItem);
//        }

//        function UpdateDataAnalysisDefinition(dataAnalysisDefinitionItem) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateDataAnalysisDefinition'), dataAnalysisDefinitionItem);
//        }

//        function GetDataAnalysisDefinitionSettingsExtensionConfigs() {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDataAnalysisDefinitionSettingsExtensionConfigs"));
//        }

//        function GetDataAnalysisDefinitionsInfo(filter) {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDataAnalysisDefinitionsInfo"), {
//                filter: filter
//            });
//        }

//        function HasAddDataAnalysisDefinitionPermission() {
//            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Analytic_ModuleConfig.moduleName, controllerName, ['AddDataAnalysisDefinition']));
//        }

//        function HasEditDataAnalysisDefinitionPermission() {
//            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Analytic_ModuleConfig.moduleName, controllerName, ['UpdateDataAnalysisDefinition']));
//        }


//        return ({
//            GetFilteredDataAnalysisDefinitions: GetFilteredDataAnalysisDefinitions,
//            GetDataAnalysisDefinition: GetDataAnalysisDefinition,
//            AddDataAnalysisDefinition: AddDataAnalysisDefinition,
//            UpdateDataAnalysisDefinition: UpdateDataAnalysisDefinition,
//            GetDataAnalysisDefinitionSettingsExtensionConfigs: GetDataAnalysisDefinitionSettingsExtensionConfigs,
//            GetDataAnalysisDefinitionsInfo: GetDataAnalysisDefinitionsInfo,
//            HasAddDataAnalysisDefinitionPermission: HasAddDataAnalysisDefinitionPermission,
//            HasEditDataAnalysisDefinitionPermission: HasEditDataAnalysisDefinitionPermission
//        });
//    }

//    appControllers.service('VR_Analytic_DataAnalysisDefinitionAPIService', DataAnalysisDefinitionAPIService);
//})(appControllers);