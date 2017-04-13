(function (appControllers) {

    'use strict';

    SellingNumberPlanService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function SellingNumberPlanService(VRModalService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];

        return ({
            addSellingNumberPlan: addSellingNumberPlan,
            editSellingNumberPlan: editSellingNumberPlan,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            getEntityUniqueName: getEntityUniqueName,
            registerHistoryViewAction: registerHistoryViewAction
        });

        function addSellingNumberPlan(onSellingNumberPlanAdded) {
            var settings = {

            };
            
            settings.onScopeReady = function (modalScope) {
                modalScope.onSellingNumberPlanAdded = onSellingNumberPlanAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanEditor.html', parameters, settings);
        }

        function editSellingNumberPlan(SellingNumberPlanId, onSellingNumberPlanUpdated) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSellingNumberPlanUpdated = onSellingNumberPlanUpdated;
            };
            var parameters = {
                SellingNumberPlanId: SellingNumberPlanId
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanEditor.html', parameters, settings);
        }
        function viewHistorySellingNumberPlan(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanEditor.html', modalParameters, modalSettings);
        };
        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_BusinessEntity_SellingNumberPlan_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistorySellingNumberPlan(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function getEntityUniqueName() {
            return "WhS_BusinessEntity_SellingNumberPlan";
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('WhS_BE_SellingNumberPlanService', SellingNumberPlanService);

})(appControllers);
