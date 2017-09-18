(function (appControllers) {

    "use strict";

    disputeCaseManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_DisputeCaseAPIService', 'UtilsService', 'VRUIUtilsService',  'VRNavigationService'];

    function disputeCaseManagementController($scope, VRNotificationService, Demo_Module_DisputeCaseAPIService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var gridAPI;
        var filter = {};
         
        defineScope();
        load();
        function defineScope() {
            $scope.searchClicked = function () {
                setfilterdobject()
                return gridAPI.loadGrid(filter);
            };

            function setfilterdobject() {
                filter = {
                    CaseNumber: $scope.caseNumber,
                };
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addNewDisputeCase = addNewDisputeCase;
        }

        function load() {

        }

        function addNewDisputeCase() {
         
        }
    }




    appControllers.controller('Demo_Module_DisputeCaseManagementController', disputeCaseManagementController);
})(appControllers);