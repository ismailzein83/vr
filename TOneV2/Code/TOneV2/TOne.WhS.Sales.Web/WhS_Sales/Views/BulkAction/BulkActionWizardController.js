(function (appControllers) {

    "use strict";

    BulkActionWizardController.$inject = ["$scope"];

    function BulkActionWizardController($scope) {

        var wizardSteps = [];

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                
            }
        }

        function defineScope() {
            //Make title dynamic and change according to step
            $scope.title = "Bulk Action";

            $scope.wizardContext = {};

            wizardContext.getStep = function (stepName) {

            };

            $scope.moveToNextStep = function () {

                
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

        }
    }

    appControllers.controller("WhS_Sales_BulkActionWizardController", BulkActionWizardController);

})(appControllers);
