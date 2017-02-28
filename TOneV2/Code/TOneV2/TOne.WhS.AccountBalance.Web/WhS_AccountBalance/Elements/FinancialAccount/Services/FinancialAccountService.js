(function (appControllers) {

    'use stict';

    FinancialAccountService.$inject = ['VRModalService', 'VRNotificationService','WhS_BE_CarrierAccountService'];

    function FinancialAccountService(VRModalService, VRNotificationService, WhS_BE_CarrierAccountService) {
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
                    CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                };

                return carrierAccountItem.financialAccountGridAPI.load(payload);
            };

            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);
        }
        return {
            addFinancialAccount: addFinancialAccount,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount
        };
    }

    appControllers.service('VR_AccountBalance_FinancialAccountService', FinancialAccountService);

})(appControllers);