(function (appControllers) {

    "use strict";

    MobileOptionPopupController.$inject = ['$scope'];

    function MobileOptionPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.selectSingleItemAndCloseModal = function ($event, c) {
                $scope.ctrl.selectValue($event, c);
                closeModal();
            };

            $scope.clearSelectionAndCloseModal = function ($event, isSingle) {
                $scope.ctrl.clearAllSelected($event, isSingle);
                if (isSingle)
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

    appControllers.controller('MobileOptionPopupController', MobileOptionPopupController);
})(appControllers);
