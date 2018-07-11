(function (appControllers) {

    "use strict";

    MenuActionsModalPopup.$inject = ['$scope'];

    function MenuActionsModalPopup($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.clickActionAndCloseModal = function (btn,actionBtn) {  
                btn.menuActionClicked(actionBtn);
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

    appControllers.controller('MenuActionsModalPopup', MenuActionsModalPopup);
})(appControllers);
