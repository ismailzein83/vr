(function (appControllers) {

    "use strict";

    customerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function customerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var customerEntity;

        loadParameters();
        load();

        function loadParameters() {

        }

        function load() {
          
        }
    }
    appControllers.controller('CP_SupplierPricelist_CustomerEditorController', customerEditorController);
})(appControllers);