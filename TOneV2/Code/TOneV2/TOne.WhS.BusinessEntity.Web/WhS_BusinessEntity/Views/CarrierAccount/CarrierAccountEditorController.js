(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_Be_CarrierAccountTypeEnum'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_CarrierAccountTypeEnum) {

        var carrierProfileDirectiveAPI;
        var carrierAccountId;
        var carrierProfileId;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
                carrierProfileId = parameters.CarrierProfileId
            }
            editMode = (carrierAccountId != undefined);
            $scope.disableCarrierProfile = ((carrierProfileId != undefined) && !editMode);

        }
        function defineScope() {
            $scope.SaveCarrierAccount = function () {
                    if (editMode) {
                        return updateCarrierAccount();
                    }
                    else {
                        return insertCarrierAccount();
                    }
            };
            $scope.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                load();
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
            $scope.isGettingData = true;
            if (carrierProfileDirectiveAPI == undefined)
                return;

            defineCarrierAccountTypes();
           
            carrierProfileDirectiveAPI.load().then(function () {
                if ($scope.disableCarrierProfile && carrierProfileId != undefined)
                {
                    carrierProfileDirectiveAPI.setData(carrierProfileId);
                    $scope.isGettingData = false;
                }   
                else if (editMode) {
                  getCarrierAccount();
                }
                else {
                    $scope.isGettingData = false;
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });

        }

        function getCarrierAccount() {
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccount(carrierAccountId).then(function (carrierAccount) {
                fillScopeFromCarrierAccountObj(carrierAccount);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildCarrierAccountObjFromScope() {
            var obj = {
                CarrierAccountId: (carrierAccountId != null) ? carrierAccountId : 0,
                Name: $scope.name,
                AccountType: $scope.selectedCarrierAccountType.value,
                CarrierProfileId:carrierProfileDirectiveAPI.getData().CarrierProfileId,
                SupplierSettings: {},
                CustomerSettings: {},
            };
            return obj;
        }

        function fillScopeFromCarrierAccountObj(carrierAccountObj) {
            $scope.name = carrierAccountObj.Name;
            for (var i = 0; i < $scope.carrierAccountTypes.length; i++)
                if (carrierAccountObj.AccountType == $scope.carrierAccountTypes[i].value)
                    $scope.selectedCarrierAccountType = $scope.carrierAccountTypes[i];
            if (carrierProfileId != undefined)
                carrierProfileDirectiveAPI.setData(carrierProfileId);
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
