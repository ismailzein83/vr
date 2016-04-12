(function (appControllers) {

    "use strict";
    function excelAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {
        var controllerName = 'Excel';
        function ReadExcelFile(fileId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "ReadExcelFile"), {
                fileId: fileId
            });
        }

        function ReadExcelFilePage(page) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "ReadExcelFilePage"), page);
        }
        function GetFieldMappingTemplateConfigs() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "GetFieldMappingTemplateConfigs"));
        }
        function GetConcatenatedPartTemplateConfigs() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "GetConcatenatedPartTemplateConfigs"));
        }
        return ({
            ReadExcelFile: ReadExcelFile,
            ReadExcelFilePage:ReadExcelFilePage,
            GetFieldMappingTemplateConfigs: GetFieldMappingTemplateConfigs,
            GetConcatenatedPartTemplateConfigs: GetConcatenatedPartTemplateConfigs
        });
    }
    excelAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'ExcelConversion_ModuleConfig'];
    appControllers.service('ExcelConversion_ExcelAPIService', excelAPIService);

})(appControllers);