(function (app) {

    'use strict';

    SwapDealGridDirective.$inject = ['WhS_Deal_SwapDealAPIService', 'WhS_Deal_SwapDealService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_DealAgreementTypeEnum', 'WhS_Deal_DealStatusTypeEnum', 'WhS_Deal_RecurDealService', 'UtilsService', 'VRDateTimeService'];

    function SwapDealGridDirective(WhS_Deal_SwapDealAPIService, WhS_Deal_SwapDealService, VRNotificationService, VRUIUtilsService, WhS_Deal_DealAgreementTypeEnum, WhS_Deal_DealStatusTypeEnum, WhS_Deal_RecurDealService, UtilsService, VRDateTimeService) {
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
                    if (dataItem.TypeDescription == WhS_Deal_DealAgreementTypeEnum.Commitment.description
                        && dataItem.SubscriberStatusDescription == WhS_Deal_DealStatusTypeEnum.Active.description
                        && dataItem.Entity.Settings.BeginDate > UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())) {
                        menuAction = {
                            name: 'View',
                            clicked: viewDeal,
                        };
                    }
                    else {
                        menuAction = {
                            name: 'Edit',
                            clicked: editDeal,
                            haspermission: hasEditSwapDealPermission
                        };
                    }
                    menuActions.push(menuAction);
                    if (dataItem.StatusDescription == WhS_Deal_DealStatusTypeEnum.Active.description && dataItem.Entity.Settings.IsRecurrable == true) {
                        var recurMenuAction = {
                            name: 'Recur',
                            clicked: recurDeal
                        };
                        menuActions.push(recurMenuAction);
                    }
                    return menuActions;
                };
            }
            function editDeal(dataItem) {
                var isReadOnly = false;

                var dealBED = UtilsService.getDateObject(dataItem.Entity.Settings.BeginDate);

                var isEffective = VRDateTimeService.getNowDateTime().getTime() > dealBED.getTime();
                if (dataItem.Entity != undefined && dataItem.Entity.Settings != undefined) {
                    if (dataItem.TypeDescription === WhS_Deal_DealAgreementTypeEnum.Commitment.description && isEffective) {
                        isReadOnly = true;
                        if (dataItem.StatusDescription === WhS_Deal_DealStatusTypeEnum.Inactive.description)
                            isReadOnly = false;
                    }
                }
                var onDealUpdated = function (updatedDeal) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedDeal);
                    gridAPI.itemUpdated(updatedDeal);
                };
                WhS_Deal_SwapDealService.editSwapDeal(dataItem.Entity.DealId, onDealUpdated, isReadOnly);
            }

            function recurDeal(dataItem) {
                var onRecur = function (dealId, recurringNumber, recurringType) {
                    return WhS_Deal_SwapDealAPIService.RecurDeal(dealId, recurringNumber, recurringType).then(function (response) {
                        for (var index in response.InsertedObject) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.InsertedObject[index]);
                            gridAPI.itemAdded(response.InsertedObject[index]);
                        }
                        return response;
                    });
                };
                WhS_Deal_RecurDealService.recurDeal(dataItem.Entity.DealId, dataItem.Entity.Name, onRecur);
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