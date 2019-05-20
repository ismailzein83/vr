﻿"use strict";

app.directive("vrRuntimeTasktriggerTimer", ['UtilsService', 'VRUIUtilsService', 'TimeSchedulerTypeEnum',
function (UtilsService, VRUIUtilsService, TimeSchedulerTypeEnum) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/Runtime/Directives/TaskTrigger/Templates/TaskTriggerTimer.html"
    };


    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var timerTypeDirectiveAPI;
        var timerTypeDirectiveReadyPromiseDeferred;

        function initializeController() {
            $scope.schedulerTypes = UtilsService.getArrayEnum(TimeSchedulerTypeEnum);

            $scope.onTimerTypeDirectiveReady = function (api) {
                timerTypeDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingTimerSection = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, timerTypeDirectiveAPI, undefined, setLoader, timerTypeDirectiveReadyPromiseDeferred);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            
            

            api.load = function (payload) {
                var data;
                var promises = [];

                if (payload != undefined && payload.data != undefined) {
                    data = payload.data;
                    $scope.ignoreSkippedIntervals = data.IgnoreSkippedIntervals;
                    $scope.selectedType = UtilsService.getItemByVal($scope.schedulerTypes, data.TimerTriggerTypeFQTN, "FQTN");
                }
                else {
                    setToDefaultValues();
                }

                if (data != undefined) {
                    timerTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    var loadTimerTypePromiseDeferred = UtilsService.createPromiseDeferred();
                    timerTypeDirectiveReadyPromiseDeferred.promise.then(function () {
                        timerTypeDirectiveReadyPromiseDeferred = undefined;
                        var payload = {
                            data: data
                        };
                        VRUIUtilsService.callDirectiveLoad(timerTypeDirectiveAPI, payload, loadTimerTypePromiseDeferred);
                    });
                    promises.push(loadTimerTypePromiseDeferred.promise);
                }

                function setToDefaultValues() {
                    $scope.selectedType = UtilsService.getItemByVal($scope.schedulerTypes, TimeSchedulerTypeEnum.Interval.FQTN, "FQTN");
                    $scope.ignoreSkippedIntervals = true;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data = timerTypeDirectiveAPI.getData();
                data.IgnoreSkippedIntervals = $scope.ignoreSkippedIntervals;
                return data;
            };

            if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        
    }

    return directiveDefinitionObject;
}]);
