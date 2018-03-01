(function (app) {

    'use strict';

    SwapDealGridDirective.$inject = ['WhS_Deal_SwapDealAPIService', 'WhS_Deal_SwapDealService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_DealAgreementTypeEnum'];

    function SwapDealGridDirective(WhS_Deal_SwapDealAPIService, WhS_Deal_SwapDealService, VRNotificationService, VRUIUtilsService, WhS_Deal_DealAgreementTypeEnum) {
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

            var gridDrillDownTabsObj;

            var gridAPI;

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
                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    var menuActions = [];
                    var menuAction;
                    if (dataItem.TypeDescription == WhS_Deal_DealAgreementTypeEnum.Commitment.description) {
                        menuAction = {
                            name: "View",
                            clicked: viewDeal,
                        };
                    }
                    else {
                        menuAction = {
                            name: 'Edit',
                            clicked: editDeal,
                            haspermission: hasEditSwapDealPermission
                        }
                    }
                    menuActions.push(menuAction);
                    return menuActions;
                };
            }
            function editDeal(dataItem) {
                var isReadOnly =  false;
                if (dataItem.Entity != undefined && dataItem.Entity.Settings != undefined)
                    isReadOnly = dataItem.Entity.Settings.DealType == WhS_Deal_DealAgreementTypeEnum.Commitment.value;
                var onDealUpdated = function (updatedDeal) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedDeal);
                    gridAPI.itemUpdated(updatedDeal);
                };
                WhS_Deal_SwapDealService.editSwapDeal(dataItem.Entity.DealId, onDealUpdated , isReadOnly);
            }

            function viewDeal(dataItem) {
                WhS_Deal_SwapDealService.viewSwapDeal(dataItem.Entity.DealId);
            }
            function hasEditSwapDealPermission() {
                return WhS_Deal_SwapDealAPIService.HasEditDealPermission();
            }
        }
    }

    app.directive('vrWhsDealSwapdealGrid', SwapDealGridDirective);

})(app);