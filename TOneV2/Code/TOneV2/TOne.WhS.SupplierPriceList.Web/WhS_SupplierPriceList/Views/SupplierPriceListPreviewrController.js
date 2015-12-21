(function (appControllers) {

    "use strict";

    supplierPriceListPreviewController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function supplierPriceListPreviewController($scope,  VRNotificationService, VRNavigationService, UtilsService) {

        
        defineScope();
        load();
        function defineScope() {
           
        }

        function load() {
               
          
        }

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
        
    }

    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewController', supplierPriceListPreviewController);
})(appControllers);
