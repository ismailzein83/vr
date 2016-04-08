(function (appControllers) {

    "use strict";

    sellingProductEditorController.$inject = ['$scope', 'WhS_BE_SellingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

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

            $scope.hasSaveSellingProductPermission = function () {
                if (isEditMode) {
                    return WhS_BE_SellingProductAPIService.HasUpdateSellingProductPermission();
                }
                else {
                    return WhS_BE_SellingProductAPIService.HasAddSellingProductPermission();
                }
            }

            $scope.scopeModal = {}
            $scope.scopeModal.isEditMode = isEditMode;
            $scope.scopeModal.saveSellingProduct = function () {
                if (isEditMode) {
                    return updateSellingProduct();
                }
                else {
                    return insertSellingProduct();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onSellingNumberPlansDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getSellingProduct().then(function () {
                    loadAllControls()
                        .finally(function () {
                            sellingProductEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getSellingProduct() {
            return WhS_BE_SellingProductAPIService.GetSellingProduct(sellingProductId).then(function (sellingProduct) {
                sellingProductEntity = sellingProduct;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadFilterBySection, loadSellingNumberPlans])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModal.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(sellingProductEntity ? sellingProductEntity.Name : null, 'Selling Product') : UtilsService.buildTitleForAddEditor('Selling Product');
        }

        function loadFilterBySection() {
            if (sellingProductEntity != undefined) {
                $scope.scopeModal.name = sellingProductEntity.Name;
            }
        }

        function loadSellingNumberPlans() {
            var sellingNumberPlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: sellingProductEntity != undefined ? sellingProductEntity.SellingNumberPlanId : undefined
                    }

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, directivePayload, sellingNumberPlanLoadPromiseDeferred);
                });
            return sellingNumberPlanLoadPromiseDeferred.promise;
        }

        function insertSellingProduct() {
            var sellingProductObject = buildSellingProductObjFromScope();
            return WhS_BE_SellingProductAPIService.AddSellingProduct(sellingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Selling product", response, "name")) {
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
                if (VRNotificationService.notifyOnItemUpdated("Selling product", response, "name")) {
                    if ($scope.onSellingProductUpdated != undefined)
                        $scope.onSellingProductUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function buildSellingProductObjFromScope() {
            var sellingProduct = {
                SellingProductId: (sellingProductId != null) ? sellingProductId : 0,
                Name: $scope.scopeModal.name,
                SellingNumberPlanId: (isEditMode ? undefined : sellingNumberPlanDirectiveAPI.getSelectedIds())
            };
            return sellingProduct;
        }
    }

    appControllers.controller('WhS_BE_SellingProductEditorController', sellingProductEditorController);

})(appControllers);
