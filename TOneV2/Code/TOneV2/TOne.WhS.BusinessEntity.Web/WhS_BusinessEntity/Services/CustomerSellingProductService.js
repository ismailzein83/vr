(function (appControllers) {

    'use strict';

    CustomerSellingProductService.CustomerSellingProductService = ['UtilsService', 'VRModalService', 'VRNotificationService'];

    function CustomerSellingProductService(UtilsService, VRModalService, VRNotificationService) {
        return ({
            addCustomerSellingProduct: addCustomerSellingProduct,
            editCustomerSellingProduct: editCustomerSellingProduct,
            deleteCustomerSellingProduct: deleteCustomerSellingProduct
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
    }

    appControllers.service('WhS_BE_CustomerSellingProductService', CustomerSellingProductService);

})(appControllers);
