(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'Demo_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'Demo_CarrierAccountTypeEnum', 'VRUIUtilsService', 'Demo_CarrierAccountActivationStatusEnum'];

    function carrierAccountEditorController($scope, Demo_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, Demo_CarrierAccountTypeEnum, VRUIUtilsService, Demo_CarrierAccountActivationStatusEnum) {
        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;

        var isEditMode;
        $scope.scopeModal = {};

        var carrierAccountId;
        var carrierProfileId;
        var carrierAccountEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
                carrierProfileId = parameters.CarrierProfileId
            }
            isEditMode = (carrierAccountId != undefined);
            $scope.scopeModal.disableCarrierProfile = ((carrierProfileId != undefined));
        }

        function defineScope() {
            $scope.scopeModal.SaveCarrierAccount = function () {
                if (isEditMode) {
                    return updateCarrierAccount();
                } else {
                    return insertCarrierAccount();
                }
            };

            $scope.scopeModal.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
            }

            $scope.scopeModal.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                carrierProfileReadyPromiseDeferred.resolve();

            }

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getCarrierAccount()
                    .then(function () {
                        loadAllControls()
                            .finally(function () {
                                carrierAccountEntity = undefined;
                            });
                    })
                    .catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModal.isLoading = false;
                    });
            } else {
                loadAllControls();
            }

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadFilterBySection, loadCarrierProfileDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(carrierAccountEntity ? carrierAccountEntity.NameSuffix : null, 'Carrier Account') : UtilsService.buildTitleForAddEditor('Carrier Account');
        }

        function loadCarrierProfileDirective() {

            var loadCarrierProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            carrierProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (carrierAccountEntity != undefined ? carrierAccountEntity.CarrierProfileId : (carrierProfileId != undefined ? carrierProfileId : undefined))
                    }
                    VRUIUtilsService.callDirectiveLoad(carrierProfileDirectiveAPI, directivePayload, loadCarrierProfilePromiseDeferred);
                });

            return loadCarrierProfilePromiseDeferred.promise;
        }

        function getCarrierAccount() {
            return Demo_CarrierAccountAPIService.GetCarrierAccount(carrierAccountId)
                .then(function (carrierAccount) {
                    carrierAccountEntity = carrierAccount;
                });
        }

        function buildCarrierAccountObjFromScope() {
            var obj = {
                CarrierAccountId: (carrierAccountId != null) ? carrierAccountId : 0,
                NameSuffix: $scope.scopeModal.name,
                CarrierProfileId: carrierProfileDirectiveAPI.getSelectedIds(),
                SupplierSettings: {},
                CustomerSettings: {},
            };
            return obj;
        }

        function loadFilterBySection() {
            if (carrierAccountEntity != undefined) {
                $scope.scopeModal.name = carrierAccountEntity.NameSuffix;
            }
        }


        function insertCarrierAccount() {
            $scope.scopeModal.isLoading = true;

            var carrierAccountObject = buildCarrierAccountObjFromScope();
            return Demo_CarrierAccountAPIService.AddCarrierAccount(carrierAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Carrier Account", response, "Name")) {
                        if ($scope.onCarrierAccountAdded != undefined)
                            $scope.onCarrierAccountAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                });

        }

        function updateCarrierAccount() {
            $scope.scopeModal.isLoading = true;

            var carrierAccountObject = buildCarrierAccountObjFromScope();
            Demo_CarrierAccountAPIService.UpdateCarrierAccount(carrierAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Carrier Account", response, "Name")) {
                        if ($scope.onCarrierAccountUpdated != undefined)
                            $scope.onCarrierAccountUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }
    }

    appControllers.controller('Demo_CarrierAccountEditorController', carrierAccountEditorController);
})(appControllers);