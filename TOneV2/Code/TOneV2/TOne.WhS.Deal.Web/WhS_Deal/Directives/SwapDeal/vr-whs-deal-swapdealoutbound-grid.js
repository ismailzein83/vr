"use strict";

app.directive("vrWhsDealSwapdealoutboundGrid", ["UtilsService", "VRNotificationService", "WhS_Deal_SwapDealOutboundService",
    function (UtilsService, VRNotificationService, WhS_Deal_SwapDealOutboundService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var swapDealOutboundGrid = new SwapDealOutboundGrid($scope, ctrl, $attrs);
                swapDealOutboundGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/Templates/SwapDealOutboundGridTemplate.html'

        };

        function SwapDealOutboundGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;
            var lastOutboundGroupNumber = 0;
            var context;
            function initializeController() {

                ctrl.datasource = [];

                ctrl.addSwapDealOutbound = function () {
                    var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;
                    var onSwapDealOutboundAdded = function (addedSwapDealOutbound) {
                        lastOutboundGroupNumber = lastOutboundGroupNumber + 1;
                        addedSwapDealOutbound.ZoneGroupNumber = lastOutboundGroupNumber;
                        ctrl.datasource.push(addedSwapDealOutbound);
                    };
                    WhS_Deal_SwapDealOutboundService.addSwapDealOutbound(onSwapDealOutboundAdded, supplierId,context);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    if (mainPayload != undefined)
                        context = mainPayload.context;
                    if (payload != undefined && payload.Outbounds != undefined) {
                        ctrl.datasource = payload.Outbounds;
                    }
                    else {
                        ctrl.datasource.length = 0;
                    }
                    if (payload != undefined && payload.lastOutboundGroupNumber != undefined) {
                        lastOutboundGroupNumber = payload.lastOutboundGroupNumber;
                    }
                };

                api.getData = function () {
                    var outbounds = [];
                    if (ctrl.datasource != undefined) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            outbounds.push({
                                Name: currentItem.Name,
                                Volume: currentItem.Volume,
                                Rate: currentItem.Rate,
                                ExtraVolumeEvaluatedRate: currentItem.ExtraVolumeEvaluatedRate,
                                EvaluatedRate: currentItem.EvaluatedRate,
                                SupplierZones: currentItem.SupplierZones,
                                CountryId: currentItem.CountryId,
                                ZoneGroupNumber: currentItem.ZoneGroupNumber
                            });
                        }
                    }
                    return { outbounds: outbounds, lastOutboundGroupNumber: lastOutboundGroupNumber };
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
                           clicked: viewSwapDealOutbound
                       }];
                }
                else {
                    defaultMenuActions = [
                        {
                            name: "Edit",
                            clicked: editSwapDealOutbound
                        },
                        {
                            name: "Delete",
                            clicked: deleteSwapDealOutbound
                        }];
                }
                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editSwapDealOutbound(swapDealOutboundObj) {
                var onSwapDealOutboundUpdated = function (swapDealOutbound) {
                    swapDealOutbound.ZoneGroupNumber = swapDealOutboundObj.ZoneGroupNumber;
                    var index = ctrl.datasource.indexOf(swapDealOutboundObj);
                    ctrl.datasource[index] = swapDealOutbound;
                };
                var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;

                WhS_Deal_SwapDealOutboundService.editSwapDealOutbound(swapDealOutboundObj, supplierId, onSwapDealOutboundUpdated,context);

            }

            function deleteSwapDealOutbound(swapDealOutboundObj) {
                var onSwapDealOutboundDeleted = function () {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, swapDealOutboundObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                WhS_Deal_SwapDealOutboundService.deleteSwapDealOutbound($scope, swapDealOutboundObj, onSwapDealOutboundDeleted);
            }
            function viewSwapDealOutbound(dealInboundObj) {

                var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;

                WhS_Deal_SwapDealOutboundService.viewSwapDealOutbound(dealInboundObj, sellingNumberPlanId);
            }

        }

        return directiveDefinitionObject;

    }
]);