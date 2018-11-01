'use strict';

app.directive('cpWhsanalyticsBlockedattemptsGrid', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'CP_WhSAnalytics_BlockedAttemptsAPIService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, CP_WhSAnalytics_BlockedAttemptsAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new BlockedAttemptsGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CP_WhSAnalytics/Elements/BlockedAttempts/Directives/Templates/BlockedAttemptsGridTemplate.html'
        };

        function BlockedAttemptsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.blockedAttempts = [];
                $scope.scopeModel.gridMenuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    console.log(dataRetrievalInput);
                    return CP_WhSAnalytics_BlockedAttemptsAPIService.GetFilteredBlockedAttempts(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
