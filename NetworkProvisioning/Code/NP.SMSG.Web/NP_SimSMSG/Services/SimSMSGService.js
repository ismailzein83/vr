(function (appControllers) {

    "use strict";

    SimSMSGService.$inject = ['WhS_BE_CarrierAccountService', 'UtilsService','WhS_BE_CarrierAccountTypeEnum'];

    function SimSMSGService(WhS_BE_CarrierAccountService, UtilsService, WhS_BE_CarrierAccountTypeEnum) {
       
        function registerClientDrillDownToCarrierAccount() {
            var customerDrillDownTab = {};

            customerDrillDownTab.title = "Client";
            customerDrillDownTab.directive = "vr-genericdata-genericbusinessentity-management";

            customerDrillDownTab.loadDirective = function (genericBusinessEntityAPI, carrierAccountIdObj) {
                var carrierAccountId = carrierAccountIdObj.Entity.CarrierAccountId;
                var promise = UtilsService.createPromiseDeferred();

                var genericBusinessEntityPayload = {
                    businessEntityDefinitionId: "3c7db360-c4f5-4c08-85b6-7fcece929b68",
                    fieldValues: {
                        CustomerId: {
                            value: carrierAccountId,
                            isHidden: true,
                            isDisabled: false
                        }
                    },
                    filterValues: {
                        CustomerId: {
                            value: carrierAccountId,
                            isHidden: true,
                            isDisabled: false
                        }
                    }
                };
                genericBusinessEntityAPI.load(genericBusinessEntityPayload).then(function () {
                    promise.resolve();
                }).catch(function (error) {
                    promise.reject(error);
                });
                return promise.promise;
            };
            customerDrillDownTab.hideDrillDownFunction = function (carrierAccountIdObj) {
                var accountType = carrierAccountIdObj.Entity.AccountType;
                if (WhS_BE_CarrierAccountTypeEnum.Supplier.value == accountType)
                    return true;
            };
            WhS_BE_CarrierAccountService.addDrillDownDefinition(customerDrillDownTab);
        }

        function registerServerDrillDownToCarrierAccount() {
            var serverDrillDownTab = {};

            serverDrillDownTab.title = "Server";
            serverDrillDownTab.directive = "vr-genericdata-genericbusinessentity-management";

            serverDrillDownTab.loadDirective = function (genericBusinessEntityAPI, carrierAccountIdObj) {
                var carrierAccountId = carrierAccountIdObj.Entity.CarrierAccountId;
                var promise = UtilsService.createPromiseDeferred();

                var genericBusinessEntityPayload = {
                    businessEntityDefinitionId: "28fa8d7f-0322-402d-a13d-ac37f2e934b9",
                    fieldValues: {
                        SupplierId: {
                            value: carrierAccountId,
                            isHidden: true,
                            isDisabled: false
                        }
                    },
                    filterValues: {
                        SupplierId: {
                            value: carrierAccountId,
                            isHidden: true,
                            isDisabled: false
                        }
                    }
                };
                genericBusinessEntityAPI.load(genericBusinessEntityPayload).then(function () {
                    promise.resolve();
                }).catch(function (error) {
                    promise.reject(error);
                });
                return promise.promise;
            };
            serverDrillDownTab.hideDrillDownFunction = function (carrierAccountIdObj) {
                var accountType = carrierAccountIdObj.Entity.AccountType;
                if (WhS_BE_CarrierAccountTypeEnum.Customer.value == accountType)
                    return true;
            };
            WhS_BE_CarrierAccountService.addDrillDownDefinition(serverDrillDownTab);
        }

        return {
            registerClientDrillDownToCarrierAccount: registerClientDrillDownToCarrierAccount,
            registerServerDrillDownToCarrierAccount: registerServerDrillDownToCarrierAccount
        };
    }
    appControllers.service('NP_SimSMSG_Service', SimSMSGService);

})(appControllers);