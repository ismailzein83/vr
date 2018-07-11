(function (appControllers) {
    "use strict";
    paymentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function paymentAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Payment";

        function GetPayments() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetPayments"));
        }

        return {
            GetPayments: GetPayments
        };
    };
    appControllers.service("Demo_Module_PaymentAPIService", paymentAPIService);

})(appControllers);