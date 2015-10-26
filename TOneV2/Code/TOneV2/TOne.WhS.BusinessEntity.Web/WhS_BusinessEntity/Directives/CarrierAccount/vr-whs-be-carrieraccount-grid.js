"use strict";

app.directive("vrWhsBeCarrieraccountGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CarrierAccountAPIService", "WhS_BE_MainService", "WhS_Be_CarrierAccountTypeEnum",
function (UtilsService, VRNotificationService, WhS_BE_CarrierAccountAPIService, WhS_BE_MainService, WhS_Be_CarrierAccountTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            hideprofilecolumn:'@'
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
            $scope.hideProfileColumn = false;
            if ($attrs.hideprofilecolumn != undefined)
            {
                $scope.hideProfileColumn = true;
            }
            $scope.isExpandable = function (dataItem) {
                if (dataItem.AccountType == WhS_Be_CarrierAccountTypeEnum.Supplier.value)
                    return  false;
                return true
            }
            
            $scope.carrierAccounts = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onCarrierAccountAdded = function (carrierAccountObject) {
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
                                if (response.Data[i].AccountType != WhS_Be_CarrierAccountTypeEnum.Supplier.value) {
                                    setDataItemExtension(response.Data[i]);
                                }
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function setDataItemExtension(dataItem) {
            
            var extensionObject = {};
            var query = {
                CustomersIds: [dataItem.CarrierAccountId],
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.custormerSellingProductGridAPI = api;
                extensionObject.custormerSellingProductGridAPI.loadGrid(query);
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;
            
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
                if (dataItem.AccountType == WhS_Be_CarrierAccountTypeEnum.Customer.value || dataItem.AccountType == WhS_Be_CarrierAccountTypeEnum.Exchange.value)
                {
                    return menuActionsWithSellingProduct;
                }
                else {  
                    return defaultMenuActions;
                }
            }
        }
        
        function editCarrierAccount(carrierAccountObj) {
            var onCarrierAccountUpdated = function (carrierAccount) {
                gridAPI.itemUpdated(carrierAccount);
            }

            WhS_BE_MainService.editCarrierAccount(carrierAccountObj, onCarrierAccountUpdated);
        }
        function assignNew(dataItem) {
            if (dataItem.AccountType == WhS_Be_CarrierAccountTypeEnum.Supplier.value)
                return;

            gridAPI.expandRow(dataItem);
            var query = {
                CustomersIds: [dataItem.CarrierAccountId],
            }
            if (dataItem.extensionObject.custormerSellingProductGridAPI != undefined)
                dataItem.extensionObject.custormerSellingProductGridAPI.loadGrid(query);
            var onCustomerSellingProductAdded = function (customerSellingProductObj) {
                if (dataItem.extensionObject.custormerSellingProductGridAPI != undefined)
                {
                    for (var i = 0; i < customerSellingProductObj.length; i++) {
                        if (customerSellingProductObj[i].Status == 0 && gridAPI != undefined)
                            dataItem.extensionObject.custormerSellingProductGridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);
                        else if (customerSellingProductObj[i].Status == 1 && gridAPI != undefined) {
                            dataItem.extensionObject.custormerSellingProductGridAPI.onCustomerSellingProductUpdated(customerSellingProductObj[i]);
                        }
                    }
                }
            };
            WhS_BE_MainService.addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem);
        }
        function deleteCarrierAccount(carrierAccountObj) {
            var onCarrierAccountDeleted = function () {
                retrieveData();
            };

            WhS_BE_MainService.deleteCarrierAccount(carrierAccountObj, onCarrierAccountDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
