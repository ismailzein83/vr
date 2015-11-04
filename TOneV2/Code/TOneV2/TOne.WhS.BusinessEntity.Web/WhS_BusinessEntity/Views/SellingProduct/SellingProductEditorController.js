(function (appControllers) {

    "use strict";

    sellingProductEditorController.$inject = ['$scope', 'WhS_BE_SellingProductAPIService','UtilsService', 'VRNotificationService', 'VRNavigationService','VRUIUtilsService'];

    function sellingProductEditorController($scope, WhS_BE_SellingProductAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var sellingProductId;
        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var sellingProductEntity;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingProductId = parameters.SellingProductId;
            }
            isEditMode = (sellingProductId != undefined);
        }

        function defineScope() {
            $scope.SaveSellingProduct = function () {
                if (isEditMode) {
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
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getSellingProduct().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routeRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }
        function loadAllControls() {
            
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadSellingNumberPlans])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function loadSellingNumberPlans() {
            var sellingNumberPlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload ={
                        selectedIds:sellingProductEntity!=undefined?sellingProductEntity.SellingNumberPlanId:undefined
                    }

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, directivePayload, sellingNumberPlanLoadPromiseDeferred);
                });
            return sellingNumberPlanLoadPromiseDeferred.promise;
        }

        function getSellingProduct() {
            return WhS_BE_SellingProductAPIService.GetSellingProduct(sellingProductId).then(function (sellingProduct) {
                sellingProductEntity = sellingProduct;
            });
        }

        function buildSellingProductObjFromScope() {
            var sellingProduct = {
                SellingProductId: (sellingProductId != null) ? sellingProductId : 0,
                Name: $scope.name,
                SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
            };
            return sellingProduct;
        }

        function loadFilterBySection() {
            if(sellingProductEntity!=undefined)
            {
                $scope.name = sellingProductEntity.Name;
            }
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
