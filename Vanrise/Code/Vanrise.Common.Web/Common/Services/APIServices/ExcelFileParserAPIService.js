(function (appControllers) {

    "use strict";
    excelFileParserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function excelFileParserAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'ExcelFileParser';



        function GetUploadedDataValues(fileId, type) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetUploadedDataValues"), {
                fileId: fileId,
                type: type
            });
        }
        function DowloadFileExcelParserTemplate(fieldName) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "DowloadFileExcelParserTemplate"), {
                fieldName: fieldName
            }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        return ({
            GetUploadedDataValues: GetUploadedDataValues,
            DowloadFileExcelParserTemplate: DowloadFileExcelParserTemplate
        });
    }

    appControllers.service('VRCommon_ExcelFileParserAPIService', excelFileParserAPIService);

})(appControllers);