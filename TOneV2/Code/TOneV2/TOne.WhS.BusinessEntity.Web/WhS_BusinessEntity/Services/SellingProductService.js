
app.service('WhS_BE_SellingProductService', ['WhS_BE_SellingProductAPIService',
    'VRModalService', 'VRNotificationService', 'UtilsService',
    function (WhS_BE_SellingProductAPIService, VRModalService, VRNotificationService, UtilsService) {

        return ({
            addSellingProduct: addSellingProduct,
            editSellingProduct: editSellingProduct,
            deleteSellingProduct: deleteSellingProduct
        });

        function addSellingProduct(onSellingProductAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New Selling Product";
                modalScope.onSellingProductAdded = onSellingProductAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', null, settings);
        }

        function editSellingProduct(sellingProductObj, onSellingProductUpdated) {
            var modalSettings = {
            };
            var parameters = {
                SellingProductId: sellingProductObj.SellingProductId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Selling Product";
                modalScope.onSellingProductUpdated = onSellingProductUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', parameters, modalSettings);
        }

        function deleteSellingProduct($scope, sellingProductObj, onSellingProductDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_BE_SellingProductAPIService.DeleteSellingProduct(sellingProductObj.SellingProductId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Selling Product", deletionResponse);
                                onSellingProductDeleted(sellingProductObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }

    }]);