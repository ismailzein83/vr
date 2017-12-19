﻿(function (app) {

    "use strict";

    VRExclusiveSessionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRCommon_VRExclusiveSessionService', 'VRCommon_VRExclusiveSessionAPIService'];

    function VRExclusiveSessionManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRCommon_VRExclusiveSessionService, VRCommon_VRExclusiveSessionAPIService) {

        var exclusiveSessionTypeAPI;
        var exclusiveSessionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;


        defineScope();
        load();


        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.search = function () {
                return loadGrid(buildGridQuery());
            };

            $scope.scopeModel.onExclusiveSessionTypeSelectorReady = function (api) {
                exclusiveSessionTypeAPI = api;
                exclusiveSessionTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.releaseAll = function () {
                var onVRExclusiveSessionForceRelease = function () {
                    return gridAPI.load(buildGridQuery());
                }
                return VRCommon_VRExclusiveSessionService.forceReleaseAll(onVRExclusiveSessionForceRelease);
            };

            $scope.scopeModel.hasForceReleaseAllSessionsPermission = function () {
                return VRCommon_VRExclusiveSessionAPIService.HasForceReleaseAllSessionsPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                return gridAPI.load(buildGridQuery());
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadExclusiveSessionType]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadGrid() {
            if (gridAPI != undefined) {
                var query = buildGridQuery();
                $scope.scopeModel.isLoading = true;
                return gridAPI.load(query).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }
        function loadExclusiveSessionType() {
            var loadExclusiveSessionTypePromiseDeferred = UtilsService.createPromiseDeferred();
            exclusiveSessionTypeSelectorReadyDeferred.promise.then(function () {
                var payloadDirective;
                VRUIUtilsService.callDirectiveLoad(exclusiveSessionTypeAPI, payloadDirective, loadExclusiveSessionTypePromiseDeferred);
            });
            return loadExclusiveSessionTypePromiseDeferred.promise;
        }

        function buildGridQuery() {
            return {
                TargetName: $scope.scopeModel.targetName,
                SessionTypeIds: exclusiveSessionTypeAPI.getSelectedIds()
            };
        }
    }

    app.controller('VRCommon_VRExclusiveSessionManagementController', VRExclusiveSessionManagementController);

})(app);