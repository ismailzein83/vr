(function (appControllers) { 

    "use strict";

    branchManagementController.$inject = ['$scope', 'Demo_Module_BranchService'];

    function branchManagementController($scope, Demo_Module_BranchService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) { 
                gridAPI = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () { 
                return gridAPI.load(getFilter());
            };

            $scope.scopeModel.addBranch = function () { 
                var onBranchAdded = function (branch) {
                    if (gridAPI != undefined)
                        gridAPI.onBranchAdded(branch);
                };

                Demo_Module_BranchService.addBranch(onBranchAdded);
            };
        };

        function load() {

        };

        function getFilter() {  
            return {
                query: {
                    Name: $scope.scopeModel.name
                }
            };
        };
    };

    appControllers.controller("Demo_Module_BranchManagementController", branchManagementController);
})(appControllers);