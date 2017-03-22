(function (appControllers) {

    'use strict';

    SellingNumberPlanService.$inject = ['VRModalService'];

    function SellingNumberPlanService(VRModalService) {
        var drillDownDefinitions = [];

        return ({
            addSellingNumberPlan: addSellingNumberPlan,
            editSellingNumberPlan: editSellingNumberPlan,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            getEntityUniqueName: getEntityUniqueName
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
