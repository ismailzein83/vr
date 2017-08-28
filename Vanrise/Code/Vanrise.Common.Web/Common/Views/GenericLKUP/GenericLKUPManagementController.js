(function (appControllers) {

    "use strict";
    
    GenericLKUPManagementController.$inject = ['$scope', 'VR_Common_GenericLKUPService','VR_Common_GnericLKUPAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function GenericLKUPManagementController($scope, VR_Common_GenericLKUPService, VR_Common_GnericLKUPAPIService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var gridAPI;
        var genericLKUPDefinitionSelectorApi;
        var genericLKUPDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.onGenericLKUPDefinitionSelectorReady = function (api) {
                genericLKUPDefinitionSelectorApi = api;
                genericLKUPDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.add = function () {
                var onGenericLKUPAdded = function (addedGenericLKUP) {

                    gridAPI.onGenericLKUPAdded(addedGenericLKUP);
                };
                VR_Common_GenericLKUPService.addGenericLKUP(onGenericLKUPAdded);
            };

            $scope.hasAddSGenericLKUPItemPermission = function () {
                return VR_Common_GnericLKUPAPIService.HasAddGenericLKUPItemPermission();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {
            $scope.isloading = true;
            loadAllControls().finally(function () {
                $scope.isloading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isloading = false;
            })
        }

        function loadAllControls() {
            function loadGenericLKUPDefinitionSelector() {
                var genericLKUPDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                genericLKUPDefinitionSelectorPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                    };
                    VRUIUtilsService.callDirectiveLoad(genericLKUPDefinitionSelectorApi, payloadSelector, genericLKUPDefinitionSelectorLoadDeferred);
                });
                return genericLKUPDefinitionSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadGenericLKUPDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
                BusinessEntityDefinitionIds: genericLKUPDefinitionSelectorApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_Common_GenericLKUPManagementController', GenericLKUPManagementController);
})(appControllers);