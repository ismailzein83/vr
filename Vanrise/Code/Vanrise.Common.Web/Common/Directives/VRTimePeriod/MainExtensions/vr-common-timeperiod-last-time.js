"use strict";

app.directive("vrCommonTimeperiodLastTime", ['UtilsService', 'VRUIUtilsService', 'VRCommon_TimeunitEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_TimeunitEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                isrequired: '=',
                hideremoveicon: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimePeriodLastTime($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: function (element, attrs) {
                return getDirectiveTemplateUrl();
            }
        };

        function getDirectiveTemplateUrl() {
            return "/Client/Modules/Common/Directives/VRTimePeriod/MainExtensions/Templates/LastTimeDirectiveTemplate.html";
        }

        function TimePeriodLastTime($scope, ctrl) {
            this.initializeController = initializeController;
            var timeUnitAPI;
            var timeUnitReadyPromiseDeferred = UtilsService.createPromiseDeferred();



            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTimeUnitReady = function (api) {
                    timeUnitAPI = api;
                    timeUnitReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                var promises = [];
                var timeUnit;
                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.timeValue = payload.timePeriod.TimeValue;
                        timeUnit = payload.timePeriod.TimeUnit;

                    }

                    var timeUnitLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    timeUnitReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(timeUnitAPI, { timeUnit: timeUnit }, timeUnitLoadPromiseDeferred);
                        });
                    promises.push(timeUnitLoadPromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);

                };
                    api.getData = function () {
                        return {
                            $type: "Vanrise.Common.MainExtensions.LastTimePeriod, Vanrise.Common.MainExtensions",
                            TimeUnit: timeUnitAPI.getSelectedIds(),
                            TimeValue:$scope.scopeModel.timeValue,
                        };
                    };

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                }
            }

            return directiveDefinitionObject;
        }]);