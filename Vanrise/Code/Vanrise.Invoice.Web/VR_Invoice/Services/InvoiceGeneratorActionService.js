
app.service('VR_Invoice_InvoiceGeneratorActionService', ['VRModalService',
    function (VRModalService) {

        function addInvoiceGeneratorAction(onInvoiceGeneratorActionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceGeneratorActionAdded = onInvoiceGeneratorActionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/GeneratorSettings/Templates/InvoiceGeneratorActionEditor.html', parameters, settings);
        }

        function editInvoiceGeneratorAction(invoiceGeneratorActionEntity, onInvoiceGeneratorActionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceGeneratorActionUpdated = onInvoiceGeneratorActionUpdated;
            };
            var parameters = {
                invoiceGeneratorActionEntity: invoiceGeneratorActionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/GeneratorSettings/Templates/InvoiceGeneratorActionEditor.html', parameters, settings);
        }

        return ({
            addInvoiceGeneratorAction: addInvoiceGeneratorAction,
            editInvoiceGeneratorAction: editInvoiceGeneratorAction

        });
    }]);
