'use strict';
app.directive('vrWhsRoutingRouterulesettingsFilterRate', ['UtilsService', 'WhS_Routing_RateOptionTypeEnum', 'WhS_Routing_RateOptionEnum',
function (UtilsService, WhS_Routing_RateOptionTypeEnum, WhS_Routing_RateOptionEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            var ctor = new rateCtor(ctrl, $scope);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Filter/Templates/FilterByRateDirective.html';
        }

    };

    function rateCtor(ctrl, $scope) {
        $scope.scopeModel = {};
        $scope.rateOptions = [];
        $scope.rateOptionTypes = [];


        $scope.validateRateOptionValue = function () {
            if ($scope.scopeModel.rateOptionType == undefined || $scope.scopeModel.rateOptionType.maxValue == undefined || $scope.scopeModel.rateOptionValue == undefined)
                return null;

            if ($scope.scopeModel.rateOptionValue > $scope.scopeModel.rateOptionType.maxValue)
                return $scope.scopeModel.rateOptionType.description + ' must be less than or equal to ' + $scope.scopeModel.rateOptionType.maxValue;

            return null;

        };
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                loadRateOptions();
                loadRateOptionTypes();
                if (payload != undefined) {
                    $scope.scopeModel.rateOption = UtilsService.getEnum(WhS_Routing_RateOptionEnum, 'value', payload.RateOption);
                    $scope.scopeModel.rateOptionType = UtilsService.getEnum(WhS_Routing_RateOptionTypeEnum, 'value', payload.RateOptionType);
                    $scope.scopeModel.rateOptionValue = payload.RateOptionValue;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Routing.Business.RouteRules.Filters.RateOptionFilter, TOne.WhS.Routing.Business",
                    RateOption: $scope.scopeModel.rateOption.value,
                    RateOptionType: $scope.scopeModel.rateOptionType.value,
                    RateOptionValue: $scope.scopeModel.rateOptionValue
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadRateOptions() {
            $scope.rateOptions = UtilsService.getArrayEnum(WhS_Routing_RateOptionEnum);
        }

        function loadRateOptionTypes() {
            $scope.rateOptionTypes = UtilsService.getArrayEnum(WhS_Routing_RateOptionTypeEnum);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);