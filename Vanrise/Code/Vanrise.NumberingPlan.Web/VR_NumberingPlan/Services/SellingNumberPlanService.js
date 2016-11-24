(function (appControllers) {

    'use strict';

    SellingNumberPlanService.$inject = ['VRModalService'];

    function SellingNumberPlanService(VRModalService) {
        var drillDownDefinitions = [];

        return ({
            addSellingNumberPlan: addSellingNumberPlan,
            editSellingNumberPlan: editSellingNumberPlan,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        });

        function addSellingNumberPlan(onSellingNumberPlanAdded) {
            var settings = {

            };
            
            settings.onScopeReady = function (modalScope) {
                modalScope.onSellingNumberPlanAdded = onSellingNumberPlanAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/VR_NumberingPlan/Views/SellingNumberPlanEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_NumberingPlan/Views/SellingNumberPlanEditor.html', parameters, settings);
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service("Vr_NP_SellingNumberPlanService", SellingNumberPlanService);

})(appControllers);
