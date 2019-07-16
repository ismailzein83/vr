(function (appControllers) {
    "use strict";
    customerTargetMatchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function customerTargetMatchAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName = "CustomerTargetMatch";

        function DownloadCustomerTargetMatchTemplate(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "DownloadCustomerTargetMatchTemplate"), { customerId }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        return ({
            DownloadCustomerTargetMatchTemplate: DownloadCustomerTargetMatchTemplate
        });
    }

    appControllers.service('WhS_Sales_CustomerTargetMatchAPIService', customerTargetMatchAPIService);

})(appControllers);