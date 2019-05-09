﻿"use strict";
app.service('VR_Invoice_InvoiceTypeService', ['VRModalService', 'VRCommon_ObjectTrackingService',
    function (VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
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


        function addGroupItemSubSection(onItemGroupingSubSectionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onItemGroupingSubSectionAdded = onItemGroupingSubSectionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSubSectionSettings/Templates/ItemGroupingSubSectionEditor.html', parameters, settings);
        }

        function editGroupItemSubSection(subSectionEntity, onItemGroupingSubSectionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onItemGroupingSubSectionUpdated = onItemGroupingSubSectionUpdated;
            };
            var parameters = {
                subSectionEntity: subSectionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSubSectionSettings/Templates/ItemGroupingSubSectionEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSubSectionSettings/Templates/InvoiceItemSubSectionEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSubSectionSettings/Templates/InvoiceItemSubSectionEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceDataSourceSettings/Templates/InvoiceDataSourceEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceDataSourceSettings/Templates/InvoiceDataSourceEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/Templates/RDLCParameterEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/Templates/RDLCParameterEditor.html', parameters, settings);
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/Templates/RDLCSubReportEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/Templates/RDLCSubReportEditor.html', parameters, settings);
        }





        function addInvoiceSettingSection(onSectionAdded, exitingSections) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSectionAdded = onSectionAdded;
            };

            var parameters = {
                exitingSections: exitingSections
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSettingDefinition/Templates/InvoiceSettingDefinitionSectionEditor.html', parameters, modalSettings);
        }

        function editInvoiceSettingSection(onSectionUpdated, exitingSections, sectionEntity) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSectionUpdated = onSectionUpdated;
            };

            var parameters = {
                sectionEntity: sectionEntity,
                exitingSections: exitingSections
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSettingDefinition/Templates/InvoiceSettingDefinitionSectionEditor.html', parameters, modalSettings);
        }

        function addInvoiceSettingPart(onRowAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowAdded = onRowAdded;
            };

            var parameters = {
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSettingDefinition/Templates/InvoiceSettingDefinitionPartEditor.html', parameters, modalSettings);
        }

        function editInvoiceSettingPart(onRowUpdated, rowEntity) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowUpdated = onRowUpdated;
            };

            var parameters = {
                rowEntity: rowEntity
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSettingDefinition/Templates/InvoiceSettingDefinitionPartEditor.html', parameters, modalSettings);
        }
        function getEntityUniqueName() {
            return "VR_Invoice_InvoiceType";
        }
        function registerObjectTrackingDrillDownToInvoiceType() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, invoiceTypeItem) {
                invoiceTypeItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: invoiceTypeItem.Entity.InvoiceTypeId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return invoiceTypeItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }


        function addItemSetNameStorageRule(onItemSetNameStorageRuleAdded, context) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onItemSetNameStorageRuleAdded = onItemSetNameStorageRuleAdded;
            };

            var parameters = {
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/ItemSetNameStorageRule/Templates/ItemSetNameStorageRuleEditor.html', parameters, modalSettings);
        }

        function editItemSetNameStorageRule(itemSetNameStorageRule, onItemSetNameStorageRuleUpdated, context) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onItemSetNameStorageRuleUpdated = onItemSetNameStorageRuleUpdated;
            };

            var parameters = {
                itemSetNameStorageRule: itemSetNameStorageRule,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/ItemSetNameStorageRule/Templates/ItemSetNameStorageRuleEditor.html', parameters, modalSettings);
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
            editGroupItemSubSection: editGroupItemSubSection,
            addGroupItemSubSection: addGroupItemSubSection,

            addInvoiceSettingSection: addInvoiceSettingSection,
            editInvoiceSettingSection:editInvoiceSettingSection,
            addInvoiceSettingPart: addInvoiceSettingPart,
            editInvoiceSettingPart: editInvoiceSettingPart,
            registerObjectTrackingDrillDownToInvoiceType: registerObjectTrackingDrillDownToInvoiceType,
            getDrillDownDefinition: getDrillDownDefinition,
            editItemSetNameStorageRule: editItemSetNameStorageRule,
            addItemSetNameStorageRule: addItemSetNameStorageRule
        });
    }]);
