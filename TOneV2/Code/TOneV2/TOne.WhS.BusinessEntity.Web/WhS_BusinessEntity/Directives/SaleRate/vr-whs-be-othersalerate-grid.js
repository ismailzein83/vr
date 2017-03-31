'use strict';

app.directive('vrWhsBeOthersalerateGrid', ['WhS_BE_OtherSaleRateAPIService', 'UtilsService', 'VRNotificationService', function (WhS_BE_OtherSaleRateAPIService, UtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var otherSaleRateGrid = new OtherSaleRateGrid($scope, ctrl, $attrs);
            otherSaleRateGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SaleRate/Templates/OtherSaleRateGridTemplate.html'
    };

    function OtherSaleRateGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.otherSaleRates = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var query;

                if (payload != undefined) {
                    query = payload.query;
                }

                return WhS_BE_OtherSaleRateAPIService.GetOtherSaleRates(query).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++)
                            $scope.scopeModel.otherSaleRates.push(response[i]);
                    }
                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);