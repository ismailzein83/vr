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
            var mainPayload;
            function initializeController() {

                ctrl.datasource = [];

                ctrl.addSwapDealInbound = function () {


                    var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;
                    var onSwapDealInboundAdded = function (addedSwapDealInbound) {
                        ctrl.datasource.push(addedSwapDealInbound);
                    };
                    WhS_Deal_SwapDealInboundService.addSwapDealInbound(onSwapDealInboundAdded, sellingNumberPlanId);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    
                    if (payload.InboundParts != undefined && payload.InboundParts != null) {

                        ctrl.datasource = payload.InboundParts;
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            ctrl.datasource[i].Amount = ctrl.datasource[i].Volume * ctrl.datasource[i].Rate;
                        }
                    }
                        
                    else
                        ctrl.datasource.length = 0;
                };

                api.getData = function () {
                    var sellingObj = 
                    {
                        sellingParts: []
                    }

                    if (ctrl.datasource != undefined) {
                        sellingObj.sellingParts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            sellingObj.sellingParts.push({
                                Name: currentItem.Name,
                                Volume: currentItem.Volume,
                                Rate: currentItem.Rate,
                                SaleZoneIds: currentItem.SaleZoneIds
                            });
                           
                        }

                    }
                    return sellingObj;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSwapDealInbound
                },
                {
                    name: "Delete",
                    clicked: deleteSwapDealInbound
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editSwapDealInbound(dealInboundObj) {
                var onDealInboundUpdated = function (dealInbound) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dealInboundObj.Name, 'Name');
                    ctrl.datasource[index] = dealInbound;
                }
                var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;

                WhS_BE_DealInboundService.editDealInbound(dealInboundObj, sellingNumberPlanId, onDealInboundUpdated);
            }

            function deleteSwapDealInbound(swapDealInboundObj) {
                var onSwapDealInboundDeleted = function () {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, swapDealInboundObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                WhS_BE_DealInboundService.deleteDealInbound($scope, swapDealInboundObj, onSwapDealInboundDeleted);
            }

        }

        return directiveDefinitionObject;

    }
]);