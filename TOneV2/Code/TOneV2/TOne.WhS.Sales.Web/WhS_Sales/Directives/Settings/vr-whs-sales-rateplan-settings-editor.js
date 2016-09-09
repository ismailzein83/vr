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
                    $scope.scopeModel.increasedRateDayOffset = payload.data.IncreasedRateDayOffset;
                    $scope.scopeModel.decreasedRateDayOffset = payload.data.DecreasedRateDayOffset;
                    $scope.scopeModel.newServiceDayOffset = payload.data.NewServiceDayOffset;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.Entities.RatePlanSettingsData, TOne.WhS.Sales.Entities",
                    IncreasedRateDayOffset: $scope.scopeModel.increasedRateDayOffset,
                    DecreasedRateDayOffset: $scope.scopeModel.decreasedRateDayOffset,
                    NewServiceDayOffset: $scope.scopeModel.newServiceDayOffset
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);