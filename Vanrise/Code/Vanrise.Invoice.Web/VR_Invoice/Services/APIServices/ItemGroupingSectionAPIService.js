(function (appControllers) {

    "use strict";
    itemGroupingSectionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function itemGroupingSectionAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'ItemGroupingSection';

        function GetFilteredGroupingInvoiceItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredGroupingInvoiceItems"), input);
        }
        return ({
            GetFilteredGroupingInvoiceItems: GetFilteredGroupingInvoiceItems,
        });
    }

    appControllers.service('VR_Invoice_ItemGroupingSectionAPIService', itemGroupingSectionAPIService);

})(appControllers);