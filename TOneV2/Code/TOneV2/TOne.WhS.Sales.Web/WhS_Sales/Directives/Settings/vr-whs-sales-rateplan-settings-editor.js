'use strict';

app.directive('vrWhsSalesRateplanSettingsEditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

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
        var costColumnsDirectiveAPI;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.scopeModel = {};

            $scope.scopeModel.onCostColumnsGridReady = function (api) {
                costColumnsDirectiveAPI = api;
                defineAPI();
            }

          
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var costCalculationsMethodsPayload = {};

                if (payload != undefined && payload.data != null) {
                	$scope.scopeModel.newRateDayOffset = payload.data.NewRateDayOffset;
                	$scope.scopeModel.increasedRateDayOffset = payload.data.IncreasedRateDayOffset;
                	$scope.scopeModel.decreasedRateDayOffset = payload.data.DecreasedRateDayOffset;
                	costCalculationsMethodsPayload.costCalculationMethods = payload.data.CostCalculationsMethods;
                }

                var setLoader = function (value) { $scope.scopeModel.isLoadingCostCalculationsMethods = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, costColumnsDirectiveAPI, costCalculationsMethodsPayload, setLoader);
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.Entities.RatePlanSettingsData, TOne.WhS.Sales.Entities",
                    NewRateDayOffset: $scope.scopeModel.newRateDayOffset,
                    IncreasedRateDayOffset: $scope.scopeModel.increasedRateDayOffset,
                    DecreasedRateDayOffset: $scope.scopeModel.decreasedRateDayOffset,
                    CostCalculationsMethods: costColumnsDirectiveAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);