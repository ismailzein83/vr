(function (appControllers) {

    "use strict";

    GridMenuActionPopupController.$inject = ['$scope'];

    function GridMenuActionPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.exportActionAndCloseModal = function () {
                $scope.ctrl.onExportClicked();
                closeModal();
            };

            $scope.executeGridActionAndCloseModal = function (menuAction) {
                $scope.ctrl.executeGridAction(menuAction);
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

    appControllers.controller('GridMenuActionPopupController', GridMenuActionPopupController);
})(appControllers);
