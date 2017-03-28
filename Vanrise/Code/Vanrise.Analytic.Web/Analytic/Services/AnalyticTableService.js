(function (appControllers) {

    'use strict';

    AnalyticTableService.$inject = ['VRModalService'];

    function AnalyticTableService(VRModalService) {
        var drillDownDefinitions = [];
        return ({
            addAnalyticTable: addAnalyticTable,
            editAnalyticTable: editAnalyticTable,
            getEntityUniqueName: getEntityUniqueName
            
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
        function getEntityUniqueName() {
            return "VR_Analytic_AnalyticTable";
        }

       
        

    };

    appControllers.service('VR_Analytic_AnalyticTableService', AnalyticTableService);

})(appControllers);
