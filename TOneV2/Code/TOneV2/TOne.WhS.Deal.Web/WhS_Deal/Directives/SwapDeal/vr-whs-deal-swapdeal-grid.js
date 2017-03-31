﻿(function (app) {

    'use strict';

    SwapDealGridDirective.$inject = ['WhS_Deal_SwapDealAPIService', 'WhS_Deal_SwapDealService', 'VRNotificationService', 'VRUIUtilsService'];

    function SwapDealGridDirective(WhS_Deal_SwapDealAPIService, WhS_Deal_SwapDealService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var swapDealGrid = new SwapDealGrid($scope, ctrl, $attrs);
                swapDealGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/swapDeal/Templates/SwapDealGridTemplate.html'
        };

        function SwapDealGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.swapDeals = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = WhS_Deal_SwapDealService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Deal_SwapDealAPIService.GetFilteredSwapDeals(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onSwapDealAdded = function (addedDeal) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedDeal);
                    gridAPI.itemAdded(addedDeal);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editDeal,
                    haspermission: hasEditSwapDealPermission
                }];
            }
            function editDeal(dataItem) {
                var onDealUpdated = function (updatedDeal) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedDeal);
                    gridAPI.itemUpdated(updatedDeal);
                };
                WhS_Deal_SwapDealService.editSwapDeal(dataItem.Entity.DealId, onDealUpdated);
            }
            function hasEditSwapDealPermission() {
                return WhS_Deal_SwapDealAPIService.HasEditDealPermission();
            }
        }
    }

    app.directive('vrWhsDealSwapdealGrid', SwapDealGridDirective);

})(app);