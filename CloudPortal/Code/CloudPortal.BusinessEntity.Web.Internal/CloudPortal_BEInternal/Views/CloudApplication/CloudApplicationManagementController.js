(function (appControllers) {

    "use strict";

    BEInternal_CloudApplicationManagementController.$inject = ['$scope', 'CloudPortal_BEInternal_CloudApplicationService','CloudPortal_BEInternal_CloudApplicationAPIService'];

    function BEInternal_CloudApplicationManagementController($scope, CloudPortal_BEInternal_CloudApplicationService, CloudPortal_BEInternal_CloudApplicationAPIService) {
        var gridAPI;
        var filter = {};
        defineScope();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addCloudApplication = function () {

                var onCloudApplicationAdded = function (addedItem) {
                    var addedItemObj = { Entity: addedItem };
                    gridAPI.onCloudApplicationAdded(addedItemObj);
                };

                CloudPortal_BEInternal_CloudApplicationService.addCloudApplication(onCloudApplicationAdded);
            }

            $scope.hasSaveCloudApplicationPermission = function () {
                return CloudPortal_BEInternal_CloudApplicationAPIService.HasAddCloudApplicationPermission();
            }
        }

        function getFilterObject() {
            filter = {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('CloudPortal_BEInternal_CloudApplicationManagementController', BEInternal_CloudApplicationManagementController);
})(appControllers);