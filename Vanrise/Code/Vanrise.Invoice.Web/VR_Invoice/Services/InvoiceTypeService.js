
app.service('VR_Invoice_InvoiceTypeService', ['VRModalService',
    function (VRModalService) {

        function addInvoiceType(onInvoiceTypeAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceTypeAdded = onInvoiceTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/InvoiceTypeEditor.html', null, settings);
        }
        function editInvoiceType(onInvoiceTypeUpdated, invoiceTypeId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceTypeUpdated = onInvoiceTypeUpdated;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/InvoiceTypeEditor.html', parameters, settings);
        }

        function addGridColumn(onGridColumnAdded, gridColumns) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridColumnAdded = onGridColumnAdded;
            };
            var parameters = {
                gridColumns: gridColumns
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/MainGridColumnsEditor.html', parameters, settings);
        }

        function editGridColumn(columnEntity, onGridColumnUpdated, gridColumns) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridColumnUpdated = onGridColumnUpdated;
            };
            var parameters = {
                columnEntity: columnEntity,
                gridColumns: gridColumns
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/MainGridColumnsEditor.html', parameters, settings);
        }

        function addSubSection(onSubSectionAdded, subSections) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubSectionAdded = onSubSectionAdded;
            };
            var parameters = {
                subSections: subSections
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/SubSectionEditor.html', parameters, settings);
        }

        function editSubSection(subSectionEntity, onSubSectionUpdated, subSections) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubSectionUpdated = onSubSectionUpdated;
            };
            var parameters = {
                subSectionEntity: subSectionEntity,
                subSections: subSections
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/SubSectionEditor.html', parameters, settings);
        }
        return ({
            addInvoiceType: addInvoiceType,
            editInvoiceType: editInvoiceType,
            addGridColumn: addGridColumn,
            editGridColumn: editGridColumn,
            addSubSection: addSubSection,
            editSubSection: editSubSection
        });
    }]);
