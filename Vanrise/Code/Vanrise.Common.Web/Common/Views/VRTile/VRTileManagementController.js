"use strict";
VRTileManagementController.$inject = ['$scope', 'VRUIUtilsService', 'VRNavigationService', 'VR_Sec_ViewAPIService', 'UtilsService', 'VRNotificationService', 'ColumnWidthEnum', 'VRTimerService'];

function VRTileManagementController($scope, VRUIUtilsService, VRNavigationService, VR_Sec_ViewAPIService, UtilsService, VRNotificationService, ColumnWidthEnum, VRTimerService) {
    var viewId;

    var viewEntity;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null && parameters.viewId != undefined) {
            viewId = parameters.viewId;
        }
    }

    function defineScope() {
        $scope.scopeModel = {};
        $scope.scopeModel.vrTiles = [];
    }

    function load() {
        $scope.scopeModel.isLoading = true;
        getView().then(function () {
            loadAllControls();

            if ($scope.jobIds)
                VRTimerService.unregisterJobByIds($scope.jobIds);

        }).catch(function () {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.scopeModel.isLoading = false;
        });
    }

    function loadAllControls() {

        function loadVRTiles() {
            var promises = [];
            if (viewEntity != undefined && viewEntity.Settings != undefined && viewEntity.Settings.VRTileViewData != undefined) {
                var tiles = viewEntity.Settings.VRTileViewData.VRTiles;
                if (tiles != undefined) {
                    for (var i = 0, tilesLength = tiles.length; i < tilesLength; i++) {
                        var tileEntity = tiles[i];
                        addTile(tileEntity);
                    }
                }
            }
            function addTile(tileEntity) {
                var columnWidthObj = UtilsService.getEnum(ColumnWidthEnum, "value", tileEntity.Settings.NumberOfColumns);
                var tile = {
                    name: tileEntity.Name,
                    runtimeEditor: tileEntity.Settings.ExtendedSettings.RuntimeEditor,
                    columnWidth: columnWidthObj != undefined ? columnWidthObj.numberOfColumns : undefined,
                    showTitle: tileEntity.ShowTitle,
                    autoRefresh: tileEntity.AutoRefresh,
                    autoRefreshInterval: tileEntity.AutoRefreshInterval
                };
                tile.onVRTileDirectiveReady = function (api) {
                    tile.tileAPI = api;
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    $scope.scopeModel.isDirectiveLoading = true;
                    var payload = { definitionSettings: tileEntity.Settings.ExtendedSettings };
                    VRUIUtilsService.callDirectiveLoad(tile.tileAPI, payload, directiveLoadDeferred);

                    directiveLoadDeferred.promise.then(function () {
                        $scope.scopeModel.isDirectiveLoading = false;
                        if (tile.autoRefresh) {
                            var autoRefreshDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                            tile.onTimerElapsed = function () {
                                var payload = { definitionSettings: tileEntity.Settings.ExtendedSettings };
                                VRUIUtilsService.callDirectiveLoad(tile.tileAPI, payload, autoRefreshDirectiveLoadDeferred);
                                return autoRefreshDirectiveLoadDeferred.promise;
                            }
                            registerAutoRefreshJob(tile);
                        }
                    });
                };
                $scope.scopeModel.vrTiles.push(tile);
            }
            return UtilsService.waitMultiplePromises(promises);
        }

        return UtilsService.waitMultipleAsyncOperations([loadVRTiles]).then(function () {
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

    }

    function registerAutoRefreshJob(tile) {
        VRTimerService.registerJob(tile.onTimerElapsed, $scope, tile.AutoRefreshInterval);
    }

    function getView() {
        return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
            viewEntity = viewEntityObj;
        });
    }
}
appControllers.controller('VRCommon_VRTileManagementController', VRTileManagementController);