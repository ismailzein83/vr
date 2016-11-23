
app.service('VR_Invoice_InvoiceSubSectionSettingsService', ['VRModalService',
    function (VRModalService) {

        function addSubSection(onSubSectionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubSectionAdded = onSubSectionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceSubSectionSettings/SubSectionEditor.html', parameters, settings);
        }

        function editSubSection(subSectionEntity, onSubSectionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubSectionUpdated = onSubSectionUpdated;
            };
            var parameters = {
                subSectionEntity: subSectionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceSubSectionSettings/SubSectionEditor.html', parameters, settings);
        }

        function addSubSectionGridColumn(onSubSectionGridColumnAdded, gridColumns) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubSectionGridColumnAdded = onSubSectionGridColumnAdded;
            };
            var parameters = {
                gridColumns: gridColumns
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceSubSectionSettings/SubSectionGridColumnEditor.html', parameters, settings);
        }

        function editSubSectionGridColumn(gridColumnEntity, onSubSectionGridColumnUpdated, gridColumns) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubSectionGridColumnUpdated = onSubSectionGridColumnUpdated;
            };
            var parameters = {
                gridColumnEntity: gridColumnEntity,
                gridColumns: gridColumns
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceSubSectionSettings/SubSectionGridColumnEditor.html', parameters, settings);
        }

        return ({
            addSubSection: addSubSection,
            editSubSection: editSubSection,
            addSubSectionGridColumn: addSubSectionGridColumn,
            editSubSectionGridColumn: editSubSectionGridColumn,
        });
    }]);
