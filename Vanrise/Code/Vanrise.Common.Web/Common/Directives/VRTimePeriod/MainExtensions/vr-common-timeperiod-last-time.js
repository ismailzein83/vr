"use strict";

app.directive("vrCommonTimeperiodLastTime", ['UtilsService', 'VRUIUtilsService', 'VRCommon_TimeunitEnum', 'VRCommon_StartingFromEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_TimeunitEnum, VRCommon_StartingFromEnum) {

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
            var startingfromAPI;
            var startingfromReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onTimeUnitReady = function (api) {
                    timeUnitAPI = api;
                    timeUnitReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onStartingfromReady = function (api) {
                    startingfromAPI = api;
                    startingfromReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([timeUnitReadyPromiseDeferred.promise, startingfromReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var timeUnit;
                    var startingfrom;

                    if (payload != undefined) {
                        $scope.scopeModel.timeValue = payload.timePeriod.TimeValue;
                        timeUnit = payload.timePeriod.TimeUnit;
                        startingfrom = payload.timePeriod.StartingFrom;
                    }

                    function loadTimeUnit() {
                        var timeunitPayload = {
                            timeUnit: timeUnit
                        };
                        return timeUnitAPI.load(timeunitPayload);
                    }

                    function loadStartingFrom() {

                        var startingFromPayload = {
                            startingfrom: startingfrom
                        };
                        return startingfromAPI.load(startingFromPayload);
                    }

                    promises.push(loadTimeUnit());
                    promises.push(loadStartingFrom());

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.LastTimePeriod, Vanrise.Common.MainExtensions",
                        TimeUnit: timeUnitAPI.getSelectedIds(),
                        StartingFrom: startingfromAPI.getSelectedIds(),
                        TimeValue: $scope.scopeModel.timeValue,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);