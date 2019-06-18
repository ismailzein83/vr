'use strict';

app.directive('vrWhsDealSwapdealanalysisInboundManagement', ['WhS_Deal_SwapDealAnalysisService', 'UtilsService', function (WhS_Deal_SwapDealAnalysisService, UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var swapDealAnalysisInboundMangement = new SwapDealAnalysisInboundMangement($scope, ctrl, $attrs);
            swapDealAnalysisInboundMangement.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisInboundManagementTemplate.html'
    };

    function SwapDealAnalysisInboundMangement($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var context;

        var settings;
        var sellingNumberPlanId;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.inbounds = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.addInbound = function () {
                addInbound();
            };

            $scope.scopeModel.removeInbound = function (inbound) {
                $scope.scopeModel.inbounds.splice($scope.scopeModel.inbounds.indexOf(inbound), 1);
            };

            $scope.scopeModel.validateInbounds = function () {
                if ($scope.scopeModel.inbounds.length == 0)
                    return 'Add at least 1 inbound';
                return null;
            };

            defineMenuActions();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                $scope.scopeModel.inbounds.length = 0;
                var inbounds;
                var summary;
                if (payload != undefined) {
                    context = payload.context;
                    settings = payload.settings;
                    inbounds = payload.Inbounds;
                    summary = payload.Summary;
                }

                if (inbounds != undefined) {
                    for (var i = 0; i < inbounds.length; i++) {
                        var inbound = inbounds[i];
                        var entity =
                        {
                            GroupName: inbound.GroupName,
                            Volume: inbound.Volume,
                            DailyVolume: inbound.DailyVolume,
                            DealRate: inbound.DealRate,
                            CurrentRate: inbound.CurrentRate,
                            RateProfit: inbound.RateProfit,
                            Profit: inbound.Profit,
                            Revenue: inbound.Revenue,
                            CountryId: inbound.CountryId,
                            ItemCalculationMethod: inbound.ItemCalculationMethod,
                            CalculationMethodId: inbound.CalculationMethodId,
                            SaleZoneIds: inbound.SaleZoneIds
                        };

                        $scope.scopeModel.inbounds.push({ Entity: entity });
                    }
                }
                if (summary != undefined) {
                    gridAPI.setSummary({
                        TotalSaleMargin: summary.TotalSaleMargin,
                        TotalSaleRevenue: summary.TotalSaleRevenue
                    });
                }
            };

            api.getData = function () {
                return getInboundEntities();
            };

            api.setSellingNumberPlanId = function (carrierSellingNumberPlanId) {
                if (carrierSellingNumberPlanId != undefined)
                    sellingNumberPlanId = carrierSellingNumberPlanId;
            };

            api.clear = function () {
                clearSummary();
                $scope.scopeModel.inbounds.length = 0;
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions = [{
                name: 'Edit',
                clicked: editInbound
            }];
        }
        function addInbound() {
            var carrierAccountId = context.settingsAPI.getCarrierAccountId();
            var onInboundAdded = function (addedInbound) {
                var obj = {
                    Entity: addedInbound
                };
                $scope.scopeModel.inbounds.push(obj);
                clearCalclulatedFields();
            };
            WhS_Deal_SwapDealAnalysisService.addInbound(settings, carrierAccountId, sellingNumberPlanId, onInboundAdded);
        }
        function editInbound(inboundEntity) {
            var carrierAccountId = context.settingsAPI.getCarrierAccountId();
            var onInboundUpdated = function (updatedInbound) {
                var obj = { Entity: updatedInbound };
                $scope.scopeModel.inbounds[$scope.scopeModel.inbounds.indexOf(inboundEntity)] = obj;
                clearCalclulatedFields();
            };
            WhS_Deal_SwapDealAnalysisService.editInbound(settings, carrierAccountId, sellingNumberPlanId, inboundEntity.Entity, onInboundUpdated);
        }

        function getInboundEntities() {
            var inbounds = [];
            for (var i = 0; i < $scope.scopeModel.inbounds.length; i++) {
                var inbounObject = $scope.scopeModel.inbounds[i].Entity;
                var inboundSetting =
                {
                    $type: "TOne.WhS.Deal.Entities.Inbound,TOne.WhS.Deal.Entities",
                    CountryId: inbounObject.CountryId,
                    CurrentRate: inbounObject.CurrentRate,
                    DailyVolume: inbounObject.DailyVolume,
                    Volume: inbounObject.Volume,
                    DealRate: inbounObject.DealRate,
                    ItemCalculationMethod: inbounObject.ItemCalculationMethod,
                    CalculationMethodId: inbounObject.CalculationMethodId,
                    GroupName: inbounObject.GroupName,
                    SaleZoneIds: inbounObject.SaleZoneIds
                };
                inbounds.push(inboundSetting);
            }
            var InboundCollection =
            {
                $type: "TOne.WhS.Deal.Entities.InboundCollection,TOne.WhS.Deal.Entities",
                $values: inbounds
            };
            return InboundCollection;
        }
        function clearCalclulatedFields() {
            for (var i = 0; i < $scope.scopeModel.inbounds.length; i++) {
                var updatedEntity = UtilsService.cloneObject($scope.scopeModel.inbounds[i].Entity);
                updatedEntity.DailyVolume = undefined;
                updatedEntity.CurrentRate = undefined;
                updatedEntity.RateProfit = undefined;
                updatedEntity.Profit = undefined;
                updatedEntity.Revenue = undefined;
                $scope.scopeModel.inbounds[i] = { Entity: updatedEntity };
            }
            clearSummary();
            context.clearResult();
        }
        function clearSummary() {
            gridAPI.clearSummary();
        }
    }
}]);