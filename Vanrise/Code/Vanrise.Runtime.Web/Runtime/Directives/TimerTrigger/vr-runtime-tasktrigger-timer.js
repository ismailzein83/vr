"use strict";

app.directive("vrRuntimeTasktriggerTimer", ['UtilsService', 'VRUIUtilsService','TimeSchedulerTypeEnum',
function (UtilsService ,VRUIUtilsService ,TimeSchedulerTypeEnum) {

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
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Runtime/Directives/TimerTrigger/Templates/TaskTriggerTimer.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

       
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            $scope.schedulerTypes = UtilsService.getArrayEnum(TimeSchedulerTypeEnum);
            var timerTypeDirectiveAPI;
            var timerTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.onTimerTypeDirectiveReady = function (api) {
                timerTypeDirectiveAPI = api;
                timerTypeDirectiveReadyPromiseDeferred.resolve();
            }
            api.getData = function () {
                return timerTypeDirectiveAPI.getData();
            };


            api.load = function (payload) {
                var data;
                var promises = [];
                if (payload != undefined && payload.data != undefined) {
                    data = payload.data;
                    $scope.selectedType = UtilsService.getItemByVal($scope.schedulerTypes, data.TimerTriggerTypeFQTN, "FQTN");
                }

                var loadTimerTypePromiseDeferred = UtilsService.createPromiseDeferred();
                timerTypeDirectiveReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        data:data
                    };
                    VRUIUtilsService.callDirectiveLoad(timerTypeDirectiveAPI, payload, loadTimerTypePromiseDeferred);
                });

                promises.push(loadTimerTypePromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
