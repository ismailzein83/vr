(function (appControllers) {

    "use strict";

    TextBoxValuesPopupController.$inject = ['$scope'];

    function TextBoxValuesPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
           
            $scope.closeModal = closeModal;

        }
        function load() {

        }

        function closeModal() {
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('TextBoxValuesPopupController', TextBoxValuesPopupController);
})(appControllers);
