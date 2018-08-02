(function (appControllers) {

    "use strict";
    excelFileUploaderAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function excelFileUploaderAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'ExcelFileUploader';


        function UploadExcelFile(excelUploaderInput) {

            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UploadExcelFile"), excelUploaderInput);
        }

        return ({
            UploadExcelFile: UploadExcelFile
        });
    }

    appControllers.service('VRCommon_ExcelFileUploaderAPIService', excelFileUploaderAPIService);

})(appControllers);