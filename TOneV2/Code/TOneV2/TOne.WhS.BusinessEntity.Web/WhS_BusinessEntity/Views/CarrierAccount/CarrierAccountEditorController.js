(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_CarrierAccountEnum'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_CarrierAccountEnum) {

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
            $scope.disablePricingProduct = (parameters != undefined);
            $scope.disableCarrierProfile = (carrierProfileId != undefined);

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
                if (carrierProfileId != undefined)
                    carrierProfileDirectiveAPI.setData(carrierProfileId);
            }

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.selectedCarrierAccountType;


        }

        function load() {
            $scope.isGettingData = true;
            defineCarrierAccountTypes();

            carrierProfileDirectiveAPI.load().then(function () {
                if (editMode) {
                    getCarrierAccount();
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
        function fillScopeFromCarrierAccountObj(carrierAccountObj) {
            $scope.name = carrierAccountObj.Name;
            for (var i = 0; i < $scope.carrierAccountTypes.length; i++)
                if (carrierAccountObj.AccountType == $scope.carrierAccountTypes[i].value)
                    $scope.selectedCarrierAccountType = $scope.carrierAccountTypes[i];
            if (carrierProfileDirectiveAPI != undefined)
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
        function buildCarrierAccountObjFromScope() {
            var carrierProfileId;
            if (carrierProfileDirectiveAPI != undefined)
                carrierProfileId = carrierProfileDirectiveAPI.getData().CarrierProfileId;
            var obj = {
                     CarrierAccountId: (carrierAccountId != null) ? carrierAccountId : 0,
                    Name: $scope.name,
                    AccountType: $scope.selectedCarrierAccountType.value,
                    CarrierProfileId: carrierProfileId,
                    SupplierSettings: {},
                    CustomerSettings: {},
                };
            return obj;
        }
        function defineCarrierAccountTypes() {
            $scope.carrierAccountTypes = [];
            for (var p in WhS_Be_CarrierAccountEnum)
                $scope.carrierAccountTypes.push(WhS_Be_CarrierAccountEnum[p]);

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
