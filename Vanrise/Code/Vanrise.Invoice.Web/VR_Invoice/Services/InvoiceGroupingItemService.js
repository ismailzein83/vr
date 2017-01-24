"use strict";
app.service('VR_Invoice_InvoiceItemGroupingService', ['VRModalService',
    function (VRModalService) {

        function addItemGrouping(onItemGroupingAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onItemGroupingAdded = onItemGroupingAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/ItemGroupingEditor.html', null, settings);
        }
        function editItemGrouping(onItemGroupingUpdated, itemGroupingEntity) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onItemGroupingUpdated = onItemGroupingUpdated;
            };
            var parameters = {
                itemGroupingEntity: itemGroupingEntity
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/ItemGroupingEditor.html', parameters, settings);
        }

        function addItemGroupingDimension(onDimensionItemGroupingAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDimensionItemGroupingAdded = onDimensionItemGroupingAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/DimensionItemGroupingEditor.html', null, settings);
        }
        function editItemGroupingDimension(onDimensionItemGroupingUpdated, dimensionEntity) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDimensionItemGroupingUpdated = onDimensionItemGroupingUpdated;
            };
            var parameters = {
                dimensionEntity: dimensionEntity
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/DimensionItemGroupingEditor.html', parameters, settings);
        }

        function addItemGroupingAggregate(onAggregateItemGroupingAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAggregateItemGroupingAdded = onAggregateItemGroupingAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/AggregateItemGroupingEditor.html', null, settings);
        }
        function editItemGroupingAggregate(onAggregateItemGroupingUpdated, aggregateEntity) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAggregateItemGroupingUpdated = onAggregateItemGroupingUpdated;
            };
            var parameters = {
                aggregateEntity: aggregateEntity
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/AggregateItemGroupingEditor.html', parameters, settings);
        }


        return ({
            addItemGrouping: addItemGrouping,
            editItemGrouping: editItemGrouping,
            addItemGroupingDimension: addItemGroupingDimension,
            editItemGroupingDimension: editItemGroupingDimension,
            addItemGroupingAggregate: addItemGroupingAggregate,
            editItemGroupingAggregate: editItemGroupingAggregate
        });
    }]);
