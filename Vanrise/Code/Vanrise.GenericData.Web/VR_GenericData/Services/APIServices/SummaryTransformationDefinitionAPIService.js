(function (appControllers) {

    'use strict';

    SummaryTransformationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function SummaryTransformationDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        var controllerName = "SummaryTransformationDefinition";

        return ({
            GetSummaryTransformationDefinition: GetSummaryTransformationDefinition,
            GetFilteredSummaryTransformationDefinitions: GetFilteredSummaryTransformationDefinitions,
            AddSummaryTransformationDefinition: AddSummaryTransformationDefinition,
            HasAddSummaryTransformationDefinition: HasAddSummaryTransformationDefinition,
            UpdateSummaryTransformationDefinition: UpdateSummaryTransformationDefinition,
            HasUpdateSummaryTransformationDefinition: HasUpdateSummaryTransformationDefinition,
            GetSummaryTransformationDefinitionInfo: GetSummaryTransformationDefinitionInfo,
            GetSummaryBatchIntervalSourceTemplates: GetSummaryBatchIntervalSourceTemplates
        });
        function GetSummaryTransformationDefinitionInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetSummaryTransformationDefinitionInfo"), { filter: filter });
        }
        function GetFilteredSummaryTransformationDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredSummaryTransformationDefinitions'), input);
        }

        function GetSummaryTransformationDefinition(summaryTransformationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetSummaryTransformationDefinition'), { summaryTransformationDefinitionId: summaryTransformationDefinitionId });
        }
        function AddSummaryTransformationDefinition(summaryTransformationDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'SummaryTransformationDefinition', 'AddSummaryTransformationDefinition'), summaryTransformationDefinition);
        }
        function HasAddSummaryTransformationDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['AddSummaryTransformationDefinition']));
        }
        function UpdateSummaryTransformationDefinition(summaryTransformationDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateSummaryTransformationDefinition'), summaryTransformationDefinitionObject);
        }
        function HasUpdateSummaryTransformationDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['UpdateSummaryTransformationDefinition']));
        }

        function GetSummaryBatchIntervalSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetSummaryBatchIntervalSourceTemplates"));
        }

    }

    appControllers.service('VR_GenericData_SummaryTransformationDefinitionAPIService', SummaryTransformationDefinitionAPIService);

})(appControllers);
