'use strict';

app.directive('retailBeRevenuecomparisonGrid', ['Retail_BE_RevenueComparisonAPIService', 'VRNotificationService', function (Retail_BE_RevenueComparisonAPIService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var revenueComparisonGrid = new RevenueComparisonGrid($scope, ctrl, $attrs);
            revenueComparisonGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RevenueComparison/Templates/RevenueComparisonGridTemplate.html'
    };

    function RevenueComparisonGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.revenueComparisons = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_RevenueComparisonAPIService.GetFilteredRevenueComparisons(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
