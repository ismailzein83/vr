'use strict';

app.directive('vrCommonConnectionGrid', ['VRCommon_VRConnectionAPIService', 'VRCommon_VRConnectionService', 'VRNotificationService',
    function (VRCommon_VRConnectionAPIService, VRCommon_VRConnectionService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrConnectionGrid = new VRConnectionGrid($scope, ctrl, $attrs);
                vrConnectionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRConnection/Templates/VRConnectionGridTemplate.html'
        };

        function VRConnectionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.vrConnections = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRConnectionAPIService.GetFilteredVRConnections(dataRetrievalInput).then(function (response) {
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

                api.onVRConnectionAdded = function (addedVRConnection) {
                    gridAPI.itemAdded(addedVRConnection);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRConnection,
                    haspermission: hasEditVRConnectionPermission
                });
            }

            function editVRConnection(vrConnectionItem) {
                var onVRConnectionUpdated = function (updatedVRConnection) {
                    gridAPI.itemUpdated(updatedVRConnection);
                };

                VRCommon_VRConnectionService.editVRConnection(vrConnectionItem.Entity.VRConnectionId, onVRConnectionUpdated);
            }

            function hasEditVRConnectionPermission() {
                return VRCommon_VRConnectionAPIService.HasEditVRConnectionPermission();
            }
        }
    }]);
