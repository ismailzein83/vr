(function (appControllers) {
    "use strict";

    branchManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_BranchAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_BranchService'];

    function branchManagementController($scope, VRNotificationService, Demo_Module_BranchAPIService, UtilsService, VRUIUtilsService, Demo_Module_BranchService) {
       
        var gridApi;

        var companyDirectiveApi;
        var companyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.companies = [];

            $scope.onGridReady = function (api) {
                var filter = {};
                gridApi = api;
                api.loadGrid(filter); 
            };

            $scope.onCompanyDirectiveReady = function (api) {
                companyDirectiveApi = api;
                companyReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
              
                return gridApi.loadGrid(getFilter());
            };

            function getFilter() {
                return {
                    Name: $scope.name,
                    CompanyIds: companyDirectiveApi.getSelectedIds()
                };
            };

            $scope.addNewBranch = function () {
                var onBranchAdded = function (branch) {
                    if (gridApi != undefined) {
                        gridApi.onBranchAdded(branch);                        
                    }
                };
                Demo_Module_BranchService.addBranch(onBranchAdded);
            };
        };

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        };
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCompanySelector])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilters = false;
             });
        };

        function loadCompanySelector() {
            var companyLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            companyReadyPromiseDeferred.promise
            .then(function () {
                VRUIUtilsService.callDirectiveLoad(companyDirectiveApi, undefined, companyLoadPromiseDeferred);
            });
            return companyLoadPromiseDeferred.promise;
        };
    };

    appControllers.controller('Demo_Module_BranchManagementController',  branchManagementController);
})(appControllers);