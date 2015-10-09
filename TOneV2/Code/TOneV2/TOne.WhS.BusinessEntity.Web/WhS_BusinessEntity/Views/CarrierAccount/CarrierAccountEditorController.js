(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_CarrierAccountEnum'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_CarrierAccountEnum) {

        var carrierProfileDirectiveAPI;
        var carrierAccountId;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
            }
            editMode = (carrierAccountId != undefined);
            $scope.disablePricingProduct = (parameters != undefined);
        }
        function defineScope() {
            $scope.SavePricingProduct = function () {
                return insertCarrierAccount();
            };
            $scope.onCarrierProfileDirectiveLoaded = function (api) {
                carrierProfileDirectiveAPI = api;
            }
            $scope.onCarrierProfileSelectionChanged = function () {
                if (carrierProfileDirectiveAPI!=undefined)
                console.log(carrierProfileDirectiveAPI.getData())
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.selectedCarrierAccountType;


        }

        function load() {
            $scope.isGettingData = true;
            defineCarrierAccountTypes();
            if(editMode)
                {
                getCarrierAccount();
                }
            else {
                    $scope.isGettingData = false;
             }
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
            var obj = {
                    Name: $scope.name,
                    AccountType: $scope.selectedCarrierAccountType.value,
                    CarrierProfileId:1,
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
    }

    appControllers.controller('WhS_BE_CarrierAccountEditorController', carrierAccountEditorController);
})(appControllers);
