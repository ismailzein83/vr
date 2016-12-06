"use strict";
app.service('VR_Invoice_InvoiceGroupingItemService', ['VRModalService',
    function (VRModalService) {

        function addGroupingItem(onGroupingItemAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGroupingItemAdded = onGroupingItemAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGroupingItem/GroupingItemEditor.html', null, settings);
        }
        function editGroupingItem(onGroupingItemUpdated, groupingItemEntity) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGroupingItemUpdated = onGroupingItemUpdated;
            };
            var parameters = {
                groupingItemEntity: groupingItemEntity
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGroupingItem/GroupingItemEditor.html', parameters, settings);
        }

        function addGroupingItemDimension(onDimensionGroupingItemAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDimensionGroupingItemAdded = onDimensionGroupingItemAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGroupingItem/DimensionGroupingItemEditor.html', null, settings);
        }
        function editGroupingItemDimension(onDimensionGroupingItemUpdated, dimensionEntity) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDimensionGroupingItemUpdated = onDimensionGroupingItemUpdated;
            };
            var parameters = {
                dimensionEntity: dimensionEntity
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGroupingItem/DimensionGroupingItemEditor.html', parameters, settings);
        }

        function addGroupingItemAggregate(onAggregateGroupingItemAdded) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAggregateGroupingItemAdded = onAggregateGroupingItemAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGroupingItem/AggregateGroupingItemEditor.html', null, settings);
        }
        function editGroupingItemAggregate(onAggregateGroupingIteUpdated, aggregateEntity) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAggregateGroupingIteUpdated = onAggregateGroupingIteUpdated;
            };
            var parameters = {
                aggregateEntity: aggregateEntity
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceGroupingItem/AggregateGroupingItemEditor.html', parameters, settings);
        }
        return ({
            addGroupingItem: addGroupingItem,
            editGroupingItem: editGroupingItem,
            addGroupingItemDimension: addGroupingItemDimension,
            editGroupingItemDimension: editGroupingItemDimension,
            addGroupingItemAggregate: addGroupingItemAggregate,
            editGroupingItemAggregate: editGroupingItemAggregate
        });
    }]);
