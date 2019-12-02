(function (appControllers) {

    "use strict";

    namedPersonalizationItemsEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", "VR_Common_EntityPersonalizationAPIService"];

    function namedPersonalizationItemsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Common_EntityPersonalizationAPIService) {

        var entityUniqueNames;
        var personilizationItemName;
        var selectedIsGlobal;
        var sharedSettingsGridAPI;
        var sharedSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var mySettingsGridAPI;
        var mySettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                entityUniqueNames = parameters.entityUniqueNames;
                personilizationItemName = parameters.personilizationItemName;
                selectedIsGlobal = parameters.selectedIsGlobal;
            }
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.onSharedSettingsGridReady = function (api) {
                sharedSettingsGridAPI = api;
                sharedSettingsGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onMySettingsGridReady = function (api) {
                mySettingsGridAPI = api;
                mySettingsGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onSettingsClicked = function (data) {
                if ($scope.onSettingsClicked != undefined) {
                    $scope.onSettingsClicked(data);
                    $scope.modalContext.closeModal();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {

            $scope.scopeModel.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadSharedSettingsGrid, loadMySettingsGrid])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function loadSharedSettingsGrid() {
            var sharedSettingsGridLoadDeferred = UtilsService.createPromiseDeferred();
            sharedSettingsGridReadyDeferred.promise.then(function () {
                var payload = {
                    query: {
                        EntityUniqueNames: entityUniqueNames,
                        IsGlobal: true
                    },
                    selectedName: personilizationItemName,
                    selectedIsGlobal: selectedIsGlobal
                };
                VRUIUtilsService.callDirectiveLoad(sharedSettingsGridAPI, payload, sharedSettingsGridLoadDeferred);
            });
            return sharedSettingsGridLoadDeferred.promise;
        }

        function loadMySettingsGrid() {
            var mySettingsGridLoadDeferred = UtilsService.createPromiseDeferred();
            mySettingsGridReadyDeferred.promise.then(function () {
                var payload = {
                    query: {
                        EntityUniqueNames: entityUniqueNames,
                        IsGlobal: false
                    },
                    selectedName: personilizationItemName,
                    selectedIsGlobal: selectedIsGlobal
                };
                VRUIUtilsService.callDirectiveLoad(mySettingsGridAPI, payload, mySettingsGridLoadDeferred);
            });
            return mySettingsGridLoadDeferred.promise;
        }


    }

    appControllers.controller("VRCommon_NamedPersonalizationItemsEditorController", namedPersonalizationItemsEditorController);
})(appControllers);
