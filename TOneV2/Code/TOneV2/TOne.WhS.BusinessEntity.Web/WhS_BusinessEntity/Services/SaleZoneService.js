(function (appControllers) {

    'use stict';

    SaleZoneService.$inject = ['WhS_BE_SellingNumberPlanService', 'VRModalService'];

    function SaleZoneService(WhS_BE_SellingNumberPlanService, VRModalService) {
        return ({
            addSaleZone: addSaleZone,
            editSaleZone: editSaleZone,
            registerDrillDownToSellingNumberPlan: registerDrillDownToSellingNumberPlan
        });

        function addSaleZone(onSaleZoneAdded, sellingNumberPlanId) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSaleZoneAdded = onSaleZoneAdded;
            };
            var parameters = {};
            if (sellingNumberPlanId != undefined) {
                parameters.sellingNumberPlanId = sellingNumberPlanId;
            }

            VRModalService.showModal('/Client/Modules/Common/Views/SaleZone/SaleZoneEditor.html', parameters, settings);
        }

        function editSaleZone(saleZoneId, onSaleZoneUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSaleZoneUpdated = onSaleZoneUpdated;
            };
            var parameters = {
                SaleZoneId: saleZoneId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/SaleZone/SaleZoneEditor.html', parameters, settings);
        }

        function registerDrillDownToSellingNumberPlan() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Sale Zones";
            drillDownDefinition.directive = "vr-whs-be-saleZone-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, sellingNumberPlanItem) {

                sellingNumberPlanItem.saleZoneGridAPI = directiveAPI;
                var query = {
                    SellingNumberId: sellingNumberPlanItem.Entity.SellingNumberPlanId,
                    EffectiveOn: new Date()
                };

                return sellingNumberPlanItem.saleZoneGridAPI.loadGrid(query);
            };

            WhS_BE_SellingNumberPlanService.addDrillDownDefinition(drillDownDefinition);
        }
    }

    appControllers.service('WhS_BE_SaleZoneService', SaleZoneService);

})(appControllers);
