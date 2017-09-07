(function (appControllers) {

    "use strict";

    sellingProductEditorController.$inject = ['$scope', 'WhS_BE_SellingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_BE_SalePricelistAPIService', 'WhS_BE_SalePriceListOwnerTypeEnum'];

    function sellingProductEditorController($scope, WhS_BE_SellingProductAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_BE_SalePricelistAPIService, WhS_BE_SalePriceListOwnerTypeEnum) {
        var isEditMode;
        var sellingProductId;
        var fixedSellingNumberPlanId;
        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var sellingProductEntity;
        var context;
        var isViewHistoryMode;

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingProductId = parameters.SellingProductId;
                fixedSellingNumberPlanId = parameters.sellingNumberPlanId;
                context = parameters.context;
            }
            isEditMode = (sellingProductId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }

        function defineScope() {

            $scope.hasSaveSellingProductPermission = function () {
                if (isEditMode) {
                    return WhS_BE_SellingProductAPIService.HasUpdateSellingProductPermission();
                }
                else {
                    return WhS_BE_SellingProductAPIService.HasAddSellingProductPermission();
                }
            };

            $scope.scopeModal = {};
            $scope.scopeModal.canEditCurrency = true;
            $scope.scopeModal.isEditMode = isEditMode;
            $scope.scopeModal.fixedSellingNumberPlanID = fixedSellingNumberPlanId != undefined;
            $scope.scopeModal.saveSellingProduct = function () {
                if (isEditMode) {
                    return updateSellingProduct();
                }
                else {
                    return insertSellingProduct();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModal.onSellingNumberPlansDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };
            $scope.scopeModal.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getSellingProduct().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getSellingProductHistory().then(function () {
                    loadAllControls().finally(function () {
                        sellingProductEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }

            else {
                loadAllControls();
            }
        }

        function getSellingProductHistory() {
            return WhS_BE_SellingProductAPIService.GetSellingProductHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                sellingProductEntity = response;

            });
        }

        function getSellingProduct() {
            return WhS_BE_SellingProductAPIService.GetSellingProduct(sellingProductId).then(function (sellingProduct) {
                sellingProductEntity = sellingProduct;
            });
        }

        function loadAllControls() {

            var asyncOperations = [setTitle, loadFilterBySection, loadSellingNumberPlans, loadCurrencySelector];

            if (isEditMode)
                asyncOperations.push(checkIfAnyPriceListExists);

            return UtilsService.waitMultipleAsyncOperations(asyncOperations)
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  sellingProductEntity = undefined;
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

        function checkIfAnyPriceListExists() {
            return WhS_BE_SalePricelistAPIService.CheckIfAnyPriceListExists(WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value, sellingProductId).then(function (response) {
                $scope.scopeModal.canEditCurrency = !response;
            });
        }

        function loadCurrencySelector() {
            var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            currencySelectorReadyDeferred.promise.then(function () {
                var payload;
                if (sellingProductEntity != undefined && sellingProductEntity.Settings != undefined) {
                    payload = {
                        selectedIds: sellingProductEntity.Settings.CurrencyId
                    }
                }
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, payload, currencySelectorLoadDeferred);
            });

            return currencySelectorLoadDeferred.promise;
        }

        function loadSellingNumberPlans() {
            var sellingNumberPlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: sellingProductEntity != undefined ? sellingProductEntity.SellingNumberPlanId : undefined,
                        selectifsingleitem: (!isEditMode) ? true : false
                    };
                    if (fixedSellingNumberPlanId != undefined)
                        directivePayload.selectedIds = fixedSellingNumberPlanId;
                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, directivePayload, sellingNumberPlanLoadPromiseDeferred);
                });
            return sellingNumberPlanLoadPromiseDeferred.promise;
        }

        function insertSellingProduct() {
            $scope.scopeModal.isLoading = true;
            return WhS_BE_SellingProductAPIService.AddSellingProduct(buildSellingProductObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Selling product", response, "name")) {
                    if ($scope.onSellingProductAdded != undefined)
                        $scope.onSellingProductAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }

        function updateSellingProduct() {
            $scope.scopeModal.isLoading = true;
            return WhS_BE_SellingProductAPIService.UpdateSellingProduct(buildSellingProductObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Selling product", response, "name")) {
                    if ($scope.onSellingProductUpdated != undefined)
                        $scope.onSellingProductUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }

        function buildSellingProductObjFromScope() {
            var obj = {
                SellingProductId: (sellingProductId != null) ? sellingProductId : 0,
                Name: $scope.scopeModal.name
            };

            obj.Settings = {
                CurrencyId: currencySelectorAPI.getSelectedIds()
            };

            if (!isEditMode) {
                obj.SellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
            }

            return obj;
        }
    }

    appControllers.controller('WhS_BE_SellingProductEditorController', sellingProductEditorController);

})(appControllers);
