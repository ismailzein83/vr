'use strict';
app.directive('vrWhsRoutingRoutingoptimizersettingsQuality', ['UtilsService','WhS_Routing_TimeSettingsTypeEnum',
    function (UtilsService, WhS_Routing_TimeSettingsTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new QualityRoutingOptimizerItemSettings(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Settings/Extensions/Templates/QualityRoutingOptimizerItemSettingsTemplate.html';
            }

        };

        function QualityRoutingOptimizerItemSettings(ctrl, $scope) {
            function initializeController() {
                $scope.scopeModel = {};

                $scope.timeUnits = UtilsService.getArrayEnum(WhS_Routing_TimeSettingsTypeEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.periodLength = payload.PeriodLength;
                        $scope.scopeModel.qualityFormula = payload.QualityFormula;
                        $scope.scopeModel.selectedTimeUnit = UtilsService.getEnum(WhS_Routing_TimeSettingsTypeEnum, "value", payload.PeriodTimeUnit);
                    }

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Entities.QualityRoutingOptimizerItemSettings, TOne.WhS.Routing.Entities",
                        PeriodLength:$scope.scopeModel.periodLength,
                        PeriodTimeUnit: $scope.scopeModel.selectedTimeUnit.value,
                        QualityFormula: $scope.scopeModel.qualityFormula,
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);