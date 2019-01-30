﻿(function (appControllers) {

    "use strict";

    customerSMSRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig"];

    function customerSMSRateAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig) {

        var controllerName = "CustomerSMSRate";

        function GetFilteredCustomerSMSRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerSMSRate"), input);
        }

        return {
            GetFilteredCustomerSMSRate: GetFilteredCustomerSMSRate
        };
         
    }

    appControllers.service("WhS_SMSBusinessEntity_CustomerSMSRateAPIService", customerSMSRateAPIService);

})(appControllers);