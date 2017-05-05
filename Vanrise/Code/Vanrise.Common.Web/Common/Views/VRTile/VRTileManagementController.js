"use strict";
VRTileManagementController.$inject = ['$scope', 'VRUIUtilsService', 'VRNavigationService','VR_Sec_ViewAPIService','UtilsService','VRNotificationService'];

function VRTileManagementController($scope, VRUIUtilsService, VRNavigationService, VR_Sec_ViewAPIService, UtilsService, VRNotificationService) {
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
        }).catch(function () {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.scopeModel.isLoading = false;
        });
    }

    function loadAllControls() {

        function loadVRTiles() {
            var promises = [];
            if (viewEntity != undefined && viewEntity.Settings != undefined && viewEntity.Settings.VRTileViewData != undefined)
            {
                var tiles = viewEntity.Settings.VRTileViewData.VRTiles;
                if(tiles != undefined)
                {
                    for (var i = 0, tilesLength = tiles.length; i < tilesLength; i++) {
                        var tileEntity = tiles[i];
                        addTile(tileEntity);
                    }
                }
            }
            function addTile(tileEntity)
            {
                var tile = {
                    name: tileEntity.Name,
                    runtimeEditor: tileEntity.Settings.ExtendedSettings.RuntimeEditor,
                };
                tile.onVRTileDirectiveReady = function (api) {
                    tile.tileAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
                    };
                    var payload = { definitionSettings: tileEntity.Settings.ExtendedSettings };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, tile.tileAPI, payload, setLoader);
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

    function getView() {
        return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
            viewEntity = viewEntityObj;
        });
    }
}
appControllers.controller('VRCommon_VRTileManagementController', VRTileManagementController);