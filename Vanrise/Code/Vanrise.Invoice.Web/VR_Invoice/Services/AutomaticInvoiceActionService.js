
app.service('VR_Invoice_AutomaticInvoiceActionService', ['VRModalService', 'UtilsService',  'VRNotificationService', 'SecurityService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService) {
    
        function addAutomaticInvoiceAction(onAutomaticInvoiceActionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAutomaticInvoiceActionAdded = onAutomaticInvoiceActionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/Templates/AutomaticInvoiceActionEditor.html', parameters, settings);
        }
        function editAutomaticInvoiceAction(automaticInvoiceActionEntity, onAutomaticInvoiceActionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAutomaticInvoiceActionUpdated = onAutomaticInvoiceActionUpdated;
            };
            var parameters = {
                automaticInvoiceActionEntity: automaticInvoiceActionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/Templates/AutomaticInvoiceActionEditor.html', parameters, settings);
        }

        return ({
            addAutomaticInvoiceAction: addAutomaticInvoiceAction,
            editAutomaticInvoiceAction: editAutomaticInvoiceAction,
        });
    }]);
