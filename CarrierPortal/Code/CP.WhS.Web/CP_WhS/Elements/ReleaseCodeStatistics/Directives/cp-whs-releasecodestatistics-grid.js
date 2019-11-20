'use strict';

app.directive('cpWhsReleasecodestatisticsGrid', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'CP_WhS_ReleaseCodeStatisticsAPIService', 'CP_WhS_AccountViewTypeEnum',
    function (UtilsService, VRUIUtilsService, VRNotificationService, CP_WhS_ReleaseCodeStatisticsAPIService, CP_WhS_AccountViewTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                dimension: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new ReleaseCodeStatisticsGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CP_WhS/Elements/ReleaseCodeStatistics/Directives/Templates/ReleaseCodeStatisticsGridTemplate.html'
        };

        function ReleaseCodeStatisticsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.showDimensionCol = function (d) {
                    return ctrl.dimension != undefined && ctrl.dimension.indexOf(d) > -1;
                };

                $scope.scopeModel = {};
                $scope.scopeModel.releaseCodeStatistics = [];
                $scope.scopeModel.gridMenuActions = [];
                $scope.scopeModel.includeSupplierCol = false;

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CP_WhS_ReleaseCodeStatisticsAPIService.GetFilteredReleaseCodeStatistics(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    if (query != undefined) {
                        if (query.Filter != undefined) {
                            $scope.scopeModel.includeSupplierCol = query.Filter.AccountType != CP_WhS_AccountViewTypeEnum.Customer.value;
                        }
                    }
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
