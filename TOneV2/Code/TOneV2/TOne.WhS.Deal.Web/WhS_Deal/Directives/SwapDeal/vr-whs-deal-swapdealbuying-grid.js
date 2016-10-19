"use strict";

app.directive("vrWhsDealSwapdealbuyingGrid", ["UtilsService", "VRNotificationService", "WhS_Deal_SwapDealBuyingService",
    function (UtilsService, VRNotificationService, WhS_Deal_SwapDealBuyingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var swapDealBuyingGrid = new SwapDealBuyingGrid($scope, ctrl, $attrs);
                swapDealBuyingGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/Templates/SwapDealBuyingGridTemplate.html'

        };

        function SwapDealBuyingGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {

                ctrl.datasource = [];

                ctrl.addSwapDealBuying = function () {


                    var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;
                    var onSwapDealBuyingAdded = function (addedSwapDealBuying) {
                        ctrl.datasource.push(addedSwapDealBuying);
                    };
                    WhS_Deal_SwapDealBuyingService.addSwapDealBuying(onSwapDealBuyingAdded, supplierId);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;

                    if (payload.Inbounds != undefined && payload.Inbounds != null) {
                        ctrl.datasource = payload.Inbounds;
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            ctrl.datasource[i].Amount = ctrl.datasource[i].Volume * ctrl.datasource[i].Rate;
                        }
                    }
                        
                    else
                        ctrl.datasource.length = 0;

                };

                api.getData = function () {
                    var buyingObj =
                      {
                          buyingParts: []
                      }

                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        buyingObj.buyingParts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            buyingObj.buyingParts.push({
                                Name: currentItem.Name,
                                Volume: currentItem.Volume,
                                Rate: currentItem.Rate,                               
                                SupplierZoneIds: currentItem.SupplierZoneIds
                            });
                        }

                    }
                    return buyingObj;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSwapDealBuying
                },
                {
                    name: "Delete",
                    clicked: deleteSwapDealBuying
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editSwapDealBuying(swapDealBuyingObj) {
                var onSwapDealBuyingUpdated = function (swapDealBuying) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, swapDealBuyingObj.Name, 'Name');
                    ctrl.datasource[index] = swapDealBuying;
                }
                var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;

                WhS_Deal_SwapDealBuyingService.editSwapDealBuying(swapDealBuyingObj, supplierId, onSwapDealBuyingUpdated);

            }

            function deleteDealBuying(dealBuyingObj) {
                var onDealBuyingDeleted = function () {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dealBuyingObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                WhS_Deal_SwapDealBuyingService.deleteSwapDealBuying($scope, dealBuyingObj, onDealBuyingDeleted);
            }

        }

        return directiveDefinitionObject;

    }
]);