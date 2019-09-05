"use strict";

app.directive("vrCommonTimeperiodLastTime", ['UtilsService', 'VRUIUtilsService', 'VRCommon_TimeunitEnum', 'VRCommon_StartingFromEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_TimeunitEnum, VRCommon_StartingFromEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                isrequired: '=',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimePeriodLastTime($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRTimePeriod/MainExtensions/Templates/LastTimeDirectiveTemplate.html"
        };

        function TimePeriodLastTime($scope, ctrl) {
            this.initializeController = initializeController;

            var timeUnitAPI;
            var timeUnitReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var startingfromAPI;
            var startingfromReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var offsetTimeUnitAPI;
            var offsetTimeUnitReadyPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.executionTimeWithOffsetValue = VRCommon_StartingFromEnum.ExecutionTimeWithOffset.value;

                $scope.scopeModel.onTimeUnitReady = function (api) {
                    timeUnitAPI = api;
                    timeUnitReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onStartingfromReady = function (api) {
                    startingfromAPI = api;
                    startingfromReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onOffsetTimeUnitReady = function (api) {
                    offsetTimeUnitAPI = api;
                    $scope.scopeModel.offsetValue = undefined;

                    var offsetTimeUnitPayload = {
                        timeUnit: VRCommon_TimeunitEnum.Day.value
                    };

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, offsetTimeUnitAPI, offsetTimeUnitPayload, setLoader, offsetTimeUnitReadyPromiseDeferred);
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
                    var offsetTimeUnit;
                    var offsetValue;

                    if (payload != undefined) {
                        if (payload.timePeriod != undefined) {
                            timeUnit = payload.timePeriod.TimeUnit;
                            startingfrom = payload.timePeriod.StartingFrom;
                            offsetTimeUnit = payload.timePeriod.OffsetTimeUnit;

                            $scope.scopeModel.timeValue = payload.timePeriod.TimeValue;
                            offsetValue = payload.timePeriod.OffsetValue;
                        }
                    }

                    if (offsetTimeUnit != undefined) {
                        offsetTimeUnitReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var loadOffsetTimeUnitPromise = loadOffsetTimeUnit();
                        loadOffsetTimeUnitPromise.then(function () {
                            $scope.scopeModel.offsetValue = offsetValue;
                        });

                        promises.push(loadOffsetTimeUnitPromise);
                    }

                    promises.push(loadTimeUnit());
                    promises.push(loadStartingFrom());

                    function loadTimeUnit() {

                        var timeunitPayload = {
                            timeUnit: timeUnit != undefined ? timeUnit : VRCommon_TimeunitEnum.Day.value
                        };
                        return timeUnitAPI.load(timeunitPayload);
                    }
                    function loadStartingFrom() {

                        var startingFromPayload = {
                            startingfrom: startingfrom != undefined ? startingfrom : VRCommon_StartingFromEnum.ExecutionTime.value
                        };
                        return startingfromAPI.load(startingFromPayload);
                    }
                    function loadOffsetTimeUnit() {
                        var offsetTimeUnitLoadDeferred = UtilsService.createPromiseDeferred();

                        offsetTimeUnitReadyPromiseDeferred.promise.then(function () {
                            offsetTimeUnitReadyPromiseDeferred = undefined;

                            var offsetTimeUnitPayload = {
                                timeUnit: offsetTimeUnit != undefined ? offsetTimeUnit : VRCommon_TimeunitEnum.Day.value
                            };
                            VRUIUtilsService.callDirectiveLoad(offsetTimeUnitAPI, offsetTimeUnitPayload, offsetTimeUnitLoadDeferred);
                        });

                        return offsetTimeUnitLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var offsetTimeUnit = null;
                    var offsetValue = null;
                    if ($scope.scopeModel.selectedStartingFrom.value == VRCommon_StartingFromEnum.ExecutionTimeWithOffset.value) {
                        offsetTimeUnit = offsetTimeUnitAPI.getSelectedIds();
                        offsetValue = $scope.scopeModel.offsetValue;
                    }

                    return {
                        $type: "Vanrise.Common.MainExtensions.LastTimePeriod, Vanrise.Common.MainExtensions",
                        StartingFrom: startingfromAPI.getSelectedIds(),
                        TimeUnit: timeUnitAPI.getSelectedIds(),
                        TimeValue: $scope.scopeModel.timeValue,
                        OffsetTimeUnit: offsetTimeUnit,
                        OffsetValue: offsetValue
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);