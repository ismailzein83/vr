"use strict";
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

        function addInvoiceItemSubSection(onInvoiceItemSubSectionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceItemSubSectionAdded = onInvoiceItemSubSectionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceSubSectionSettings/InvoiceItemSubSectionEditor.html', parameters, settings);
        }

        function editInvoiceItemSubSection(subSectionEntity, onInvoiceItemSubSectionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceItemSubSectionUpdated = onInvoiceItemSubSectionUpdated;
            };
            var parameters = {
                subSectionEntity: subSectionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceSubSectionSettings/InvoiceItemSubSectionEditor.html', parameters, settings);
        }

        function addDataSource(onDataSourceAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDataSourceAdded = onDataSourceAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceDataSourceEditor.html', parameters, settings);
        }

        function editDataSource(dataSourceEntity, onDataSourceUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDataSourceUpdated = onDataSourceUpdated;
            };
            var parameters = {
                dataSourceEntity: dataSourceEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceDataSourceEditor.html', parameters, settings);
        }

        function addParameter(onParameterAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onParameterAdded = onParameterAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCParameterEditor.html', parameters, settings);
        }

        function editParameter(parameterEntity, onParameterUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onParameterUpdated = onParameterUpdated;
            };
            var parameters = {
                parameterEntity: parameterEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCParameterEditor.html', parameters, settings);
        }

        function addSubReport(onSubReportAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubReportAdded = onSubReportAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCSubReportEditor.html', parameters, settings);
        }

        function editSubReport(subReportEntity, onSubReportUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubReportUpdated = onSubReportUpdated;
            };
            var parameters = {
                subReportEntity: subReportEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/RDLCSubReportEditor.html', parameters, settings);
        }

        return ({
            addInvoiceType: addInvoiceType,
            editInvoiceType: editInvoiceType,
            addDataSource: addDataSource,
            editDataSource: editDataSource,
            addParameter: addParameter,
            editParameter: editParameter,
            addSubReport: addSubReport,
            editSubReport: editSubReport,
            addInvoiceItemSubSection: addInvoiceItemSubSection,
            editInvoiceItemSubSection: editInvoiceItemSubSection,
        });
    }]);
