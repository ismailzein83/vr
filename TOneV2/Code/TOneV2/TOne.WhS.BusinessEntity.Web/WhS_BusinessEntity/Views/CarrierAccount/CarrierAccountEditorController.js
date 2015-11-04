(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_Be_CarrierAccountTypeEnum','VRUIUtilsService'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_CarrierAccountTypeEnum, VRUIUtilsService) {

        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountId;
        var carrierProfileId;
        var carrierAccountEntity;
        var isEditMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
                carrierProfileId = parameters.CarrierProfileId
            }
            isEditMode = (carrierAccountId != undefined);
            $scope.disableCarrierProfile = ((carrierProfileId != undefined) && !isEditMode);

        }
        function defineScope() {
            $scope.SaveCarrierAccount = function () {
                if (isEditMode) {
                        return updateCarrierAccount();
                    }
                    else {
                        return insertCarrierAccount();
                    }
            };
            $scope.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                carrierProfileReadyPromiseDeferred.resolve();

            }

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.selectedCarrierAccountType;
            $scope.customerTabShow = function () {
                if ($scope.selectedCarrierAccountType!=undefined && $scope.selectedCarrierAccountType.value != WhS_Be_CarrierAccountTypeEnum.Supplier.value)
                    return true;
                return false;
            }
            $scope.SupplierTabShow = function () {
                if ($scope.selectedCarrierAccountType != undefined &&  $scope.selectedCarrierAccountType.value != WhS_Be_CarrierAccountTypeEnum.Customer.value)
                    return true;
                return false;
            }

        }

        function load() {
            $scope.isLoading = true;
            defineCarrierAccountTypes();
           
            if (isEditMode) {
                getCarrierAccount().then(function () {
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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadCarrierProfileDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }
        function loadCarrierProfileDirective() {

            var loadCarrierProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            carrierProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: carrierProfileId != undefined? carrierProfileId : undefined
                    }
                    VRUIUtilsService.callDirectiveLoad(carrierProfileDirectiveAPI, directivePayload, loadCarrierProfilePromiseDeferred);
                });

            return loadCarrierProfilePromiseDeferred.promise;
        }


        function getCarrierAccount() {
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccount(carrierAccountId).then(function (carrierAccount) {
                carrierAccountEntity = carrierAccount;
            });
        }

        function buildCarrierAccountObjFromScope() {
            var obj = {
                CarrierAccountId: (carrierAccountId != null) ? carrierAccountId : 0,
                Name: $scope.name,
                AccountType: $scope.selectedCarrierAccountType.value,
                CarrierProfileId: carrierProfileDirectiveAPI.getSelectedIds(),
                SupplierSettings: {},
                CustomerSettings: {},
            };
            return obj;
        }
        function loadFilterBySection() {
            if(carrierAccountEntity!=undefined)
            {
                $scope.name = carrierAccountEntity.Name;
                for (var i = 0; i < $scope.carrierAccountTypes.length; i++)
                    if (carrierAccountEntity.AccountType == $scope.carrierAccountTypes[i].value)
                        $scope.selectedCarrierAccountType = $scope.carrierAccountTypes[i];
            }
        }
      
        function insertCarrierAccount() {
            var carrierAccountObject = buildCarrierAccountObjFromScope();
            return WhS_BE_CarrierAccountAPIService.AddCarrierAccount(carrierAccountObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Carrier Account", response)) {
                    if ($scope.onCarrierAccountAdded != undefined)
                        $scope.onCarrierAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
      
        function defineCarrierAccountTypes() {
            $scope.carrierAccountTypes = [];
            for (var p in WhS_Be_CarrierAccountTypeEnum)
                $scope.carrierAccountTypes.push(WhS_Be_CarrierAccountTypeEnum[p]);

        }
        function updateCarrierAccount() {
            var carrierAccountObject = buildCarrierAccountObjFromScope();
            WhS_BE_CarrierAccountAPIService.UpdateCarrierAccount(carrierAccountObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Carrier Account", response)) {
                    if ($scope.onCarrierAccountUpdated != undefined)
                        $scope.onCarrierAccountUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountEditorController', carrierAccountEditorController);
})(appControllers);
