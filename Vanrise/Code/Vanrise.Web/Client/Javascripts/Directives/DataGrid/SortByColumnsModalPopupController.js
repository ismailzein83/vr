(function (appControllers) {

    "use strict";

    SortByColumnsModalPopupController.$inject = ['$scope'];

    function SortByColumnsModalPopupController($scope) {
        defineScope();
        load();
        function loadParameters() {

        }
        function defineScope() {
            $scope.sortByColumnAndCloseModal = function (colDef) {
                var sortDirection = $scope.selectedSortDirectionIndex == 0 ? "ASC" : "DESC";
                colDef.onSort(sortDirection);
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

    appControllers.controller('SortByColumnsModalPopupController', SortByColumnsModalPopupController);
})(appControllers);
