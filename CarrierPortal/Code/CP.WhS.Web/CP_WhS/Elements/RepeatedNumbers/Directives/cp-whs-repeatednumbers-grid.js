'use strict';

app.directive('cpWhsRepeatednumbersGrid', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'CP_WhS_RepeatedNumbersAPIService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, CP_WhS_RepeatedNumbersAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new RepeatedNumbersGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CP_WhS/Elements/RepeatedNumbers/Directives/Templates/RepeatedNumbersGridTemplate.html'
        };

        function RepeatedNumbersGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.repeatedNumbers = [];
                $scope.scopeModel.gridMenuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CP_WhS_RepeatedNumbersAPIService.GetFilteredRepeatedNumbers(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    $scope.showCustomer = query.Filter.CustomerIds != undefined;
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
