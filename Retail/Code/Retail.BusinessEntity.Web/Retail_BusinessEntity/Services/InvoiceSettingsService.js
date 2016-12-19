(function (appControllers) {

    'use strict';

    InvoiceSettingService.$inject = ['UtilsService', 'VRModalService'];

    function InvoiceSettingService(UtilsService, VRModalService) {
        return ({
            addInvoiceSetting: addInvoiceSetting,
            editInvoiceSetting: editInvoiceSetting,
            getSubscriberInvoiceMailType: getSubscriberInvoiceMailType
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

        function getSubscriberInvoiceMailType() {
            var promiseDeffered = UtilsService.createPromiseDeferred();
            promiseDeffered.resolve("691E1A9E-41BC-482D-88BE-7DC77E2D2CA1");
            return promiseDeffered.promise;
        }
    }

    appControllers.service('Retail_BE_InvoiceSettingService', InvoiceSettingService);

})(appControllers);
