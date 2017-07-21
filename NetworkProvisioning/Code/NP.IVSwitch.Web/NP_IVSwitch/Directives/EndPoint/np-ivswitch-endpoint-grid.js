'use strict';

app.directive('npIvswitchEndpointGrid', ['NP_IVSwitch_EndPointAPIService', 'NP_IVSwitch_EndPointService', 'VRNotificationService', 'NP_IVSwitch_EndPointEnum', 'UtilsService', 'VRUIUtilsService',
    function (NP_IVSwitch_EndPointAPIService, NP_IVSwitch_EndPointService, VRNotificationService, NP_IVSwitch_EndPointEnum, UtilsService, VRUIUtilsService) {
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
            var gridDrillDownTabsObj;
            var carrierAccountId;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.endPoint = [];
                $scope.scopeModel.menuActions = [];
                $scope.scopeModel.Type = {};
 
                $scope.scopeModel.addEndPoint = function () {;
                    var onEndPointAdded = function (addedEndPoint) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(addedEndPoint);
                        gridAPI.itemAdded(addedEndPoint);
                    };
                    NP_IVSwitch_EndPointService.addEndPoint(carrierAccountId, onEndPointAdded);
                };
                $scope.scopeModel.hadAddEndPointPermission = function () {
                    return NP_IVSwitch_EndPointAPIService.HasAddEndPointPermission();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = NP_IVSwitch_EndPointService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_EndPointAPIService.GetFilteredEndPoints(dataRetrievalInput).then(function (response) {

                        var EnumArray = UtilsService.getArrayEnum(NP_IVSwitch_EndPointEnum);
                        if (response !=undefined && response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                         //for (var i = 0; i < response.Data.length; i++) {
                         //    if (response.Data[i].Entity.EndPointType == NP_IVSwitch_EndPointEnum.ACL.value) {
                         //        response.Data[i].Entity.Type = NP_IVSwitch_EndPointEnum.ACL.description;
                         //   }
                         //    else {
                         //        response.Data[i].Entity.Type = NP_IVSwitch_EndPointEnum.SIP.description; 
                         //    }
                         //}
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
                    carrierAccountId = query != undefined && query.CarrierAccountId != undefined ? query.CarrierAccountId : undefined;
                    return gridAPI.retrieveData(query);
                };

                api.onEndPointAdded = function (addedEndPoint) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedEndPoint);
                    gridAPI.itemAdded(addedEndPoint);
                };

              

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
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedEndPoint);
                    gridAPI.itemUpdated(updatedEndPoint);
                };
                NP_IVSwitch_EndPointService.editEndPoint(EndPointItem.Entity.EndPointId, onEndPointUpdated);
            }
            function hasEditEndPointPermission() {
                return NP_IVSwitch_EndPointAPIService.HasEditEndPointPermission();
            }

        }
    }]);

