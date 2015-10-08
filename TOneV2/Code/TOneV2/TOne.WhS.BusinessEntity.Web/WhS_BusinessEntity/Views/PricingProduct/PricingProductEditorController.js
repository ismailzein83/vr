(function (appControllers) {

    "use strict";

    pricingProductEditorController.$inject = ['$scope', 'WhS_BE_PricingProductAPIService','UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function pricingProductEditorController($scope, WhS_BE_PricingProductAPIService,UtilsService, VRNotificationService, VRNavigationService) {

        var editMode;
        var pricingProductId;
        var saleZonePackagesDirectiveAPI;
        var routingProductDirectiveAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                pricingProductId = parameters.PricingProductId;
            }
            editMode = (pricingProductId != undefined);
        }

        function defineScope() {
            $scope.SavePricingProduct = function () {
                if (editMode) {
                    return updatePricingProduct();
                }
                else {
                    return insertPricingProduct();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.onSaleZonePackagesDirectiveLoaded = function (api) {
                saleZonePackagesDirectiveAPI = api;
            }
            $scope.onRoutingProductDirectiveLoaded = function (api) {
                routingProductDirectiveAPI = api;
            }
            $scope.onselectionchanged = function () {
            }
            $scope.onselectionchanged1 = function () {
            }
        }

        function load() {
            $scope.isGettingData = true;
                if (editMode) {
                    getPricingProduct();
                }
                else {
                    $scope.isGettingData = false;
                }
        }

        function getPricingProduct() {
            return WhS_BE_PricingProductAPIService.GetPricingProduct(pricingProductId).then(function (pricingProduct) {
                fillScopeFromPricingProductObj(pricingProduct);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {

                $scope.isGettingData = false;
            });
        }

        function buildPricingProductObjFromScope() {
            var salezonepackageid;
            var routingProductId;
            if (saleZonePackagesDirectiveAPI != undefined)
                salezonepackageid = saleZonePackagesDirectiveAPI.getData().SaleZonePackageId;
            var pricingProduct = {
                PricingProductId: (pricingProductId != null) ? pricingProductId : 0,
                Name: $scope.name,
                SaleZonePackageId: salezonepackageid,
            };

            return pricingProduct;
        }

        function insertPricingProduct() {
            var pricingProductObject = buildPricingProductObjFromScope();
            return WhS_BE_PricingProductAPIService.AddPricingProduct(pricingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Pricing Product", response)) {
                    if ($scope.onPricingProductAdded != undefined)
                        $scope.onPricingProductAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function fillScopeFromPricingProductObj(pricingProductObj) {
            $scope.name = pricingProductObj.Name;
                saleZonePackagesDirectiveAPI.setData(pricingProductObj.SaleZonePackageId);
                 
        }
        function updatePricingProduct() {
            var pricingProductObject = buildPricingProductObjFromScope();
            WhS_BE_PricingProductAPIService.UpdatePricingProduct(pricingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Pricing Product", response)) {
                    if ($scope.onPricingProductUpdated != undefined)
                        $scope.onPricingProductUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_BE_PricingProductEditorController', pricingProductEditorController);
})(appControllers);
