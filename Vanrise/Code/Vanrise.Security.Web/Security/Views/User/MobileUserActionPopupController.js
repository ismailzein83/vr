(function (appControllers) {

    "use strict";

    MobileUserActionPopupController.$inject = ['$scope'];

    function MobileUserActionPopupController($scope) {
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

    appControllers.controller('MobileUserActionPopupController', MobileUserActionPopupController);
})(appControllers);
