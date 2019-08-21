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
            $scope.isLastFullView = function () {
                if ($scope.ctrl.parentNames && $scope.ctrl.parentNames.length > 1)
                    return $scope.objectTypeId == $scope.ctrl.parentNames[$scope.ctrl.parentNames.length - 1].objectTypeId;
                else return true;
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
