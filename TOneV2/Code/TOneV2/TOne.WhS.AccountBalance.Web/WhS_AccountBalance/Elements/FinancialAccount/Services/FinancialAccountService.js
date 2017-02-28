(function (appControllers) {

    'use stict';

    FinancialAccountService.$inject = ['VRModalService', 'VRNotificationService','WhS_BE_CarrierAccountService','WhS_BE_CarrierProfileService'];

    function FinancialAccountService(VRModalService, VRNotificationService, WhS_BE_CarrierAccountService, WhS_BE_CarrierProfileService) {
        function addFinancialAccount(carrierAccountId, carrierProfileId, onFinancialAccountAdded) {
            var parameters = {
                carrierAccountId: carrierAccountId,
                carrierProfileId: carrierProfileId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountAdded = onFinancialAccountAdded
            };
            VRModalService.showModal('/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Views/FinancialAccountEditor.html', parameters, settings);
        };
        function editFinancialAccount(onFinancialAccountUpdated, financialAccountId) {
            var parameters = {
                financialAccountId: financialAccountId,
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountUpdated = onFinancialAccountUpdated
            };
            VRModalService.showModal('/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Views/FinancialAccountEditor.html', parameters, settings);
        };
        function registerDrillDownToCarrierAccount() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Financial Accounts";
            drillDownDefinition.directive = "whs-accountbalance-financialaccount-search";

            drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                carrierAccountItem.financialAccountGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                    },
                    carrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                };

                return carrierAccountItem.financialAccountGridAPI.load(payload);
            };

            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);
        }
        function registerDrillDownToCarrierProfile() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Financial Accounts";
            drillDownDefinition.directive = "whs-accountbalance-financialaccount-search";

            drillDownDefinition.loadDirective = function (directiveAPI, carrierProfileItem) {
                carrierProfileItem.financialAccountGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                    },
                    carrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                };

                return carrierProfileItem.financialAccountGridAPI.load(payload);
            };

            WhS_BE_CarrierProfileService.addDrillDownDefinition(drillDownDefinition);
        }
        return {
            addFinancialAccount: addFinancialAccount,
            editFinancialAccount:editFinancialAccount,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount,
            registerDrillDownToCarrierProfile: registerDrillDownToCarrierProfile
        };
    }

    appControllers.service('VR_AccountBalance_FinancialAccountService', FinancialAccountService);

})(appControllers);