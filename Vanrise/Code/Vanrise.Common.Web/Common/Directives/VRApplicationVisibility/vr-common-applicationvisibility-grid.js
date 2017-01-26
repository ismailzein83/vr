'use strict';

app.directive('vrCommonApplicationvisibilityGrid', ['VRNotificationService', 'VRCommon_VRApplicationVisibilityAPIService', 'VRCommon_VRApplicationVisibilityService', 
    function (VRNotificationService, VRCommon_VRApplicationVisibilityAPIService, VRCommon_VRApplicationVisibilityService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrApplicationVisibilityGrid = new VRApplicationVisibilityGrid($scope, ctrl, $attrs);
                vrApplicationVisibilityGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRApplicationVisibility/Templates/VRApplicationVisibilityGridTemplate.html'
        };

        function VRApplicationVisibilityGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrApplicationVisibilities = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRApplicationVisibilityAPIService.GetFilteredVRApplicationVisibilities(dataRetrievalInput).then(function (response) {
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

                api.onVRApplicationVisibilityAdded = function (addedVRApplicationVisibility) {
                    gridAPI.itemAdded(addedVRApplicationVisibility);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRApplicationVisibility
                    //haspermission: hasEditVRApplicationVisibilityPermission
                });
            }
            function editVRApplicationVisibility(vrApplicationVisibilityItem) {
                var onVRApplicationVisibilityUpdated = function (updatedVRApplicationVisibility) {
                    gridAPI.itemUpdated(updatedVRApplicationVisibility);
                };

                VRCommon_VRApplicationVisibilityService.editVRApplicationVisibility(vrApplicationVisibilityItem.Entity.VRApplicationVisibilityId, onVRApplicationVisibilityUpdated);
            }
            //function hasEditVRApplicationVisibilityPermission() {
            //    return VRCommon_VRApplicationVisibilityAPIService.HasEditVRApplicationVisibilityPermission();
            //}
        }
    }]);
