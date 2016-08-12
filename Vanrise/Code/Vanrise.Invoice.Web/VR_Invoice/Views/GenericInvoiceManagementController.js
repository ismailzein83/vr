(function (appControllers) {

    "use strict";

    genericInvoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService','VRNavigationService'];

    function genericInvoiceManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService) {
        var invoiceTypeId;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }

        function defineScope() {
            $scope.searchClicked = function () {
            };
            $scope.generateInvoice = generateInvoice;
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }

        function generateInvoice() {
            var onGenerateInvoice = function (invoice) {
            };

            VR_Invoice_GenericInvoiceService.generateInvoice(onGenerateInvoice, invoiceTypeId);
        }

    }

    appControllers.controller('VR_Invoice_GenericInvoiceManagementController', genericInvoiceManagementController);
})(appControllers);