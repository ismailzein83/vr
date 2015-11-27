(function (appControllers) {

    "use strict";

    saleCodeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function saleCodeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {


        var gridAPI;
       
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;            
               
            }
          
        }
        function load() {           
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoading = false;
             });
        }
      
        
       
        function setFilterObject() {
            filter = {                
                EffectiveOn: $scope.effectiveOn
            };
           
        }

    }

    appControllers.controller('WhS_BE_SaleCodeManagementController', saleCodeManagementController);
})(appControllers);