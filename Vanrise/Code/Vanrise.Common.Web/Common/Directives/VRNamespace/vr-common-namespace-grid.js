'use strict';

app.directive('vrCommonNamespaceGrid', ['VRUIUtilsService', 'VRNotificationService', 'VRCommon_VRNamespaceAPIService', 'VRCommon_VRNamespaceService',
    function (VRUIUtilsService, VRNotificationService, VRCommon_VRNamespaceAPIService, VRCommon_VRNamespaceService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRNamespaceGridDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRNamespace/Templates/VRNamespaceGridTemplate.html'
        };

        function VRNamespaceGridDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrNamespaces = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VRCommon_VRNamespaceService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.scopeModel.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRNamespaceAPIService.GetFilteredVRNamespaces(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                $scope.scopeModel.vrNamespaces.push(response.Data[i]);
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

                api.onVRNamespaceAdded = function (addedVRNamespace) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVRNamespace);
                    gridAPI.itemAdded(addedVRNamespace);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRNamespace,
                    haspermission: hasEditVRNamespacePermission
                });
            }
            function editVRNamespace(vrNamespaceObj) {
                var onVRNamespaceUpdated = function (updatedVRNamespace) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRNamespace);
                    gridAPI.itemUpdated(updatedVRNamespace);
                };

                VRCommon_VRNamespaceService.editVRNamespace(onVRNamespaceUpdated, vrNamespaceObj.VRNamespaceId);
            }
            function hasEditVRNamespacePermission() {
                return VRCommon_VRNamespaceAPIService.HasEditVRNamespacePermission();
            }
        }
    }]);
