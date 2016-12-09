'use strict';

app.directive('vrWhsSalesRateplanSettingsEditor', [function () {

    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ratePlanSettings = new RatePlanSettings(ctrl, $scope);
            ratePlanSettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Settings/Templates/RatePlanSettingsEditorTemplate.html'
    };

    function RatePlanSettings(ctrl, $scope) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.data != null) {
                	$scope.scopeModel.newRateDayOffset = payload.data.NewRateDayOffset;
                	$scope.scopeModel.increasedRateDayOffset = payload.data.IncreasedRateDayOffset;
                    $scope.scopeModel.decreasedRateDayOffset = payload.data.DecreasedRateDayOffset;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.Entities.RatePlanSettingsData, TOne.WhS.Sales.Entities",
                    NewRateDayOffset: $scope.scopeModel.newRateDayOffset,
                    IncreasedRateDayOffset: $scope.scopeModel.increasedRateDayOffset,
                    DecreasedRateDayOffset: $scope.scopeModel.decreasedRateDayOffset
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);