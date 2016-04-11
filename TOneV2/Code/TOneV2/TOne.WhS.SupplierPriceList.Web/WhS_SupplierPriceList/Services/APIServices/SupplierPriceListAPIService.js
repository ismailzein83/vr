(function (appControllers) {

    "use strict";
    supplierPriceListAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SupPL_ModuleConfig", "SecurityService"];

    function supplierPriceListAPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig, SecurityService) {

        var controllerName = "SupplierPriceList";

        function DownloadSupplierPriceListTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "DownloadSupplierPriceListTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function HasDownloadSupplierPriceListTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SupPL_ModuleConfig.moduleName, controllerName, ['DownloadSupplierPriceListTemplate']));
        }

        return ({
            DownloadSupplierPriceListTemplate: DownloadSupplierPriceListTemplate,
            HasDownloadSupplierPriceListTemplatePermission: HasDownloadSupplierPriceListTemplatePermission
        });
    }

    appControllers.service("WhS_SupPL_SupplierPriceListAPIService", supplierPriceListAPIService);
})(appControllers);