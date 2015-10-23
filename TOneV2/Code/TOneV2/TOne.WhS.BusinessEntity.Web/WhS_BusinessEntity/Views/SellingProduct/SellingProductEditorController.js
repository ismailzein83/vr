(function (appControllers) {

    "use strict";

    sellingProductEditorController.$inject = ['$scope', 'WhS_BE_SellingProductAPIService','UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function sellingProductEditorController($scope, WhS_BE_SellingProductAPIService,UtilsService, VRNotificationService, VRNavigationService) {

        var editMode;
        var sellingProductId;
        var sellingNumberPlansDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingProductId = parameters.SellingProductId;
            }
            editMode = (sellingProductId != undefined);
        }

        function defineScope() {
            $scope.SaveSellingProduct = function () {
                if (editMode) {
                    return updateSellingProduct();
                }
                else {
                    return insertSellingProduct();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.onSellingNumberPlansDirectiveReady = function (api) {
                sellingNumberPlansDirectiveAPI = api;
                load();
            }
        }

        function load() {
            $scope.isGettingData = true;

            if (sellingNumberPlansDirectiveAPI == undefined)
                return;

            sellingNumberPlansDirectiveAPI.load().then(function () {
                if (editMode) {
                    getSellingProduct();
                }
                else {
                    $scope.isGettingData = false;
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }

        function getSellingProduct() {
            return WhS_BE_SellingProductAPIService.GetSellingProduct(sellingProductId).then(function (sellingProduct) {
                fillScopeFromSellingProductObj(sellingProduct);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });;
        }

        function buildSellingProductObjFromScope() {
            var sellingProduct = {
                SellingProductId: (sellingProductId != null) ? sellingProductId : 0,
                Name: $scope.name,
                SellingNumberPlanId: sellingNumberPlansDirectiveAPI.getData().SellingNumberPlanId,
            };

            return sellingProduct;
        }


        function fillScopeFromSellingProductObj(sellingProductObj) {
            $scope.name = sellingProductObj.Name;
            sellingNumberPlansDirectiveAPI.setData(sellingProductObj.SellingNumberPlanId);
        }

        function insertSellingProduct() {
            var sellingProductObject = buildSellingProductObjFromScope();
            return WhS_BE_SellingProductAPIService.AddSellingProduct(sellingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Selling Product", response)) {
                    if ($scope.onSellingProductAdded != undefined)
                        $scope.onSellingProductAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateSellingProduct() {
            var sellingProductObject = buildSellingProductObjFromScope();
            WhS_BE_SellingProductAPIService.UpdateSellingProduct(sellingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Selling Product", response)) {
                    if ($scope.onSellingProductUpdated != undefined)
                        $scope.onSellingProductUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_BE_SellingProductEditorController', sellingProductEditorController);
})(appControllers);
