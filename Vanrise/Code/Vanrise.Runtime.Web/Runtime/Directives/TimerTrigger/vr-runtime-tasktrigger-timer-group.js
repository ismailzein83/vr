"use strict";

app.directive("vrRuntimeTasktriggerTimerGroup", ['UtilsService', 'VRUIUtilsService','TimeSchedulerTypeEnum',
function (UtilsService,VRUIUtilsService ,TimeSchedulerTypeEnum) {

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
        return "/Client/Modules/Runtime/Directives/TimerTrigger/Templates/TaskTriggerTimerGroup.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

       
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            $scope.schedulerTypes = UtilsService.getArrayEnum(TimeSchedulerTypeEnum);
            api.getData = function () {
                
            };


            api.load = function (payload) {

                console.log(payload)
                var data;
                if (payload != undefined && payload.data != undefined) {
                    data = payload.data;
                    $scope.selectedType = UtilsService.getItemByVal($scope.schedulerTypes, data.TimerTriggerTypeFQTN, "FQTN");
                    $scope.schedulerTypeTaskTrigger = {};
                    $scope.schedulerTypeTaskTrigger.data = data;
                    if ($scope.schedulerTypeTaskTrigger.loadTemplateData != undefined)
                        $scope.schedulerTypeTaskTrigger.loadTemplateData();
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
