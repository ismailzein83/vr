
app.service('VR_Invoice_InvoiceBulkActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService) {
    
        function addInvoiceBulkAction(onInvoiceBulkActionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceBulkActionAdded = onInvoiceBulkActionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceBulkAction/Templates/InvoiceBulkActionEditor.html', parameters, settings);
        }
        function editInvoiceBulkAction(invoiceBulkActionEntity, onInvoiceBulkActionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceBulkActionUpdated = onInvoiceBulkActionUpdated;
            };
            var parameters = {
                invoiceBulkActionEntity: invoiceBulkActionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceBulkAction/Templates/InvoiceBulkActionEditor.html', parameters, settings);
        }

        function addMenualBulkAction(onInvoiceBulkActionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceBulkActionAdded = onInvoiceBulkActionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/MenualBulkAction/Templates/MenualBulkActionEditor.html', parameters, settings);
        }
        function editMenualBulkAction(actionEntity, onInvoiceBulkActionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceBulkActionUpdated = onInvoiceBulkActionUpdated;
            };
            var parameters = {
                actionEntity: actionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/MenualBulkAction/Templates/MenualBulkActionEditor.html', parameters, settings);
        }
        function openMenualInvoiceBulkActions(onBulkActionExecuted, invoiceTypeId, context)
        {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onBulkActionExecuted = onBulkActionExecuted;
            };
            var parameters = {
                context: context,
                invoiceTypeId:invoiceTypeId
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/MenualInvoiceBulkActionsRunTimeEditor.html', parameters, settings);
        }

        return ({
            addInvoiceBulkAction: addInvoiceBulkAction,
            editInvoiceBulkAction: editInvoiceBulkAction,
            editMenualBulkAction: editMenualBulkAction,
            addMenualBulkAction: addMenualBulkAction,
            openMenualInvoiceBulkActions: openMenualInvoiceBulkActions
        });
    }]);
