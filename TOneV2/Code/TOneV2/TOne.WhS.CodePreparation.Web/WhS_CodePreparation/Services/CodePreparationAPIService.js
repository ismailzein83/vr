(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CodePrep_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_CodePrep_ModuleConfig) {

        function UploadSaleZonesList(sellingNumberPlanId, fileId, effectiveDate) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName,"CodePreparation","UploadSaleZonesList"), {
                sellingNumberPlanId: sellingNumberPlanId,
                fileId: fileId,
                effectiveDate: effectiveDate
            });
        }
        function DownloadImportCodePreparationTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "DownloadImportCodePreparationTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        return ({
            UploadSaleZonesList: UploadSaleZonesList,
            DownloadImportCodePreparationTemplate: DownloadImportCodePreparationTemplate
        });
    }

    appControllers.service('WhS_CodePrep_CodePrepAPIService', supplierZoneAPIService);
})(appControllers);