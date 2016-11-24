(function (appControllers) {

    'use strict';

    CustomerSellingProductService.CustomerSellingProductService = ['UtilsService', 'VRModalService', 'VRNotificationService','WhS_BE_CarrierAccountService','WhS_BE_CarrierAccountTypeEnum','WhS_BE_CustomerSellingProductAPIService','WhS_BE_CarrierAccountActivationStatusEnum'];

    function CustomerSellingProductService(UtilsService, VRModalService, VRNotificationService, WhS_BE_CarrierAccountService, WhS_BE_CarrierAccountTypeEnum, WhS_BE_CustomerSellingProductAPIService, WhS_BE_CarrierAccountActivationStatusEnum) {
        return ({
            addCustomerSellingProduct: addCustomerSellingProduct,
            editCustomerSellingProduct: editCustomerSellingProduct,
            deleteCustomerSellingProduct: deleteCustomerSellingProduct,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount
        });

        function addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCustomerSellingProductAdded = onCustomerSellingProductAdded;
            };
            var parameters = null;
            if (dataItem != undefined) {
                parameters = {
                    SellingProductId: dataItem.SellingProductId,
                    CarrierAccountId: dataItem.CarrierAccountId
                };
            }

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/CustomerSellingProductEditor.html', parameters, settings);
        }

        function editCustomerSellingProduct(obj, onCustomerSellingProductUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCustomerSellingProductUpdated = onCustomerSellingProductUpdated;
            };
            var parameters = {
                CustomerSellingProductId: obj.CustomerSellingProductId,
                CarrierAccountId: obj.CustomerId
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/CustomerSellingProductEditor.html', parameters, settings);
        }

        function deleteCustomerSellingProduct($scope, customerSellingProductObj, onCustomerSellingProductDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_BE_CustomerSellingProductAPIService.DeleteCustomerSellingProduct(customerSellingProductObj.CustomerSellingProductId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Customer Selling Product", deletionResponse);
                                onCustomerSellingProductDeleted(deletionResponse.UpdatedObject);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }

        function registerDrillDownToCarrierAccount() {
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
            drillDownDefinition.hideDrillDownFunction = function (dataItem) {
                return !(!checkIfCarrierAccountIsInactive(dataItem.Entity) && (dataItem.Entity.AccountType == WhS_BE_CarrierAccountTypeEnum.Customer.value || dataItem.Entity.AccountType == WhS_BE_CarrierAccountTypeEnum.Exchange.value));
            };
            drillDownDefinition.parentMenuActions = [{
                name: "Assign Selling Product",
                clicked: assignNew,
                haspermission: hasAddCustomerSellingProductPermission
            }];
            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);

            function checkIfCarrierAccountIsInactive(carrierAccount) {
                return (carrierAccount.CarrierAccountSettings != undefined && carrierAccount.CarrierAccountSettings.ActivationStatus == WhS_BE_CarrierAccountActivationStatusEnum.Inactive.value);
            }
            function hasAddCustomerSellingProductPermission() {
                return WhS_BE_CustomerSellingProductAPIService.HasAddCustomerSellingProductPermission();
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
        }
        
    }

    appControllers.service('WhS_BE_CustomerSellingProductService', CustomerSellingProductService);

})(appControllers);
