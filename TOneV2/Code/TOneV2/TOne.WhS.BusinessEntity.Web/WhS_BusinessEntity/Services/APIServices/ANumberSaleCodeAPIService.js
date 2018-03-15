(function (app) {

    "use strict";
    aNumberSaleCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function aNumberSaleCodeAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "ANumberSaleCode";

        function GetFilteredANumberSaleCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredANumberSaleCodes"), input);
        }

        function GetUploadedSaleCodes(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetUploadedSaleCodes"), { fileId: fileId });
        }
        function AddANumberSaleCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddANumberSaleCodes"), input);
        }
        function DownloadSaleCodesLog(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "DownloadSaleCodesLog"), { fileID: fileId }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        return ({
            GetFilteredANumberSaleCodes: GetFilteredANumberSaleCodes,
            GetUploadedSaleCodes: GetUploadedSaleCodes,
            AddANumberSaleCodes: AddANumberSaleCodes,
            DownloadSaleCodesLog: DownloadSaleCodesLog
        });
    }

    app.service("WhS_BE_ANumberSaleCodeAPIService", aNumberSaleCodeAPIService);
})(app);