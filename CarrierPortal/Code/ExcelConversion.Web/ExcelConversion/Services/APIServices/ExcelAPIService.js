(function (appControllers) {

    "use strict";
    function excelAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {
        function ReadExcelFile(fileId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "Excel", "ReadExcelFile"), {
                fileId: fileId
            });
        }
       
        return ({
            ReadExcelFile: ReadExcelFile
           
        });
    }
    excelAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'ExcelConversion_ModuleConfig'];
    appControllers.service('ExcelConversion_ExcelAPIService', excelAPIService);

})(appControllers);