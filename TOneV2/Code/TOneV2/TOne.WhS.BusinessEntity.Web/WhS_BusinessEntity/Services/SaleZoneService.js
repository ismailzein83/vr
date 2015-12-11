
app.service('WhS_BE_SaleZoneService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'WhS_BE_SellingNumberPlanService',
    function (VRModalService, VRNotificationService, UtilsService, WhS_BE_SellingNumberPlanService) {
        return ({
            editSaleZone: editSaleZone,
            addSaleZone: addSaleZone,
            registerDrillDownToSellingNumberPlan: registerDrillDownToSellingNumberPlan
        });
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

        function registerDrillDownToSellingNumberPlan() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Sale Zones";
            drillDownDefinition.directive = "vr-whs-be-saleZone-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "New Sale Zone",
                clicked: function (sellingNumberPlanItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(sellingNumberPlanItem);
                    var query = {
                        SellingNumberPlanIds: [sellingNumberPlanItem.Entity.SellingNumberPlanId]
                    }
                    var onSaleZoneAdded = function (saleZoneObj) {
                        if (sellingNumberPlanItem.saleZoneGridAPI != undefined) {
                            sellingNumberPlanItem.saleZoneGridAPI.onSaleZoneAdded(saleZoneObj);
                        }
                    };
                    addSaleZone(onSaleZoneAdded, sellingNumberPlanItem.Entity.sellingNumberPlanId);
                }
            }];

            drillDownDefinition.loadDirective = function (directiveAPI, sellingNumberPlanItem) {

                sellingNumberPlanItem.saleZoneGridAPI = directiveAPI;
                var query = {
                    SellingNumber: sellingNumberPlanItem.Entity.SellingNumberPlanId
                };

                return sellingNumberPlanItem.saleZoneGridAPI.loadGrid(query);
            };

            WhS_BE_SellingNumberPlanService.addDrillDownDefinition(drillDownDefinition);
        }


    }]);
