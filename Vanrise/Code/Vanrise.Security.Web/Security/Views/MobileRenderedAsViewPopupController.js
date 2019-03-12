(function (appControllers) {

    "use strict";

    MobileRenderedAsViewPopupController.$inject = ['$scope'];

    function MobileRenderedAsViewPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.closeModal = closeModal;
        }
        function load() {

        }
        $scope.selectViewAndCloseModal = function (a) {
            closeModal();
            window.location.href = a.Location;
        };
        function closeModal() {
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('MobileRenderedAsViewPopupController', MobileRenderedAsViewPopupController);
})(appControllers);
