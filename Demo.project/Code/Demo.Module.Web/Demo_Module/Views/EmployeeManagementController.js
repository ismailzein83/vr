(function (appControllers) {
    "use strict";

    employeeManagementController.$inject = ['$scope', 'Demo_Module_EmployeeService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function employeeManagementController($scope, Demo_Module_EmployeeService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var gridApi;
        defineScope();
      load();

        function defineScope() {

            $scope.scopeModel = {};


            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };

            $scope.scopeModel.addEmployee = function () {
                var onEmployeeAdded = function (employee) {
                    if (gridApi != undefined) {
                        gridApi.onEmployeeAdded(employee);
                    }
                };
           

                Demo_Module_EmployeeService.addEmployee(onEmployeeAdded);

            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

        };


        function loadAllControls() {
           
                 $scope.scopeModel.isLoading = false;
             
        };
   

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                }
            };
        };







    };

    appControllers.controller('Demo_Module_EmployeeManagementController', employeeManagementController);
})(appControllers);