(function (appControllers) {

    "use strict";

    supplierEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierManagmentAPIService'];

    function supplierEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, supplierManagmentAPIService) {

        var isEditMode;
        var supplierEntity;
        var userDirectiveApi;
        var userReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            
        }
        function defineScope() {

            $scope.close = function () {
                $scope.modalContext.closeModal();
            }
            $scope.onUserDirectiveReady = function (api) {
                userDirectiveApi = api;
                userReadyPromiseDeferred.resolve();
            }
        }
        function load() {
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, LoadUser])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            //if (isEditMode && customerEntity != undefined)
            //    $scope.title = UtilsService.buildTitleForUpdateEditor(customerEntity.Name, "Supplier");
            //else
                $scope.title = UtilsService.buildTitleForAddEditor("Supplier");
        }


        function LoadUser() {        
            var userLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            userReadyPromiseDeferred.promise.then(function () {
             
                VRUIUtilsService.callDirectiveLoad(userDirectiveApi, undefined, userLoadPromiseDeferred);
            });
            return userLoadPromiseDeferred.promise;
        }

       
    }
    appControllers.controller('CP_SupplierPricelist_SupplierEditorController', supplierEditorController);
})(appControllers);