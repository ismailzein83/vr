(function (app) {

    'use strict';

    VolumeCommitmentGridDirective.$inject = ['WhS_Deal_VolCommitmentDealAPIService','WhS_Deal_DealStatusTypeEnum' ,'WhS_Deal_VolumeCommitmentService','WhS_Deal_ReoccurDealService', 'VRNotificationService', 'VRUIUtilsService'];

    function VolumeCommitmentGridDirective(WhS_Deal_VolCommitmentDealAPIService, WhS_Deal_DealStatusTypeEnum, WhS_Deal_VolumeCommitmentService, WhS_Deal_ReoccurDealService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dealGrid = new VolumeCommitmentGrid($scope, ctrl, $attrs);
                dealGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentGridTemplate.html'
        };

        function VolumeCommitmentGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = WhS_Deal_VolumeCommitmentService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Deal_VolCommitmentDealAPIService.GetFilteredVolCommitmentDeals(dataRetrievalInput).then(function (response) {
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

                api.onVolumeCommitmentAdded = function (addedVolumeCommitment) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVolumeCommitment);
                    gridAPI.itemAdded(addedVolumeCommitment);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = function (dataItem) {
                var menuActions = [];
                var editMenuAction = {
                    name: 'Edit',
                    clicked: editVolumeCommitment,
                    haspermission: hasEditVolCommitmentDealPermission
                };
                menuActions.push(editMenuAction);

                    if (dataItem.StatusDescription == WhS_Deal_DealStatusTypeEnum.Active.description && dataItem.Entity.Settings.IsReoccurrable==true ) {
                    var reoccurMenuAction = {
                        name: 'Reoccur',
                        clicked: reoccurDeal
                    };
                    menuActions.push(reoccurMenuAction);
                }

                return menuActions;
                };
            }
            function reoccurDeal(dataItem) {
                var onReoccur = function (dealId, reoccuringNumber, reoccuringType) {
                    return WhS_Deal_VolCommitmentDealAPIService.ReoccurDeal(dealId, reoccuringNumber, reoccuringType).then(function (response) {
                        for (var index in response.InsertedObject) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.InsertedObject[index]);
                            gridAPI.itemAdded(response.InsertedObject[index]);
                        }
                        return response;
                    });
                };
                WhS_Deal_ReoccurDealService.reoccurDeal(dataItem.Entity.DealId, dataItem.Entity.Name, onReoccur);
            }

            function editVolumeCommitment(dataItem) {
                var onVolumeCommitmentUpdated = function (updatedVolumeCommitment) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVolumeCommitment);
                    gridAPI.itemUpdated(updatedVolumeCommitment);
                };
                WhS_Deal_VolumeCommitmentService.editVolumeCommitment(dataItem.Entity.DealId, onVolumeCommitmentUpdated);
            }

            function hasEditVolCommitmentDealPermission() {
                return WhS_Deal_VolCommitmentDealAPIService.HasEditDealPermission();
            }
        }
    }

    app.directive('vrWhsDealVolumecommitmentGrid', VolumeCommitmentGridDirective);

})(app);