'use strict';

app.directive('vrCommonComponenttypeGrid', ['VRCommon_VRComponentTypeAPIService', 'VRCommon_VRComponentTypeService', 'VRNotificationService',
    function (VRCommon_VRComponentTypeAPIService, VRCommon_VRComponentTypeService, VRNotificationService) {
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

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.vrComponentTypes = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRComponentTypeAPIService.GetFilteredVRComponentTypes(dataRetrievalInput).then(function (response) {
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
                    gridAPI.itemUpdated(updatedVRComponentType);
                };

                VRCommon_VRComponentTypeService.editVRComponentType(vrComponentTypeItem.Entity.Settings.VRComponentTypeConfigId, vrComponentTypeItem.Entity.VRComponentTypeId, onVRComponentTypeUpdated);
            }

            function hasEditVRComponentTypePermission() {
                return VRCommon_VRComponentTypeAPIService.HasEditVRComponentTypePermission();
            }
        }
    }]);
