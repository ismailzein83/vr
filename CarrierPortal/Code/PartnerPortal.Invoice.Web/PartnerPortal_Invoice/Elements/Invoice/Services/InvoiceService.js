
app.service('PartnerPortal_Invoice_InvoiceService', ['VRModalService',
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
            VRModalService.showModal('/Client/Modules/PartnerPortal_Invoice/Elements/Invoice/Directives/Templates/GridColumnsEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/PartnerPortal_Invoice/Elements/Invoice/Directives/Templates/GridColumnsEditor.html', parameters, settings);
        }

        return ({
            addGridColumn: addGridColumn,
            editGridColumn: editGridColumn,
        });
    }]);
