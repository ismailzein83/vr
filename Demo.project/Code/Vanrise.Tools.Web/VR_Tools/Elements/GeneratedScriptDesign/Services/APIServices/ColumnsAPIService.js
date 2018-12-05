(function (appControllers) {
    "use strict";
    columnsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Tools_ModuleConfig', 'SecurityService'];
    function columnsAPIService(BaseAPIService, UtilsService, VR_Tools_ModuleConfig, SecurityService) {

        var controller = "Columns";

        function GetColumnsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GetColumnsInfo"), { filter: filter });
        }
        function GetGeneratedScriptItemTableSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GetGeneratedScriptItemTableSettingsConfigs"));
        }
        function Validate(generatedScriptItemTables) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "Validate"), generatedScriptItemTables);
        }
        function GenerateQueries(generatedScriptItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GenerateQueries"), generatedScriptItem);
        }
        function GenerateQueriesFromTextFile(filePath,type) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GenerateQueriesFromTextFile"), {
                filePath: filePath,
                type: type
            });
        }
        return {
            GetColumnsInfo: GetColumnsInfo,
            GetGeneratedScriptItemTableSettingsConfigs: GetGeneratedScriptItemTableSettingsConfigs,
            Validate: Validate,
            GenerateQueries: GenerateQueries,
            GenerateQueriesFromTextFile: GenerateQueriesFromTextFile
        };
    }
    appControllers.service("VR_Tools_ColumnsAPIService", columnsAPIService);

})(appControllers);