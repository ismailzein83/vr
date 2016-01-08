"use strict";

app.directive("vrWhsBeCarrieraccountGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CarrierAccountAPIService", "WhS_Be_CarrierAccountTypeEnum", "WhS_BE_CustomerSellingProductService", "WhS_BE_CarrierAccountService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_CarrierAccountAPIService, WhS_Be_CarrierAccountTypeEnum, WhS_BE_CustomerSellingProductService, WhS_BE_CarrierAccountService, VRUIUtilsService) {

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

        function initializeController() {
            var gridDrillDownTabsObj;
            $scope.hideProfileColumn = false;

            $scope.isExpandable = function (dataItem) {
                if (dataItem.Entity.AccountType == WhS_Be_CarrierAccountTypeEnum.Supplier.value)
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

                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                if (response.Data[i].Entity.AccountType != WhS_Be_CarrierAccountTypeEnum.Supplier.value) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
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
            var menuActionsWithSellingProduct = [
                {
                    name: "Edit",
                    clicked: editCarrierAccount,
                },
                {
                    name: "Assign Selling Product",
                    clicked: assignNew
                }
            ];
            var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editCarrierAccount,
                }
            ];

            $scope.gridMenuActions = function (dataItem) {
                if (dataItem.Entity.AccountType == WhS_Be_CarrierAccountTypeEnum.Customer.value || dataItem.Entity.AccountType == WhS_Be_CarrierAccountTypeEnum.Exchange.value) {
                    return menuActionsWithSellingProduct;
                } else {
                    return defaultMenuActions;
                }
            }
        }

        function editCarrierAccount(carrierAccountObj) {
            var onCarrierAccountUpdated = function (carrierAccount) {
                gridAPI.itemUpdated(carrierAccount);
            }
            var carrierAccountItem;

            if ($scope.hideProfileColumn)
                carrierAccountItem = carrierAccountObj.Entity;
            else
                carrierAccountItem = carrierAccountObj.Entity.CarrierAccountId;
            WhS_BE_CarrierAccountService.editCarrierAccount(carrierAccountItem, onCarrierAccountUpdated);
        }

        function assignNew(dataItem) {
            if (dataItem.Entity.AccountType == WhS_Be_CarrierAccountTypeEnum.Supplier.value)
                return;

            gridAPI.expandRow(dataItem);
            var onCustomerSellingProductAdded = function (customerSellingProductObj) {
                if (dataItem.extensionObject.custormerSellingProductGridAPI != undefined) {
                    for (var i = 0; i < customerSellingProductObj.length; i++) {
                        dataItem.extensionObject.custormerSellingProductGridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);
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