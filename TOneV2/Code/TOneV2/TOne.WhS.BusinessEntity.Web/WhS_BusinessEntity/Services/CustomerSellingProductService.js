app.service('WhS_BE_CustomerSellingProductService', ['WhS_BE_CustomerSellingProductAPIService',
    'VRModalService', 'VRNotificationService', 'UtilsService',
    function (WhS_BE_CustomerSellingProductAPIService, VRModalService, VRNotificationService, UtilsService) {


        function addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Customer Selling Product";
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
                modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.SellingProductName, "Customer Selling Product");
                modalScope.onCustomerSellingProductUpdated = onCustomerSellingProductUpdated;
            };
            var parameters = {
                CustomerSellingProductId: obj.CustomerSellingProductId,
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


        return ({
            addCustomerSellingProduct: addCustomerSellingProduct,
            editCustomerSellingProduct: editCustomerSellingProduct,
            deleteCustomerSellingProduct: deleteCustomerSellingProduct
        });

    }]);