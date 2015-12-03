(function (appControllers) {

    "use strict";

    SupplierManagementController.$inject = ['$scope'];

    function SupplierManagementController($scope) {

        defineScope();
        load();

        function defineScope() {
            $scope.scodeModal = {};
            $scope.scodeModal.testModel = 'QM_BE_SupplierManagementController';
        }

        function load() {

           


        }
    }

    appControllers.controller('QM_BE_SupplierManagementController', SupplierManagementController);
})(appControllers);