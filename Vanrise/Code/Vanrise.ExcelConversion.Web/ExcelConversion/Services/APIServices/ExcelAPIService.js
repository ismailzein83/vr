(function (appControllers) {

    "use strict";
    excelAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_ExcelConversion_ModuleConfig'];
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
        function ReadConditionsFromFile(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "ReadConditionsFromFile"), input);
        }
        
        return ({
            ReadExcelFile: ReadExcelFile,
            ReadExcelFilePage: ReadExcelFilePage,
            GetFieldMappingTemplateConfigs: GetFieldMappingTemplateConfigs,
            GetConcatenatedPartTemplateConfigs: GetConcatenatedPartTemplateConfigs,
            ReadConditionsFromFile: ReadConditionsFromFile
        });
    }

    appControllers.service('VR_ExcelConversion_ExcelAPIService', excelAPIService);

})(appControllers);