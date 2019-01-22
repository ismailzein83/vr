(function (appControllers) {
    "use strict";
    columnsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Devtools_ModuleConfig', 'SecurityService'];
    function columnsAPIService(BaseAPIService, UtilsService, VR_Devtools_ModuleConfig, SecurityService) {

        var controller = "Columns";

        function GetColumnsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetColumnsInfo"), { filter: filter });
        }
        function GetGeneratedScriptItemTableSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetGeneratedScriptItemTableSettingsConfigs"));
        }
        function GetGeneratedScriptVariableSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetGeneratedScriptVariableSettingsConfigs"));
        }
        function Validate(generatedScriptItemTables) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "Validate"), generatedScriptItemTables);
        }
        function GenerateQueries(generatedScriptItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GenerateQueries"), generatedScriptItem);
        }
        function GenerateQueriesFromTextFile(filePath, type) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GenerateQueriesFromTextFile"), {
                filePath: filePath,
                type: type
            });
        }
        return {
            GetColumnsInfo: GetColumnsInfo,
            GetGeneratedScriptItemTableSettingsConfigs: GetGeneratedScriptItemTableSettingsConfigs,
            GetGeneratedScriptVariableSettingsConfigs: GetGeneratedScriptVariableSettingsConfigs,
            Validate: Validate,
            GenerateQueries: GenerateQueries,
            GenerateQueriesFromTextFile: GenerateQueriesFromTextFile
        };
    }
    appControllers.service("VR_Devtools_ColumnsAPIService", columnsAPIService);

})(appControllers);