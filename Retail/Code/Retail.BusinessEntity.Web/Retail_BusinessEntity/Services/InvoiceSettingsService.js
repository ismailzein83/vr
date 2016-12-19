(function (appControllers) {

    'use strict';

    InvoiceSettingService.$inject = ['UtilsService', 'VRModalService'];

    function InvoiceSettingService(UtilsService, VRModalService) {
        return ({
            addInvoiceSetting: addInvoiceSetting,
            editInvoiceSetting: editInvoiceSetting,
            getCustomerInvoiceMailType: getCustomerInvoiceMailType
        });


        function addInvoiceSetting(onInvoiceSettingsAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceSettingsAdded = onInvoiceSettingsAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/InvoiceSettings/Templates/InvoiceSettingEditor.html', modalParameters, modalSettings);
        }

        function editInvoiceSetting(invoiceSettingEntity, onInvoiceSettingsUpdated) {
            var modalParameters = {
                invoiceSettingEntity: invoiceSettingEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceSettingsUpdated = onInvoiceSettingsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/InvoiceSettings/Templates/InvoiceSettingEditor.html', modalParameters, modalSettings);
        }

        function getCustomerInvoiceMailType() {
            var promiseDeffered = UtilsService.createPromiseDeferred();
            promiseDeffered.resolve("d077a578-53b3-4faf-84b7-5e1ef5724c79");
            return promiseDeffered.promise;
        }
    }

    appControllers.service('Retail_BE_InvoiceSettingService', InvoiceSettingService);

})(appControllers);
