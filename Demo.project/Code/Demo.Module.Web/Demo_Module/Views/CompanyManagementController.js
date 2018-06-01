(function (appControllers)
{
    "use strict";

    companyManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_CompanyAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function companyManagementController($scope, VRNotificationService, Demo_Module_CompanyAPIService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService)
    {
        
        var gridApi;
        defineScope();
        load();
        function defineScope() {

            $scope.onGridReady = function (api) {
                var filter = {};
                gridApi = api;
                api.loadGrid(filter); 
            };

            $scope.searchClicked = function () {
                
                return gridApi.loadGrid(getFilter()); // because it returns a promise
            };

            function getFilter() {
                return {
                    Name: $scope.name
                };
            };

            $scope.addNewCompany = function () {
                var onCompanyAdded = function (company) {
                    if (gridApi != undefined) {
                        gridApi.onCompanyAdded(company);                        
                    }                    
                };
                Demo_Module_CompanyService.addCompany(onCompanyAdded);
            };                        
        };

        function load()
        {

        }
    };

appControllers.controller('Demo_Module_CompanyManagementController', companyManagementController);
})(appControllers);