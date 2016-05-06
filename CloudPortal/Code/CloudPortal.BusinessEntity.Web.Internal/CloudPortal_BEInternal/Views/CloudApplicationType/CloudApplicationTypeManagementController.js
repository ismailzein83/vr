(function (appControllers) {

    "use strict";

    BEInternal_CloudApplicationTypeManagementController.$inject = ['$scope', 'CloudPortal_BEInternal_CloudApplicationTypeService'];

    function BEInternal_CloudApplicationTypeManagementController($scope, CloudPortal_BEInternal_CloudApplicationTypeService) {
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

            $scope.addCloudApplicationType = function () {

                var onCloudApplicationTypeAdded = function (addedItem) {
                    var addedItemObj = { Entity: addedItem };
                    gridAPI.onCloudApplicationTypeAdded(addedItemObj);
                };

                CloudPortal_BEInternal_CloudApplicationTypeService.addCloudApplicationType(onCloudApplicationTypeAdded);
            }
        }

        function getFilterObject() {
            filter = {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('CloudPortal_BEInternal_CloudApplicationTypeManagementController', BEInternal_CloudApplicationTypeManagementController);
})(appControllers);