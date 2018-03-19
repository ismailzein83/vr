(function (app) {

    'use strict';

    TrafficStatisticQualityConfigurationSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_TrafficStatisticQualityConfigurationAPIService'];

    function TrafficStatisticQualityConfigurationSettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_TrafficStatisticQualityConfigurationAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TrafficStatisticQualityConfigurationSettingsCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Routing/Directives/QualityConfiguration/QualityConfigurationRuntime/Extensions/Templates/TrafficStatisticQualityConfigurationSettingsTemplate.html'
        };

        function TrafficStatisticQualityConfigurationSettingsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var timePeriodAPI;
            var timePeriodReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.qualityConfigurationFields = [];
                $scope.scopeModel.qualityConfigurationSignsFields = buildQualityConfigurationSignsFields();

                $scope.scopeModel.onTimeperiodReady = function (api) {
                    timePeriodAPI = api;
                    timePeriodReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.qualityConfigurationFieldClicked = function (measure) {
                    if ($scope.scopeModel.expression == undefined)
                        $scope.scopeModel.expression = measure.Expression;
                    else
                        $scope.scopeModel.expression += " " + measure.Expression;
                };

                $scope.scopeModel.validateExpression = function () {
                    if ($scope.scopeModel.expression == undefined)
                        return null;

                    if ($scope.scopeModel.expression.indexOf("context.GetMeasureValue(") == -1)
                        return "Expression should contain at least one Measure!!";

                    return null;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promise = [];

                    var qualityConfigurationSettings;
                    var qualityConfigurationDefinitionId;

                    if (payload != undefined) {
                        qualityConfigurationSettings = payload.qualityConfigurationSettings;
                        qualityConfigurationDefinitionId = payload.qualityConfigurationDefinitionId;

                        $scope.scopeModel.expression = qualityConfigurationSettings != undefined ?  qualityConfigurationSettings.Expression : undefined;
                    }

                    var loadTimeperiodDirectivePromise = loadTimeperiodDirective();
                    promise.push(loadTimeperiodDirectivePromise);

                    if (qualityConfigurationDefinitionId != undefined) {
                        var loadTrafficStatisticQualityConfigurationMeasuresPromise = loadTrafficStatisticQualityConfigurationMeasures();
                        promise.push(loadTrafficStatisticQualityConfigurationMeasuresPromise);
                    }


                    function loadTimeperiodDirective() {
                        var loadTimeperiodPromiseDeferred = UtilsService.createPromiseDeferred();

                        timePeriodReadyPromiseDeferred.promise.then(function () {
                            var directivePayload;
                            if (qualityConfigurationSettings != undefined && qualityConfigurationSettings.TimePeriod != undefined) {
                                directivePayload = { timePeriod: qualityConfigurationSettings.TimePeriod };
                            }
                            VRUIUtilsService.callDirectiveLoad(timePeriodAPI, directivePayload, loadTimeperiodPromiseDeferred);
                        });

                        return loadTimeperiodPromiseDeferred.promise;
                    }

                    function loadTrafficStatisticQualityConfigurationMeasures() {
                        return WhS_Routing_TrafficStatisticQualityConfigurationAPIService.GetTrafficStatisticQualityConfigurationMeasures(qualityConfigurationDefinitionId).then(function (response) {
                            if (response != undefined) {
                                for (var i = 0, length = response.length ; i < length ; i++) {
                                    var responseItem = response[i];
                                    $scope.scopeModel.qualityConfigurationFields.push({
                                        Name: responseItem.Name,
                                        Title: responseItem.Title,
                                        Expression: 'context.GetMeasureValue("' + responseItem.Name + '")'
                                    });
                                }
                            }
                        });
                    }
                };

                api.getData = function () {

                    var obj = {
                        $type: "TOne.WhS.Routing.Business.TrafficStatisticQualityConfigurationSettings, TOne.WhS.Routing.Business",
                        TimePeriod: timePeriodAPI.getData(),
                        Expression: $scope.scopeModel.expression
                    };

                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildQualityConfigurationSignsFields() {
                return [{
                    Name: '(',
                    Title: '(',
                    Expression: '(',
                }, {
                    Name: ')',
                    Title: ')',
                    Expression: ')',
                }, {
                    Name: '+',
                    Title: '+',
                    Expression: '+',
                }, {
                    Name: '-',
                    Title: '-',
                    Expression: '-',
                }, {
                    Name: '*',
                    Title: '*',
                    Expression: '*',
                }, {
                    Name: '/',
                    Title: '/',
                    Expression: '/',
                }];
            }
        }
    }

    app.directive('vrWhsRoutingQcTrafficstatisticSettings', TrafficStatisticQualityConfigurationSettingsDirective);
})(app);