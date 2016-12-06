
app.service('WhS_Invoice_InvoiceSettingService', ['VRModalService', 'UtilsService',
    function (VRModalService, UtilsService) {
        function getCustomerInvoiceMailType()
        {
            var promiseDeffered = UtilsService.createPromiseDeferred();
            promiseDeffered.resolve("d077a578-53b3-4faf-84b7-5e1ef5724c79");
            return promiseDeffered.promise;
        }

        function addInvoiceSetting(onInvoiceSettingsAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceSettingsAdded = onInvoiceSettingsAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Invoice/Views/InvoiceSettings/Templates/InvoiceSettingEditor.html', modalParameters, modalSettings);
        }

        function editInvoiceSetting(invoiceSettingEntity, onInvoiceSettingsUpdated) {
            var modalParameters = {
                invoiceSettingEntity: invoiceSettingEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceSettingsUpdated = onInvoiceSettingsUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Invoice/Views/InvoiceSettings/Templates/InvoiceSettingEditor.html', modalParameters, modalSettings);
        }

        return ({
            getCustomerInvoiceMailType: getCustomerInvoiceMailType,
            addInvoiceSetting: addInvoiceSetting,
            editInvoiceSetting: editInvoiceSetting
        });
    }]);
