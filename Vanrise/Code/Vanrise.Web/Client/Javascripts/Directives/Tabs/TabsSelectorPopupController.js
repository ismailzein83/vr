(function (appControllers) {

    "use strict";

    TabsSelectorPopupController.$inject = ['$scope'];

    function TabsSelectorPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.selectTabAndCloseModal = function (tab) {
                tab.isSelected = true;
                closeModal();
            };

            $scope.closeModal = closeModal;

        }
        function load() {

        }

        function closeModal() {
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('TabsSelectorPopupController', TabsSelectorPopupController);
})(appControllers);
