(function (appControllers) {

    "use strict";

    DIDManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_DIDAPIService', 'Retail_BE_DIDService'];

    function DIDManagementController($scope, UtilsService, VRUIUtilsService, Retail_BE_DIDAPIService, Retail_BE_DIDService) {

        var gridAPI;

        var didNumberTypeSelectorAPI;
        var didNumberTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onDIDNumberTypeSelectorReady = function (api) {
                didNumberTypeSelectorAPI = api;
                didNumberTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        };

        function load() {
            $scope.isGettingData = true;
            loadAllControls();
        };

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDIDNumberTypeSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        };

        function loadDIDNumberTypeSelector() {
            var didNumberTypeSelectorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            didNumberTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(didNumberTypeSelectorAPI, undefined, didNumberTypeSelectorDirectiveLoadDeferred);
            });
            return didNumberTypeSelectorDirectiveLoadDeferred.promise;
        };

        function buildGridQuery() {
            return {
                Number: $scope.scopeModel.number,
                DIDNumberTypes: didNumberTypeSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_DIDManagementController', DIDManagementController);

})(appControllers);