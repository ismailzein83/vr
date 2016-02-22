(function (appControllers) {

    "use strict";

    customerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_SupplierPricelist_CustomerManagmentAPIService'];

    function customerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, customerManagmentAPIService) {

        var isEditMode;
        var customerEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            
        }
        function defineScope() {
            $scope.sourceTypeTemplates = [];
            $scope.close = function() {
                $scope.modalContext.closeModal();
            }
        }
        function load() {
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, LoadCustomerTemplates])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && customerEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(customerEntity.Name, "Customer");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Customer");
        }

        function LoadCustomerTemplates() {
            
            customerManagmentAPIService.GetCustomerTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.sourceTypeTemplates.push(item);
                });
            });
        }
    }
    appControllers.controller('CP_SupplierPricelist_CustomerEditorController', customerEditorController);
})(appControllers);