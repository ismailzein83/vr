(function (appControllers) {

    'use strict';

    AnalyticTableQuerySettingsSubtablesService.$inject = ['VRModalService'];

    function AnalyticTableQuerySettingsSubtablesService(VRModalService) {
        var drillDownDefinitions = [];
        return ({
            addSubtable: addSubtable,
            editSubtable: editSubtable,

        });

        function addSubtable(analyticTableId, onAnalyticSubtableAdded, context) {
            var modalParameters = {
                analyticTableId: analyticTableId,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticSubtableAdded = onAnalyticSubtableAdded;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/AnalyticSubtableEditor.html', modalParameters, modalSettings);
        }

        function editSubtable(analyticTableId, subtableEntity, onAnalyticSubtableUpdated, context) {
            var modalParameters = {
                analyticTableId: analyticTableId,
                subtableEntity: subtableEntity,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticSubtableUpdated = onAnalyticSubtableUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/AnalyticSubtableEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_Analytic_AnalyticTableQuerySettingsSubtablesService', AnalyticTableQuerySettingsSubtablesService);

})(appControllers);
