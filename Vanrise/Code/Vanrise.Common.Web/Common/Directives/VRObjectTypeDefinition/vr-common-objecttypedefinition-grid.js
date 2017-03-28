'use strict';

app.directive('vrCommonObjecttypedefinitionGrid', ['VRCommon_VRObjectTypeDefinitionAPIService', 'VRCommon_VRObjectTypeDefinitionService', 'VRNotificationService', 'VRUIUtilsService',
    function (VRCommon_VRObjectTypeDefinitionAPIService, VRCommon_VRObjectTypeDefinitionService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrObjectTypeDefinitionGrid = new VRObjectTypeDefinitionGrid($scope, ctrl, $attrs);
                vrObjectTypeDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRObjectTypeDefinition/Templates/VRObjectTypeDefinitionGridTemplate.html'
        };

        function VRObjectTypeDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrObjectTypeDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VRCommon_VRObjectTypeDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRObjectTypeDefinitionAPIService.GetFilteredVRObjectTypeDefinitions(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onVRObjectTypeDefinitionAdded = function (addedVRObjectTypeDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVRObjectTypeDefinition);
                    gridAPI.itemAdded(addedVRObjectTypeDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRObjectTypeDefinition,
                    haspermission: hasEditVRObjectTypeDefinitionPermission
                });
            }
            function editVRObjectTypeDefinition(vrObjectTypeDefinitionItem) {
                var onVRObjectTypeDefinitionUpdated = function (updatedVRObjectTypeDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRObjectTypeDefinition);
                    gridAPI.itemUpdated(updatedVRObjectTypeDefinition);
                };

                VRCommon_VRObjectTypeDefinitionService.editVRObjectTypeDefinition(vrObjectTypeDefinitionItem.Entity.VRObjectTypeDefinitionId, onVRObjectTypeDefinitionUpdated);
            }
            function hasEditVRObjectTypeDefinitionPermission() {
                return VRCommon_VRObjectTypeDefinitionAPIService.HasEditVRObjectTypeDefinitionPermission();
            }
        }
}]);
