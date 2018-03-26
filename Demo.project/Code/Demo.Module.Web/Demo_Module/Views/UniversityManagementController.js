(function (appControllers) {

    "use strict";

    universityManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_UniversityAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_UniversityService', 'VRNavigationService'];

    function universityManagementController($scope, VRNotificationService, Demo_Module_UniversityAPIService, UtilsService, VRUIUtilsService, Demo_Module_UniversityService, VRNavigationService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getGridQuery());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getGridQuery());
            };

            $scope.addNewUniversity = addNewUniversity;
        }

        function load() {
        }

        function getGridQuery() {
            return {
                Name: $scope.name,
            };
        }

        function addNewUniversity() {
            var onUniversityAdded = function (university) {
                if (gridAPI != undefined)
                    gridAPI.onUniversityAdded(university);
            };

            Demo_Module_UniversityService.addUniversity(onUniversityAdded);
        }

    }

    appControllers.controller('Demo_Module_UniversityManagementController', universityManagementController);
})(appControllers);