'use strict';

app.directive('vrCommonComponenttypeGrid', ['VRCommon_VRComponentTypeAPIService', 'VRCommon_VRComponentTypeService', 'VRNotificationService', 'VRUIUtilsService',
    function (VRCommon_VRComponentTypeAPIService, VRCommon_VRComponentTypeService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrComponentTypeGrid = new VRComponentTypeGrid($scope, ctrl, $attrs);
                vrComponentTypeGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRComponentType/Templates/VRComponentTypeGridTemplate.html'
        };

        function VRComponentTypeGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.vrComponentTypes = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VRCommon_VRComponentTypeService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRComponentTypeAPIService.GetFilteredVRComponentTypes(dataRetrievalInput).then(function (response) {
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

                api.onVRComponentTypeAdded = function (addedVRComponentType) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVRComponentType);
                    gridAPI.itemAdded(addedVRComponentType);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRComponentType,
                    haspermission: hasEditVRComponentTypePermission
                });
            }

            function editVRComponentType(vrComponentTypeItem) {
                var onVRComponentTypeUpdated = function (updatedVRComponentType) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRComponentType);
                    gridAPI.itemUpdated(updatedVRComponentType);
                };

                VRCommon_VRComponentTypeService.editVRComponentType(vrComponentTypeItem.Entity.Settings.VRComponentTypeConfigId, vrComponentTypeItem.Entity.VRComponentTypeId, onVRComponentTypeUpdated);
            }

            function hasEditVRComponentTypePermission() {
                return VRCommon_VRComponentTypeAPIService.HasEditVRComponentTypePermission();
            }
        }
    }]);
