(function (app) {

    "use strict";
    aNumberSupplierCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function aNumberSupplierCodeAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "ANumberSupplierCode";

        function GetFilteredANumberSupplierCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredANumberSupplierCodes"), input);
        }

        function GetUploadedSupplierCodes(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetUploadedSupplierCodes"), { fileId: fileId });
        }
        function AddANumberSupplierCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddANumberSupplierCodes"), input);
        }
        function DownloadSupplierCodesLog(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "DownloadSupplierCodesLog"), { fileID: fileId }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        return ({
            GetFilteredANumberSupplierCodes: GetFilteredANumberSupplierCodes,
            GetUploadedSupplierCodes: GetUploadedSupplierCodes,
            AddANumberSupplierCodes: AddANumberSupplierCodes,
            DownloadSupplierCodesLog: DownloadSupplierCodesLog
        });
    }

    app.service("WhS_BE_ANumberSupplierCodeAPIService", aNumberSupplierCodeAPIService);
})(app);