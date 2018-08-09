"use strict";

app.directive("vrWhsDealSwapdealinboundGrid", ["UtilsService", "VRNotificationService", "WhS_Deal_SwapDealInboundService",
    function (UtilsService, VRNotificationService, WhS_Deal_SwapDealInboundService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var swapDealInboundGrid = new SwapDealInboundGrid($scope, ctrl, $attrs);
                swapDealInboundGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/Templates/SwapDealInboundGridTemplate.html'

        };

        function SwapDealInboundGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var gridApi;
            var mainPayload;
            var lastInboundGroupNumber = 0;
            var context;
            var carrierAccountId;
            var dealId;

            function initializeController() {

                ctrl.datasource = [];

                ctrl.addSwapDealInbound = function () {
                    var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;
                    var onSwapDealInboundAdded = function (addedSwapDealInbound) {
                        lastInboundGroupNumber = lastInboundGroupNumber + 1;
                        addedSwapDealInbound.ZoneGroupNumber = lastInboundGroupNumber;
                        ctrl.datasource.push(addedSwapDealInbound);
                    };
                    WhS_Deal_SwapDealInboundService.addSwapDealInbound(onSwapDealInboundAdded, sellingNumberPlanId, context, carrierAccountId, dealId);
                };

                $scope.onGridReady = function (api) {
                    gridApi = api;
                    defineAPI();
                };
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    carrierAccountId = mainPayload.carrierAccountId;
                    dealId = mainPayload.dealId;
                    if (mainPayload != undefined)
                        context = mainPayload.context;
                    if (payload != undefined && payload.Inbounds != undefined) {
                        ctrl.datasource = payload.Inbounds;
                    }
                    else {
                        ctrl.datasource.length = 0;
                    }

                    if (payload != undefined && payload.lastInboundGroupNumber != undefined) {
                        lastInboundGroupNumber = payload.lastInboundGroupNumber;
                    }

                };

                api.getData = function () {
                    var inbounds = [];
                    if (ctrl.datasource != undefined) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            inbounds.push({
                                Name: currentItem.Name,
                                Volume: currentItem.Volume,
                                Rate: currentItem.Rate,
                                ExtraVolumeRate: currentItem.ExtraVolumeRate,
                                SaleZones: currentItem.SaleZones,
                                CountryIds: currentItem.CountryIds,
                                ZoneGroupNumber: currentItem.ZoneGroupNumber,
                                SubstituteRateType: currentItem.SubstituteRateType,
                                FixedRate: currentItem.FixedRate
                            });
                        }
                    }
                    return { inbounds: inbounds, lastInboundGroupNumber: lastInboundGroupNumber };
                };

                api.hasData = function () {
                    return ctrl.datasource.length > 0;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions;
                if (UtilsService.isContextReadOnly($scope)) {
                    defaultMenuActions = [
                       {
                           name: "View",
                           clicked: viewSwapDealInbound
                       }];
                }
                else {
                    defaultMenuActions = [
                        {
                            name: "Edit",
                            clicked: editSwapDealInbound
                        },
                        {
                            name: "Delete",
                            clicked: deleteSwapDealInbound
                        }];
                }
                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editSwapDealInbound(dealInboundObj) {
                var onDealInboundUpdated = function (dealInbound) {
                    dealInbound.ZoneGroupNumber = dealInboundObj.ZoneGroupNumber;
                    var index = ctrl.datasource.indexOf(dealInboundObj);
                    ctrl.datasource[index] = dealInbound;
                };
                var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;

                WhS_Deal_SwapDealInboundService.editSwapDealInbound(dealInboundObj, sellingNumberPlanId, onDealInboundUpdated, context, carrierAccountId,dealId);
            }

            function viewSwapDealInbound(dealInboundObj) {

                var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;

                WhS_Deal_SwapDealInboundService.viewSwapDealInbound(dealInboundObj, sellingNumberPlanId, context);
            }

            function deleteSwapDealInbound(swapDealInboundObj) {
                var onSwapDealInboundDeleted = function () {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, swapDealInboundObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                WhS_Deal_SwapDealInboundService.deleteSwapDealInbound($scope, swapDealInboundObj, onSwapDealInboundDeleted);
            }
        }

        return directiveDefinitionObject;
    }
]);