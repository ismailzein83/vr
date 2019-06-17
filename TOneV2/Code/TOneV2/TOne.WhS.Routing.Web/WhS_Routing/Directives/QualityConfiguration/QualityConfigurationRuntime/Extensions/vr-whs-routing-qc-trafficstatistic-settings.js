(function (app) {

    'use strict';

    TrafficStatisticQualityConfigurationSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_TrafficStatisticQCDefinitionAPIService'];

    function TrafficStatisticQualityConfigurationSettingsDirective(UtilsService, VRUIUtilsService, WhS_Routing_TrafficStatisticQCDefinitionAPIService) {
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

            var trafficStatisticQCDefinitionData;

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
                    var promises = [];

                    var qualityConfigurationSettings;
                    var qualityConfigurationDefinitionId;

                    if (payload != undefined) {
                        qualityConfigurationSettings = payload.qualityConfigurationSettings;
                        qualityConfigurationDefinitionId = payload.qualityConfigurationDefinitionId;

                        $scope.scopeModel.expression = qualityConfigurationSettings != undefined ? qualityConfigurationSettings.Expression : undefined;
                    }

                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadPromiseDeferred.promise);

                    getTrafficStatisticQCDefinitionData().then(function () {
                        if (trafficStatisticQCDefinitionData != undefined) {
                            if (trafficStatisticQCDefinitionData.AnalyticMeasureInfos != undefined) {
                                for (var i = 0, length = trafficStatisticQCDefinitionData.AnalyticMeasureInfos.length; i < length; i++) {
                                    var analyticMeasureInfo = trafficStatisticQCDefinitionData.AnalyticMeasureInfos[i];
                                    $scope.scopeModel.qualityConfigurationFields.push({
                                        Name: analyticMeasureInfo.Name,
                                        Title: analyticMeasureInfo.Title,
                                        Expression: 'context.GetMeasureValue("' + analyticMeasureInfo.Name + '")'
                                    });
                                }
                            }

                            loadTimePeriodDirective().then(function () {
                                loadPromiseDeferred.resolve();
                            });
                        }
                    });


                    function getTrafficStatisticQCDefinitionData() {
                        return WhS_Routing_TrafficStatisticQCDefinitionAPIService.GetTrafficStatisticQCDefinitionData(qualityConfigurationDefinitionId).then(function (response) {
                            trafficStatisticQCDefinitionData = response;
                        });
                    }
                    function loadTimePeriodDirective() {
                        var timeSettingsDirective = trafficStatisticQCDefinitionData.TimeSettingsDirective;
                        $scope.scopeModel.timeSettingsDirective = timeSettingsDirective != undefined ? timeSettingsDirective : "vr-common-timeperiod";

                        var loadTimePeriodPromiseDeferred = UtilsService.createPromiseDeferred();

                        timePeriodReadyPromiseDeferred.promise.then(function () {

                            var directivePayload;
                            if (qualityConfigurationSettings != undefined && qualityConfigurationSettings.TimePeriod != undefined) {
                                directivePayload = { timePeriod: qualityConfigurationSettings.TimePeriod };
                            }
                            VRUIUtilsService.callDirectiveLoad(timePeriodAPI, directivePayload, loadTimePeriodPromiseDeferred);
                        });

                        return loadTimePeriodPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
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