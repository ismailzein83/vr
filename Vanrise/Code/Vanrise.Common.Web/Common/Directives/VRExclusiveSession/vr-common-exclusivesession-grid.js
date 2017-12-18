'use strict';

app.directive('vrCommonExclusivesessionGrid', ['VRCommon_VRExclusiveSessionAPIService', 'VRNotificationService', 'VRUIUtilsService',
    function (VRCommon_VRExclusiveSessionAPIService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new VRExclusiveSessionGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRExclusiveSession/Templates/VRExclusiveSessionGridTemplate.html'
        };

        function VRExclusiveSessionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.vrExclusiveSessions = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;                   
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRExclusiveSessionAPIService.GetFilteredVRExclusiveSessions(dataRetrievalInput).then(function (response) {
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

               

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Release Session',
                    clicked: releaseSession//,
                    //haspermission: hasEditVRConnectionPermission
                });
            }

            function releaseSession(vrExclusiveSessionItem) {
                
            }

            function hasEditVRConnectionPermission() {
                return VRCommon_VRConnectionAPIService.HasEditVRConnectionPermission();
            }
        }
    }]);
