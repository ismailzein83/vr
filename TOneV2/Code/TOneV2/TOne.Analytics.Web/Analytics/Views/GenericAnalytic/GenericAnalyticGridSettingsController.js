(function (appControllers) {

    "use strict";

    genericAnalyticGridSettingsController.$inject = ['$scope', 'VRNavigationService'];
    function genericAnalyticGridSettingsController($scope, VRNavigationService) {

        defineScope();
        function defineScope() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                $scope.asr = parameters.asr;
                $scope.acd = parameters.acd;
                $scope.attempts = parameters.attempts;
            }

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.saveSettings = function () {
                var values = {
                    asr: $scope.asr,
                    acd: $scope.acd,
                    attempts: $scope.attempts
                };

                $scope.onSaveSettings(values);
                $scope.modalContext.closeModal();
            };
        }
    }
    appControllers.controller('GenericAnalyticGridSettingsController', genericAnalyticGridSettingsController);

})(appControllers);