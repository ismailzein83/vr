(function (appControllers) {
    "use strict";

    schoolManagementController.$inject = ['$scope', 'Demo_Module_SchoolService'];

    function schoolManagementController($scope, Demo_Module_SchoolService) {

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


            $scope.scopeModel.addSchool = function () {
                var onSchoolAdded = function (school) {
                    if (gridApi != undefined) {
                        gridApi.onSchoolAdded(school);
                    }
                };
                Demo_Module_SchoolService.addSchool(onSchoolAdded);
            };
        };

        function load() {

        }


        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                    Age: $scope.scopeModel.age
                }
            };
        };

    };

    appControllers.controller('Demo_Module_SchoolManagementController', schoolManagementController);
})(appControllers);