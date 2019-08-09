(function (appControllers) {

    "use strict";

    RowFullScreenModeModalPopupController.$inject = ['$scope'];

    function RowFullScreenModeModalPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.closeFullViewAndCloseModal = function (dataItem, $event) {
                $scope.ctrl.switchFullScreenModeOff(dataItem, $event);
                closeModal();
            };

        }
        function load() {

        }

        function closeModal() {
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('RowFullScreenModeModalPopupController', RowFullScreenModeModalPopupController);
})(appControllers);
