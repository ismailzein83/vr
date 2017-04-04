(function (appControllers) {

    'use stict';

    InvoiceAccountService.$inject = ['VRModalService', 'VRNotificationService','WhS_BE_CarrierAccountService','WhS_BE_CarrierProfileService'];

    function InvoiceAccountService(VRModalService, VRNotificationService, WhS_BE_CarrierAccountService, WhS_BE_CarrierProfileService) {

        function addInvoiceAccount(carrierAccountId, carrierProfileId, onInvoiceAccountAdded) {
            var parameters = {
                carrierAccountId: carrierAccountId,
                carrierProfileId: carrierProfileId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceAccountAdded = onInvoiceAccountAdded
            };
            VRModalService.showModal('/Client/Modules/WhS_Invoice/Views/InvoiceAccountEditor.html', parameters, settings);
        };

        function editInvoiceAccount(onInvoiceAccountUpdated, invoiceAccountId) {
            var parameters = {
                invoiceAccountId: invoiceAccountId,
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceAccountUpdated = onInvoiceAccountUpdated
            };
            VRModalService.showModal('/Client/Modules/WhS_Invoice/Views/InvoiceAccountEditor.html', parameters, settings);
        };

        function registerDrillDownToCarrierAccount() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Invoice Accounts";
            drillDownDefinition.directive = "whs-invoice-invoiceaccount-search";

            drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                carrierAccountItem.invoiceAccountGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                    },
                    carrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                };

                return carrierAccountItem.invoiceAccountGridAPI.load(payload);
            };

            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);
        }

        function registerDrillDownToCarrierProfile() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Invoice Accounts";
            drillDownDefinition.directive = "whs-invoice-invoiceaccount-search";

            drillDownDefinition.loadDirective = function (directiveAPI, carrierProfileItem) {
                carrierProfileItem.invoiceAccountGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                    },
                    carrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                };

                return carrierProfileItem.invoiceAccountGridAPI.load(payload);
            };

            WhS_BE_CarrierProfileService.addDrillDownDefinition(drillDownDefinition);
        }

        return {
            addInvoiceAccount: addInvoiceAccount,
            editInvoiceAccount:editInvoiceAccount,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount,
            registerDrillDownToCarrierProfile: registerDrillDownToCarrierProfile
        };
    }

    appControllers.service('WhS_Invoice_InvoiceAccountService', InvoiceAccountService);

})(appControllers);