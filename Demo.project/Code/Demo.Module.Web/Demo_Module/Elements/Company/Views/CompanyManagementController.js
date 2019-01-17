(function (appControllers) { // review addCompany !!

    "use strict";

    companyManagementController.$inject = ['$scope', 'Demo_Module_CompanyService'];

    function companyManagementController($scope, Demo_Module_CompanyService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) { // 1-obtaining the api on grid ready 2-scan the text boxes 3- load the grid according to this scan  
                gridAPI = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () { //  1-scan the text boxes 2-load the grid according to this scan
                return gridAPI.load(getFilter());
            };

            $scope.scopeModel.addCompany = function () { // 1-call the company service and pass the onCompany function 2-onCompanyAdded call the function that add the company to the grid

                var onCompanyAdded = function (company) { // after adding it to the DB return to here from the editor and ...
                    if (gridAPI != undefined)
                        gridAPI.onCompanyAdded(company); // then go the grid and update it
                };

                Demo_Module_CompanyService.addCompany(onCompanyAdded);
            };
        };

        function load() {

        };

        function getFilter() {   // scan the text box 
            return {
                Name: $scope.scopeModel.name,
            };
        };

    };

    appControllers.controller("Demo_Module_CompanyManagementController", companyManagementController);
})(appControllers);