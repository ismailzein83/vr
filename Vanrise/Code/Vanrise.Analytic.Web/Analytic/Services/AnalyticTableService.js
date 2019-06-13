(function (appControllers) {

    'use strict';

    AnalyticTableService.$inject = ['VRModalService'];

    function AnalyticTableService(VRModalService) {
        var drillDownDefinitions = [];
        return ({
            addAnalyticTable: addAnalyticTable,
            editAnalyticTable: editAnalyticTable,
            getEntityUniqueName: getEntityUniqueName,
            openMeasureStyles: openMeasureStyles,
            viewMeasureStyles: viewMeasureStyles,
            openPermanentFilter: openPermanentFilter,
            addConditionFilterItem: addConditionFilterItem,
            editConditionFilterItem: editConditionFilterItem
        });

        function addAnalyticTable(onAnalyticTableAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticTableAdded = onAnalyticTableAdded;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticTableEditor.html', null, modalSettings);
        }

        function editAnalyticTable(tableId, onAnalyticTableUpdated) {
            var modalParameters = {
                tableId: tableId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticTableUpdated = onAnalyticTableUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticTableEditor.html', modalParameters, modalSettings);
        }

        function openMeasureStyles(analyticTableId) {
            var modalParameters = {
                analyticTableId: analyticTableId
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticMeasureStyleEditor.html', modalParameters, null);
        }
        
        function viewMeasureStyles(analyticTableId,statusBeDefinitionId,highlightedId) {
            var modalParameters = {
                analyticTableId: analyticTableId,
                statusBeDefinitionId: statusBeDefinitionId,
                highlightedId: highlightedId
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Runtime/GenericAnalyticMeasureStyleViewer.html', modalParameters, null);
        }
        function openPermanentFilter(analyticTableId) {
            var modalParameters = {
                analyticTableId: analyticTableId
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticPermanentFilterEditor.html', modalParameters, null);
        }
        function getEntityUniqueName() {
            return "VR_Analytic_AnalyticTable";
        }
        function addConditionFilterItem(analyticTableId, onConditionFilterItemAdded) {
            var modalParameters = {
                analyticTableId: analyticTableId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConditionFilterItemAdded = onConditionFilterItemAdded;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticPermanentFilterConditionFilterItemEditor.html', modalParameters, modalSettings);
        }
        function editConditionFilterItem(analyticTableId, entity, onConditionFilterItemUpdated) {
            var modalParameters = {
                analyticTableId: analyticTableId,
                entity: entity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConditionFilterItemUpdated = onConditionFilterItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticPermanentFilterConditionFilterItemEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_Analytic_AnalyticTableService', AnalyticTableService);

})(appControllers);
