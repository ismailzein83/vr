"use strict";

app.directive("vrWhsBeDealbuyingGrid", ["UtilsService", "VRNotificationService", "WhS_BE_DealBuyingService",
    function (UtilsService, VRNotificationService, WhS_BE_DealBuyingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dealBuyingGrid = new DealBuyingGrid($scope, ctrl, $attrs);
                dealBuyingGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Deal/Templates/DealBuyingGridTemplate.html'

        };

        function DealBuyingGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {

                ctrl.datasource = [];

                ctrl.addDealBuying = function () {


                    var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;
                    var onDealBuyingAdded = function (addedDealBuying) {
                        ctrl.datasource.push(addedDealBuying);
                    };
                    WhS_BE_DealBuyingService.addDealBuying(onDealBuyingAdded, supplierId);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;

                    if (payload.BuyingParts != undefined && payload.BuyingParts != null) {
                        ctrl.datasource = payload.BuyingParts;
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
                      buyingParts: [],
                      buyingAmount: 0,
                      buyingDuration: 0
                  }

                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        buyingObj.buyingParts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            buyingObj.buyingParts.push({
                                Name: currentItem.Name,
                                Volume: currentItem.Volume,
                                Rate: currentItem.Rate,
                                MinSellingRate: currentItem.MinSellingRate,
                                SubstituteRate: currentItem.SubstituteRate,
                                ExtraVolumeRate: currentItem.ExtraVolumeRate,
                                SupplierZoneIds: currentItem.SupplierZoneIds,
                                ASR: currentItem.ASR,
                                NER: currentItem.NER,
                                ACD: currentItem.ACD
                            });
                            buyingObj.buyingAmount = buyingObj.buyingAmount + (currentItem.Volume * currentItem.Rate);
                            buyingObj.buyingDuration = Number(buyingObj.buyingDuration) + Number(currentItem.Volume);
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
                    clicked: editDealBuying
                },
                {
                    name: "Delete",
                    clicked: deleteDealBuying
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editDealBuying(dealBuyingObj) {
                var onDealBuyingUpdated = function (dealBuying) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dealBuyingObj.Name, 'Name');
                    ctrl.datasource[index] = dealBuying;
                }
                var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;

                WhS_BE_DealBuyingService.editDealBuying(dealBuyingObj, supplierId, onDealBuyingUpdated);

            }

            function deleteDealBuying(dealBuyingObj) {
                var onDealBuyingDeleted = function () {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dealBuyingObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                WhS_BE_DealBuyingService.deleteDealBuying($scope, dealBuyingObj, onDealBuyingDeleted);
            }

        }

        return directiveDefinitionObject;

    }
]);