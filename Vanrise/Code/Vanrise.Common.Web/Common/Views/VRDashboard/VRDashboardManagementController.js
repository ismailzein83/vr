"use strict";
VRDashboardManagementController.$inject = ['$scope', 'VRUIUtilsService', 'VRNavigationService', 'VR_Sec_ViewAPIService', 'UtilsService', 'VRNotificationService', 'ColumnWidthEnum', 'VRCommon_VRDashboardAPIService'];

function VRDashboardManagementController($scope, VRUIUtilsService, VRNavigationService, VR_Sec_ViewAPIService, UtilsService, VRNotificationService, ColumnWidthEnum, VRCommon_VRDashboardAPIService) {

    var viewId;
    var viewEntity;
    var dashboardDefinitionIds;

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
        $scope.scopeModel.vrDashboardsDefinition = [];

        $scope.scopeModel.loadVRDashboardEntity = function (vrDashboardDefinitionId) {
            getDashboardEntity(vrDashboardDefinitionId);
        };
    }

    function load() {
        $scope.scopeModel.isLoading = true;
        getView().then(function () {
            loadAllControls();
        }).catch(function () {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });
    }

    function loadAllControls() {

        function loadDashboardInfoList() {
            var filter = {
                DashboardDefinitionIds: dashboardDefinitionIds
            };
            var serializedFilter = UtilsService.serializetoJson(filter);
            return VRCommon_VRDashboardAPIService.GetDashboardDefinitionInfo(serializedFilter).then(function (response) {
                if (response != null) {
                    var dashboardsDefinition = response;
                    for (var i = 0; i < dashboardsDefinition.length; i++) {
                        var dashboardDefinition = dashboardsDefinition[i];
                        $scope.scopeModel.vrDashboardsDefinition.push(dashboardDefinition);
                    }
                    getDashboardEntity(dashboardsDefinition[0].VRDashboardDefinitionId);
                }
            });
        }

        return UtilsService.waitMultipleAsyncOperations([loadDashboardInfoList]).then(function () {
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function getDashboardEntity(vrDashboardDefinitionId) {
        $scope.scopeModel.isLoading = true;
        return VRCommon_VRDashboardAPIService.GetDashboardDefinitionEntity(vrDashboardDefinitionId).then(function (response) {
            if (response != null && response.Settings.VRTiles != null && response.Settings.VRTiles.length > 0) {
                var tiles = response.Settings.VRTiles;
                $scope.scopeModel.vrTiles = [];
                for (var i = 0; i < tiles.length; i++) {
                    var tile = tiles[i];
                    addTile(tile);
                }
            }
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        })
    }

    function addTile(tileEntity) {
        var columnWidthObj = UtilsService.getEnum(ColumnWidthEnum, "value", tileEntity.Settings.NumberOfColumns);
        var tile = {
            name: tileEntity.Name,
            runtimeEditor: tileEntity.Settings.ExtendedSettings != undefined ? tileEntity.Settings.ExtendedSettings.RuntimeEditor : undefined,
            columnWidth: columnWidthObj != undefined ? columnWidthObj.numberOfColumns : undefined
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

    function getView() {
        dashboardDefinitionIds = [];
        return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
            viewEntity = viewEntityObj;
            for (var i = 0; i < viewEntity.Settings.DashboardDefinitionItems.length; i++) {
                var dashboardDefinitionItem = viewEntity.Settings.DashboardDefinitionItems[i];
                dashboardDefinitionIds.push(dashboardDefinitionItem.DashboardDefinitionId);
            }
        });
    }
}
appControllers.controller('VRCommon_VRDashboardManagementController', VRDashboardManagementController);




