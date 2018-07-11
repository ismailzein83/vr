(function (appControllers) {

    "use strict";

    AllActionsModalPopupController.$inject = ['$scope'];

    function AllActionsModalPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.clickActionAndCloseModal = function (btn,actionBtn) {
                if (actionBtn == undefined && btn.menuActions != undefined && btn.menuActions.length > 0) {
                   // btn.showSubMenu = !btn.showSubMenu;
                    return;
                }
                else if (actionBtn != undefined) {
                    btn.menuActionClicked(actionBtn);
                }
                else {
                    btn.onInternalClick();
                }
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

    appControllers.controller('AllActionsModalPopupController', AllActionsModalPopupController);
})(appControllers);
