(function (appControllers) {

    "use strict";

    DIDManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_DIDAPIService', 'Retail_BE_DIDService'];

    function DIDManagementController($scope, UtilsService, VRUIUtilsService, Retail_BE_DIDAPIService, Retail_BE_DIDService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                return gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.add = function () {
                var onDIDAdded = function (addedDID) {
                    gridAPI.onDIDAdded(addedDID);
                };

                Retail_BE_DIDService.addDID(onDIDAdded);
            };

            $scope.scopeModel.hasAddDIDPermission = function () {
                return Retail_BE_DIDAPIService.HasAddDIDPermission();
            };


            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
        function load() {

        }

        function buildGridQuery() {
            return {
                Number: $scope.scopeModel.number
            };
        }
    }

    appControllers.controller('Retail_BE_DIDManagementController', DIDManagementController);

})(appControllers);