(function (appControllers) {

    "use strict";

    RowActionPopupController.$inject = ['$scope'];

    function RowActionPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.clickActionAndCloseModal = function (action, dataItem) {
                $scope.ctrl.menuActionClicked(action, dataItem);
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

    appControllers.controller('RowActionPopupController', RowActionPopupController);
})(appControllers);
