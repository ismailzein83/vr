
app.service('VR_Invoice_InvoiceGridSettingService', ['VRModalService',
    function (VRModalService) {


        function addGridColumn(onGridColumnAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridColumnAdded = onGridColumnAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/Templates/MainGridColumnsEditor.html', parameters, settings);
        }

        function editGridColumn(columnEntity, onGridColumnUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridColumnUpdated = onGridColumnUpdated;
            };
            var parameters = {
                columnEntity: columnEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/Templates/MainGridColumnsEditor.html', parameters, settings);
        }

        function addGridAction(onGridActionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridActionAdded = onGridActionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/Templates/InvoiceGridActionEditor.html', parameters, settings);
        }

        function editGridAction(actionEntity, onGridActionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridActionUpdated = onGridActionUpdated;
            };
            var parameters = {
                actionEntity: actionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/Templates/InvoiceGridActionEditor.html', parameters, settings);
        }

        return ({
            addGridColumn: addGridColumn,
            editGridColumn: editGridColumn,
            addGridAction: addGridAction,
            editGridAction: editGridAction,
        });
    }]);
