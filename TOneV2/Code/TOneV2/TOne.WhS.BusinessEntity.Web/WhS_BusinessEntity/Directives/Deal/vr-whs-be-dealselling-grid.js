"use strict";

app.directive("vrWhsBeDealsellingGrid", ["UtilsService", "VRNotificationService", "WhS_BE_DealSellingService",
    function (UtilsService, VRNotificationService, WhS_BE_DealSellingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dealSellingGrid = new DealSellingGrid($scope, ctrl, $attrs);
                dealSellingGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Deal/Templates/DealSellingGridTemplate.html'

        };

        function DealSellingGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {

                ctrl.datasource = [];

                ctrl.addDealSelling = function () {


                    var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;
                    var onDealSellingAdded = function (addedDealSelling) {
                        ctrl.datasource.push(addedDealSelling);
                    };
                    WhS_BE_DealSellingService.addDealSelling(onDealSellingAdded, sellingNumberPlanId);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    


                    if (payload.SellingParts != undefined && payload.SellingParts != null)
                        ctrl.datasource = payload.SellingParts;
                    else
                        ctrl.datasource.length = 0;

                    console.log("1");
                    console.log(payload);

                };

                api.getData = function () {
                    var sellingParts;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        sellingParts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            sellingParts.push({
                                Name: currentItem.Name,
                                Volume: currentItem.Volume,
                                Rate: currentItem.Rate,
                                MaxBuyingRate: currentItem.MaxBuyingRate,
                                SubstituteRate: currentItem.SubstituteRate,
                                ExtraVolumeRate: currentItem.ExtraVolumeRate,
                                SaleZoneIds: currentItem.SaleZoneIds,
                                ASR: currentItem.asr,
                                NER: currentItem.ner,
                                ACD: currentItem.acd
                            });
                        }

                    }
                    return sellingParts;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editDealSelling
                },
                {
                    name: "Delete",
                    clicked: deleteDealSelling
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editDealSelling(dealSellingObj) {
                var onDealSellingUpdated = function (dealSelling) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dealSellingObj.Name, 'Name');
                    ctrl.datasource[index] = dealSelling;
                }
                var sellingNumberPlanId = mainPayload != undefined ? mainPayload.sellingNumberPlanId : undefined;

                WhS_BE_DealSellingService.editDealSelling(dealSellingObj, sellingNumberPlanId, onDealSellingUpdated);

            }

            function deleteDealSelling(dealSellingObj) {
                var onDealSellingDeleted = function () {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dealSellingObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                WhS_BE_DealSellingService.deleteDealSelling($scope, dealSellingObj, onDealSellingDeleted);
            }

        }

        return directiveDefinitionObject;

    }
]);