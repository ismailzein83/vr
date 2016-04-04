(function (appControllers) {

    "use strict";
    function excelAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {
        var controllerName = 'Excel';
        function ReadExcelFile(fileId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "ReadExcelFile"), {
                fileId: fileId
            });
        }
        function GetFieldMappingTemplateConfigs() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "GetFieldMappingTemplateConfigs"));
        }
        return ({
            ReadExcelFile: ReadExcelFile,
           GetFieldMappingTemplateConfigs:GetFieldMappingTemplateConfigs
        });
    }
    excelAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'ExcelConversion_ModuleConfig'];
    appControllers.service('ExcelConversion_ExcelAPIService', excelAPIService);

})(appControllers);