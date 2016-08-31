
app.service('VR_Invoice_InvoiceTypeService', ['VRModalService',
    function (VRModalService) {

        function addInvoiceType(onInvoiceTypeAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceTypeAdded = onInvoiceTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceTypeEditor.html', null, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceTypeEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/MainGridColumnsEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/MainGridColumnsEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/SubSectionEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/SubSectionEditor.html', parameters, settings);
        }

        function addGridAction(onGridActionAdded, gridActions) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridActionAdded = onGridActionAdded;
            };
            var parameters = {
                gridActions: gridActions
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGridActionEditor.html', parameters, settings);
        }

        function editGridAction(actionEntity, onGridActionUpdated, gridActions) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridActionUpdated = onGridActionUpdated;
            };
            var parameters = {
                actionEntity: actionEntity,
                gridActions: gridActions
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGridActionEditor.html', parameters, settings);
        }

        function addDataSource(onDataSourceAdded, dataSources) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDataSourceAdded = onDataSourceAdded;
            };
            var parameters = {
                dataSources: dataSources
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCDataSourceEditor.html', parameters, settings);
        }

        function editDataSource(dataSourceEntity, onDataSourceUpdated, dataSources) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDataSourceUpdated = onDataSourceUpdated;
            };
            var parameters = {
                dataSourceEntity: dataSourceEntity,
                dataSources: dataSources
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCDataSourceEditor.html', parameters, settings);
        }

        function addParameter(onParameterAdded, parametersObj) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onParameterAdded = onParameterAdded;
            };
            var parameters = {
                parametersObj: parametersObj
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCParameterEditor.html', parameters, settings);
        }

        function editParameter(parameterEntity, onParameterUpdated, parametersObj) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onParameterUpdated = onParameterUpdated;
            };
            var parameters = {
                parameterEntity: parameterEntity,
                parameters: parametersObj
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCParameterEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/SubSectionGridColumnEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/SubSectionGridColumnEditor.html', parameters, settings);
        }

        function addSubReport(onSubReportAdded, subReports) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubReportAdded = onSubReportAdded;
            };
            var parameters = {
                subReports: subReports
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCSubReportEditor.html', parameters, settings);
        }

        function editSubReport(subReportEntity, onSubReportUpdated, subReports) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubReportUpdated = onSubReportUpdated;
            };
            var parameters = {
                subReportEntity: subReportEntity,
                subReports: subReports
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCSubReportEditor.html', parameters, settings);
        }

        return ({
            addInvoiceType: addInvoiceType,
            editInvoiceType: editInvoiceType,
            addGridColumn: addGridColumn,
            editGridColumn: editGridColumn,
            addSubSection: addSubSection,
            editSubSection: editSubSection,
            addGridAction: addGridAction,
            editGridAction: editGridAction,
            addDataSource: addDataSource,
            editDataSource: editDataSource,
            addParameter: addParameter,
            editParameter: editParameter,
            addSubSectionGridColumn: addSubSectionGridColumn,
            editSubSectionGridColumn: editSubSectionGridColumn,
            addSubReport: addSubReport,
            editSubReport: editSubReport

        });
    }]);
