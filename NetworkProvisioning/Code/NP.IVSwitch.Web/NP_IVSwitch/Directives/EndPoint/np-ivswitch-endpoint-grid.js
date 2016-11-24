'use strict';

app.directive('npIvswitchEndpointGrid', ['NP_IVSwitch_EndPointAPIService', 'NP_IVSwitch_EndPointService', 'VRNotificationService', 'NP_IVSwitch_EndPointEnum', 'UtilsService',
    function (NP_IVSwitch_EndPointAPIService, NP_IVSwitch_EndPointService, VRNotificationService, NP_IVSwitch_EndPointEnum, UtilsService) {
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
                $scope.scopeModel.Type = {};
 



                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_EndPointAPIService.GetFilteredEndPoints(dataRetrievalInput).then(function (response) {

                        var EnumArray = UtilsService.getArrayEnum(NP_IVSwitch_EndPointEnum);

                         for (var i = 0; i < response.Data.length; i++) {
                             if (response.Data[i].Entity.EndPointType == NP_IVSwitch_EndPointEnum.ACL.value) {
                                 response.Data[i].Entity.Type = NP_IVSwitch_EndPointEnum.ACL.description;
                            }
                             else {
                                 response.Data[i].Entity.Type = NP_IVSwitch_EndPointEnum.SIP.description; 
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

