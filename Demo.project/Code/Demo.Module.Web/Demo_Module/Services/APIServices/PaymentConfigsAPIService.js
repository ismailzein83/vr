
(function (appControllers) {

    'use strict';

    PaymentConfigsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function PaymentConfigsAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = 'PaymentConfigs';

        function GetPaymentTypeTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetPaymentTypeTemplateConfigs'));
        }

        return ({
            GetPaymentTypeTemplateConfigs: GetPaymentTypeTemplateConfigs,
        });
    }


    appControllers.service('Demo_Module_PaymentConfigsAPIService', PaymentConfigsAPIService);
})(appControllers);