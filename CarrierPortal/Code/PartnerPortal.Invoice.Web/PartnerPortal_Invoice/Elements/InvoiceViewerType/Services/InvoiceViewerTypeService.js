
app.service('PartnerPortal_Invoice_InvoiceViewerTypeService', ['VRModalService',
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
            VRModalService.showModal('/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceViewerType/Directives/Templates/GridColumnsEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceViewerType/Directives/Templates/GridColumnsEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceViewerType/Directives/Templates/GridActionEditor.html', parameters, settings);
        }

        function editGridAction(gridActionEntity, onGridActionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridActionUpdated = onGridActionUpdated;
            };
            var parameters = {
                gridActionEntity: gridActionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceViewerType/Directives/Templates/GridActionEditor.html', parameters, settings);
        }


        return ({
            addGridColumn: addGridColumn,
            editGridColumn: editGridColumn,
            addGridAction: addGridAction,
            editGridAction: editGridAction
        });
    }]);
