"use strict";

app.directive("vrWhsBeCarrieraccountGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CarrierAccountAPIService", "WhS_BE_CarrierAccountTypeEnum",
    "WhS_BE_CustomerSellingProductService", "WhS_BE_CarrierAccountService", "VRUIUtilsService", "WhS_BE_CustomerSellingProductAPIService", "WhS_BE_CarrierAccountActivationStatusEnum",
function (UtilsService, VRNotificationService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum, WhS_BE_CustomerSellingProductService,
    WhS_BE_CarrierAccountService, VRUIUtilsService, WhS_BE_CustomerSellingProductAPIService, WhS_BE_CarrierAccountActivationStatusEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
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

            $scope.isExpandable = function (dataItem) {
                if (dataItem.Entity.AccountType == WhS_BE_CarrierAccountTypeEnum.Supplier.value)
                    return false;
                return true
            }

            $scope.carrierAccounts = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Customer Selling Product";
                drillDownDefinition.directive = "vr-whs-be-customersellingproduct-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                    carrierAccountItem.customersellingproductGridAPI = directiveAPI;
                    var payload = {
                        query: {
                            CustomersIds: [carrierAccountItem.Entity.CarrierAccountId]
                        },
                        hideCustomerColumn: true
                    };
                    return carrierAccountItem.customersellingproductGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

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
                    }
                    directiveAPI.onCarrierAccountAdded = function (carrierAccountObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(carrierAccountObject);
                        gridAPI.itemAdded(carrierAccountObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CarrierAccountAPIService.GetFilteredCarrierAccounts(dataRetrievalInput)
                    .then(function (response) {

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                if (response.Data[i].Entity.AccountType != WhS_BE_CarrierAccountTypeEnum.Supplier.value) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                                if (response.Data[i].Entity.AccountType != WhS_BE_CarrierAccountTypeEnum.Customer.value)
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
            var menuActionsWithSellingProduct = [{
                name: "Edit",
                clicked: editCarrierAccount,
                haspermission: hasUpdateCarrierAccountPermission
            }, {
                name: "Assign Selling Product",
                clicked: assignNew,
                haspermission: hasAddCustomerSellingProductPermission
            }];
            var defaultMenuActions = [{
                name: "Edit",
                clicked: editCarrierAccount,
                haspermission: hasUpdateCarrierAccountPermission
            }];

            $scope.gridMenuActions = function (dataItem) {
                if (!checkIfCarrierAccountIsInactive(dataItem.Entity) && (dataItem.Entity.AccountType == WhS_BE_CarrierAccountTypeEnum.Customer.value || dataItem.Entity.AccountType == WhS_BE_CarrierAccountTypeEnum.Exchange.value)) {
                    return menuActionsWithSellingProduct;
                } else {
                    return defaultMenuActions;
                }
            };

            function checkIfCarrierAccountIsInactive(carrierAccount)
            {
                return (carrierAccount.CarrierAccountSettings != undefined && carrierAccount.CarrierAccountSettings.ActivationStatus == WhS_BE_CarrierAccountActivationStatusEnum.Inactive.value);
            }

            function hasAddCustomerSellingProductPermission() {
                return WhS_BE_CustomerSellingProductAPIService.HasAddCustomerSellingProductPermission();
            }

            function hasUpdateCarrierAccountPermission() {
                return WhS_BE_CarrierAccountAPIService.HasUpdateCarrierAccountPermission();
            }
        }

        function editCarrierAccount(carrierAccountObj) {
            var onCarrierAccountUpdated = function (carrierAccount) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(carrierAccount);
                gridAPI.itemUpdated(carrierAccount);
            }
            var carrierAccountItem;

            if ($scope.hideProfileColumn)
                carrierAccountItem = carrierAccountObj.Entity;
            else
                carrierAccountItem = carrierAccountObj.Entity.CarrierAccountId;
            WhS_BE_CarrierAccountService.editCarrierAccount(carrierAccountItem, onCarrierAccountUpdated);
        }

        var addReadySericeApi = function (dataItem) {
            dataItem.onServiceReady = function (api) {
                dataItem.ServieApi = api
                dataItem.ServieApi.load({ selectedIds: dataItem.Services });
            }
        }
        function assignNew(dataItem) {
            if (dataItem.Entity.AccountType == WhS_BE_CarrierAccountTypeEnum.Supplier.value)
                return;

            gridAPI.expandRow(dataItem);
            var onCustomerSellingProductAdded = function (customerSellingProductObj) {
                if (dataItem.customersellingproductGridAPI != undefined) {
                    for (var i = 0; i < customerSellingProductObj.length; i++) {
                        dataItem.customersellingproductGridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);
                    }
                }
            };
            WhS_BE_CustomerSellingProductService.addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem.Entity);
        }

        function deleteCarrierAccount(carrierAccountObj) {
            var onCarrierAccountDeleted = function () {
                retrieveData();
            };

            // WhS_BE_MainService.deleteCarrierAccount(carrierAccountObj, onCarrierAccountDeleted); to be added in CarrierAccountService
        }
    }

    return directiveDefinitionObject;

}]);