(function (appControllers) {

    'use strict';

    AnalyticTableService.$inject = ['VRModalService'];

    function AnalyticTableService(VRModalService) {
        return ({
            addAnalyticTable: addAnalyticTable,
            editAnalyticTable: editAnalyticTable,
        });

        function addAnalyticTable(onAnalyticTableAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticTableAdded = onAnalyticTableAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Table/TableEditor.html', null, modalSettings);
        }

        function editAnalyticTable(userId, onAnalyticTableUpdated) {
            var modalParameters = {
                userId: userId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticTableUpdated = onAnalyticTableUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Table/TableEditor.html', modalParameters, modalSettings);
        }

    };

    appControllers.service('VR_Analytic_AnalyticTableService', AnalyticTableService);

})(appControllers);
