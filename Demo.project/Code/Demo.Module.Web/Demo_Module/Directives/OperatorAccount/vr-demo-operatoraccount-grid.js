"use strict";

app.directive("vrDemoOperatoraccountGrid", ["UtilsService", "VRNotificationService", "Demo_OperatorAccountAPIService", "Demo_OperatorAccountService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_OperatorAccountAPIService, Demo_OperatorAccountService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var operatorAccountGrid = new OperatorAccountGrid($scope, ctrl, $attrs);
            operatorAccountGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/OperatorAccount/Templates/OperatorAccountGridTemplate.html"

    };

    function OperatorAccountGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;
        var gridDrillDownTabsObj;
        function initializeController() {

            $scope.hideProfileColumn = false;

            $scope.isExpandable = function (dataItem) {
                return true
            }

            $scope.operatorAccounts = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Customer Selling Product";
                drillDownDefinition.directive = "vr-demo-customersellingproduct-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, operatorAccountItem) {
                    operatorAccountItem.customersellingproductGridAPI = directiveAPI;
                    var payload = {
                        query: {
                            CustomersIds: [operatorAccountItem.Entity.OperatorAccountId]
                        },
                        hideCustomerColumn: true
                    };
                    return operatorAccountItem.customersellingproductGridAPI.loadGrid(payload);
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
                    directiveAPI.onOperatorAccountAdded = function (operatorAccountObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(operatorAccountObject);
                        gridAPI.itemAdded(operatorAccountObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_OperatorAccountAPIService.GetFilteredOperatorAccounts(dataRetrievalInput)
                    .then(function (response) {

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
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
                    clicked: editOperatorAccount,
                },
                {
                    name: "Assign Selling Product",
                    clicked: assignNew
                }
            ];
            var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editOperatorAccount,
                }
            ];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            }
        }

        function editOperatorAccount(operatorAccountObj) {
            var onOperatorAccountUpdated = function (operatorAccount) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(operatorAccount);
                gridAPI.itemUpdated(operatorAccount);
            }
            var operatorAccountItem;

            if ($scope.hideProfileColumn)
                operatorAccountItem = operatorAccountObj.Entity;
            else
                operatorAccountItem = operatorAccountObj.Entity.OperatorAccountId;
            Demo_OperatorAccountService.editOperatorAccount(operatorAccountItem, onOperatorAccountUpdated);
        }

        function assignNew(dataItem) {

            gridAPI.expandRow(dataItem);
            var onCustomerSellingProductAdded = function (customerSellingProductObj) {
                if (dataItem.customersellingproductGridAPI != undefined) {
                    for (var i = 0; i < customerSellingProductObj.length; i++) {
                        dataItem.customersellingproductGridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);
                    }
                }
            };
            // Demo_CustomerSellingProductService.addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem.Entity);
        }

        function deleteOperatorAccount(operatorAccountObj) {
            var onOperatorAccountDeleted = function () {
                retrieveData();
            };

            // Demo_MainService.deleteOperatorAccount(operatorAccountObj, onOperatorAccountDeleted); to be added in OperatorAccountService
        }
    }

    return directiveDefinitionObject;

}]);