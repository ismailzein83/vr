'use strict';

app.directive('cpWhsBlockedattemptsGrid', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'CP_WhS_BlockedAttemptsAPIService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, CP_WhS_BlockedAttemptsAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                shownumber: "=",
                showcustomer:"="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new BlockedAttemptsGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CP_WhS/Elements/BlockedAttempts/Directives/Templates/BlockedAttemptsGridTemplate.html'
        };

        function BlockedAttemptsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.blockedAttempts = [];
                $scope.scopeModel.gridMenuActions = [];

                $scope.scopeModel.showCustomer = true;

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CP_WhS_BlockedAttemptsAPIService.GetFilteredBlockedAttempts(dataRetrievalInput).then(function (response) {
                        $scope.scopeModel.showNumber = ctrl.shownumber;
                        $scope.scopeModel.showCustomer = ctrl.showcustomer;
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
