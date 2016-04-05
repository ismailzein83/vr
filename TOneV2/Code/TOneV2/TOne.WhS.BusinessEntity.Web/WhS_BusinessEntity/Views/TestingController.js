(function (appControllers) {

    "use strict";

    carrierAccountManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'WhS_BE_CarrierAccountTypeEnum', 'VRUIUtilsService'];

    function carrierAccountManagementController($scope, UtilsService, VRNotificationService, WhS_BE_CarrierAccountTypeEnum, VRUIUtilsService) {
        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.selectedCarrierAccountTypes = [];

            $scope.onCarrierTypeSelectionChanged = function () {
                console.log('SelectionChanged');
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountType])
                .catch(function (error) {
                    $scope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }

        function loadCarrierAccountType() {
            $scope.carrierAccountTypes = UtilsService.getArrayEnum(WhS_BE_CarrierAccountTypeEnum);
        }


    }

    appControllers.controller('WhS_BE_TestingController', carrierAccountManagementController);
})(appControllers);