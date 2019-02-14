﻿(function (appControllers) {

    "use strict";

    customerSMSRateChangesAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig"];

    function customerSMSRateChangesAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig) {

        var controllerName = "CustomerSMSRateChanges";

        function GetFilteredChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredChanges"), input);
        }

        function InsertOrUpdateChanges(customerChanges) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "InsertOrUpdateChanges"), customerChanges);
        }

        function UpdateSMSRateChangesStatus(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "UpdateSMSRateChangesStatus"), input);
        }

        function GetDraftData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetDraftData"), input);
        }

        

        return {
            GetFilteredChanges: GetFilteredChanges,
            InsertOrUpdateChanges: InsertOrUpdateChanges,
            UpdateSMSRateChangesStatus: UpdateSMSRateChangesStatus,
            GetDraftData: GetDraftData
        };

    }

    appControllers.service("WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService", customerSMSRateChangesAPIService);

})(appControllers);