"use strict";

app.directive("vrWhsBeCarrieraccountGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CarrierAccountAPIService", "WhS_BE_CarrierAccountTypeEnum",
    "WhS_BE_CustomerSellingProductService", "WhS_BE_CarrierAccountService", "VRUIUtilsService", "WhS_BE_CustomerSellingProductAPIService", "WhS_BE_CarrierAccountActivationStatusEnum",
function (UtilsService, VRNotificationService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum, WhS_BE_CustomerSellingProductService,
    WhS_BE_CarrierAccountService, VRUIUtilsService, WhS_BE_CustomerSellingProductAPIService, WhS_BE_CarrierAccountActivationStatusEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var carrierAccountGrid = new CarrierAccountGrid($scope, ctrl, $attrs);
            carrierAccountGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CarrierAccount/Templates/CarrierAccountGridTemplate.html"

    };

    function CarrierAccountGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;
        var gridDrillDownTabsObj;
        function initializeController() {

            $scope.hideProfileColumn = false;
            $scope.menuActions = [];
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                if (dataItem.drillDownExtensionObject != undefined && dataItem.drillDownExtensionObject.menuActions != undefined) {
                    for (var i = 0; i < dataItem.drillDownExtensionObject.menuActions.length; i++) {
                        var menuAction = dataItem.drillDownExtensionObject.menuActions[i];
                        menuActions.push(menuAction);
                    }
                }
                return menuActions;
            };
            $scope.isExpandable = function (dataItem) {
                if (dataItem.drillDownExtensionObject !=undefined && dataItem.drillDownExtensionObject.drillDownDirectiveTabs != undefined && dataItem.drillDownExtensionObject.drillDownDirectiveTabs.length > 0)
                    return true;
                return false;
            };
 
            $scope.carrierAccounts = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = WhS_BE_CarrierAccountService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.menuActions,true);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        var query = payload;
                        if (payload.hideProfileColumn) {
                            $scope.hideProfileColumn = true;
                            query = payload.query;
                        }
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onCarrierAccountAdded = function (carrierAccountObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(carrierAccountObject);
                        gridAPI.itemAdded(carrierAccountObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CarrierAccountAPIService.GetFilteredCarrierAccounts(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                               gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                               addReadySericeApi(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {
            $scope.menuActions.push({
                name: "View",
                clicked: viewCarrierAccount,
                haspermission: hasViewCarrierAccountPermission
            });
            
            $scope.menuActions.push({
                name: "Edit",
                clicked: editCarrierAccount,
                haspermission: hasUpdateCarrierAccountPermission
            });


            function hasUpdateCarrierAccountPermission() {
                return WhS_BE_CarrierAccountAPIService.HasUpdateCarrierAccountPermission();
            }

            function hasViewCarrierAccountPermission() {
                return WhS_BE_CarrierAccountAPIService.HasUpdateCarrierAccountPermission().then(function (response) {
                    return !response;
                });
            }
        }

        function editCarrierAccount(carrierAccountObj) {
            var onCarrierAccountUpdated = function (carrierAccount) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(carrierAccount);
                addReadySericeApi(carrierAccount);
                gridAPI.itemUpdated(carrierAccount);
            };
            var carrierAccountItem;

            if ($scope.hideProfileColumn)
                carrierAccountItem = carrierAccountObj.Entity;
            else
                carrierAccountItem = carrierAccountObj.Entity.CarrierAccountId;
            WhS_BE_CarrierAccountService.editCarrierAccount(carrierAccountItem, onCarrierAccountUpdated);
        }

        function viewCarrierAccount(carrierAccountObj) {          
            WhS_BE_CarrierAccountService.viewCarrierAccount(carrierAccountObj.Entity.CarrierAccountId);
        }

        var addReadySericeApi = function (dataItem) {
            dataItem.onServiceReady = function (api) {
                dataItem.ServieApi = api;
                dataItem.ServieApi.load({ selectedIds: dataItem.Services });
            };
        };
        function deleteCarrierAccount(carrierAccountObj) {
            var onCarrierAccountDeleted = function () {
                retrieveData();
            };

            // WhS_BE_MainService.deleteCarrierAccount(carrierAccountObj, onCarrierAccountDeleted); to be added in CarrierAccountService
        }
    }

    return directiveDefinitionObject;

}]);