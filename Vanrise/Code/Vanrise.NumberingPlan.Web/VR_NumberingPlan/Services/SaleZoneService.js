(function (appControllers) {

    'use strict';

    SaleZoneService.$inject = ['Vr_NP_SellingNumberPlanService', 'VRModalService'];

    function SaleZoneService(Vr_NP_SellingNumberPlanService, VRModalService) {
        return ({
            registerDrillDownToSellingNumberPlan: registerDrillDownToSellingNumberPlan
        });

       
        function registerDrillDownToSellingNumberPlan() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Sale Zones";
            drillDownDefinition.directive = "vr-np-saleZone-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, sellingNumberPlanItem) {

                sellingNumberPlanItem.saleZoneGridAPI = directiveAPI;
                var query = {
                    SellingNumberId: sellingNumberPlanItem.Entity.SellingNumberPlanId,
                    EffectiveOn: new Date()
                };

                return sellingNumberPlanItem.saleZoneGridAPI.loadGrid(query);
            };

            Vr_NP_SellingNumberPlanService.addDrillDownDefinition(drillDownDefinition);
        }
    }

    appControllers.service('Vr_NP_SaleZoneService', SaleZoneService);

})(appControllers);
