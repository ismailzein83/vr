(function (appControllers) {

    "use strict";

    carrierProfileEditorController.$inject = ['$scope', 'WhS_BE_CarrierProfileAPIService', 'WhS_BE_TechnicalSettingsAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function carrierProfileEditorController($scope, WhS_BE_CarrierProfileAPIService, WhS_BE_TechnicalSettingsAPIService, UtilsService, VRNotificationService, VRNavigationService) {
        var customers;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                customers = parameters.customers;
            }
        }

        function defineScope() {
            $scope.scopeModal = {
                customers: [],
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
        }
        function loadStaticData() {
            $scope.scopeModal.customers = customers;
        }


        $scope.scopeModal.cancelSendSalePricelist = function () {
            if ($scope.onCancelSendSalePricelist != undefined)
                $scope.onCancelSendSalePricelist();
        };

        $scope.scopeModal.continueSendSalePricelist = function () {
            if ($scope.onContinueSendSalePricelist != undefined)
                $scope.onContinueSendSalePricelist();
        };

    }

    appControllers.controller('WhS_BE_SendSalePricelistConfirmationController', carrierProfileEditorController);
})(appControllers);
