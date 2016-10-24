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
            function initializeController() {

                ctrl.datasource = [];

                ctrl.addSwapDealOutbound = function () {
                    var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;
                    var onSwapDealOutboundAdded = function (addedSwapDealOutbound) {
                        ctrl.datasource.push(addedSwapDealOutbound);
                    };
                    WhS_Deal_SwapDealOutboundService.addSwapDealOutbound(onSwapDealOutboundAdded, supplierId);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;

                    if (payload!= undefined && payload.Outbounds != null) {
                        ctrl.datasource = payload.Outbounds;
                       
                    }
                        
                    else
                        ctrl.datasource.length = 0;

                };

                api.getData = function () {
                    var outbounds = [];


                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            outbounds.push({
                                Name: currentItem.Name,
                                Volume: currentItem.Volume,
                                Rate: currentItem.Rate,                               
                                SupplierZoneIds: currentItem.SupplierZoneIds,
                                CountryId: currentItem.CountryId
                            });
                        }

                    }
                    return outbounds;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSwapDealOutbound
                },
                {
                    name: "Delete",
                    clicked: deleteSwapDealOutbound
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editSwapDealOutbound(swapDealOutboundObj) {
                var onSwapDealOutboundUpdated = function (swapDealOutbound) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, swapDealOutboundObj.Name, 'Name');
                    ctrl.datasource[index] = swapDealOutbound;
                }
                var supplierId = mainPayload != undefined ? mainPayload.supplierId : undefined;

                WhS_Deal_SwapDealOutboundService.editSwapDealOutbound(swapDealOutboundObj, supplierId, onSwapDealOutboundUpdated);

            }

            function deleteSwapDealOutbound(swapDealOutboundObj) {
                var onSwapDealOutboundDeleted = function () {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, swapDealOutboundObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                WhS_Deal_SwapDealOutboundService.deleteSwapDealOutbound($scope, swapDealOutboundObj, onSwapDealOutboundDeleted);
            }

        }

        return directiveDefinitionObject;

    }
]);