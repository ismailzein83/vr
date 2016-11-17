'use strict';

app.directive('npIvswitchEndpointGrid', ['NP_IVSwitch_EndPointAPIService', 'NP_IVSwitch_EndPointService', 'VRNotificationService',
    function (NP_IVSwitch_EndPointAPIService, NP_IVSwitch_EndPointService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var endPointGrid = new EndPointGrid($scope, ctrl, $attrs);
                endPointGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/EndPoint/Templates/EndPointGridTemplate.html'
        };

        function EndPointGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.endPoint = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_EndPointAPIService.GetFilteredEndPoints(dataRetrievalInput).then(function (response) {
                        console.log(response)
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

                api.onEndPointAdded = function (addedEndPoint) {
                    gridAPI.itemAdded(addedEndPoint);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editEndPoint,
                    haspermission: hasEditEndPointPermission
                });
            }
            function editEndPoint(EndPointItem) {
                var onEndPointUpdated = function (updatedEndPoint) {
                    gridAPI.itemUpdated(updatedEndPoint);
                };
                NP_IVSwitch_EndPointService.editEndPoint(EndPointItem.Entity.EndPointId, onEndPointUpdated);
            }
            function hasEditEndPointPermission() {
                return NP_IVSwitch_EndPointAPIService.HasEditEndPointPermission();
            }

        }
    }]);

