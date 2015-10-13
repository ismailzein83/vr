"use strict";

app.directive("vrWhsBeCarrieraccountgrid", ["UtilsService", "VRNotificationService", "WhS_BE_CarrierAccountAPIService", "WhS_BE_MainService","WhS_Be_CarrierAccountEnum",
function (UtilsService, VRNotificationService, WhS_BE_CarrierAccountAPIService, WhS_BE_MainService,WhS_Be_CarrierAccountEnum) {

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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Templates/CarrierAccountGridTemplate.html"

    };

    function CarrierAccountGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.hideprofilecolumn=false;
            if ($attrs.hideprofilecolumn != undefined)
            {
                $scope.hideprofilecolumn=true;
            }
            $scope.isExpandable = function (dataItem) {
                if (dataItem.AccountType == WhS_Be_CarrierAccountEnum.Supplier.value)
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
                                if (response.Data[i].AccountType != WhS_Be_CarrierAccountEnum.Supplier.value) {
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
                extensionObject.custormerPricingProductGridAPI = api;
                extensionObject.custormerPricingProductGridAPI.loadGrid(query);
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;
            
        }

        function defineMenuActions() {
            var assignPricingProductObj = [
                        {
                            name: "Edit",
                            clicked: editCarrierAccount,
                        },
                        {
                            name: "Assign Pricing Product",
                            clicked: assignNew
                        }
            ];
            var assignCustomersObj = [
                        {
                            name: "Edit",
                            clicked: editCarrierAccount,
                        },
                       {
                           name: "Assign Customers",
                           clicked: assignNew
                       }
            ];

            $scope.gridMenuActions = function (dataItem) {
                if (dataItem.CarrierAccountId != undefined)
                {
                    return assignPricingProductObj;
                }
                else {  
                    return assignCustomersObj;
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
            if (dataItem.AccountType == WhS_Be_CarrierAccountEnum.Supplier.value)
                return;

            gridAPI.expandRow(dataItem);
            var query = {
                CustomersIds: [dataItem.CarrierAccountId],
            }
            if (dataItem.extensionObject.custormerPricingProductGridAPI != undefined)
                dataItem.extensionObject.custormerPricingProductGridAPI.loadGrid(query);
            var onCustomerPricingProductAdded = function (customerPricingProductObj) {
                if (dataItem.extensionObject.custormerPricingProductGridAPI != undefined)
                    dataItem.extensionObject.custormerPricingProductGridAPI.onCustomerPricingProductAdded(customerPricingProductObj);
            };
            WhS_BE_MainService.addCustomerPricingProduct(onCustomerPricingProductAdded, dataItem);
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
