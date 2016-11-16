"use strict";

app.directive("vrRuntimeTaskactionScheduledailyrepricing", ['UtilsService', 'VRUIUtilsService', 
function (UtilsService, VRUIUtilsService) {

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
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Runtime/Directives/TaskAction/Templates/TaskActionScheduleDailyRepricing.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            $scope.dateOptions = [{ Name: "Trigger Date", Value: 0 }, { Name: "Specific Date", Value: 1 }];
            var api = {};
           
            api.getData = function () {
                return {
                    $type: "TOne.CDRProcess.Arguments.DailyRepricingProcessInput, TOne.CDRProcess.Arguments",
                    RepricingDay: $scope.repricingDay,
                    DivideProcessIntoSubProcesses: $scope.divideProcessIntoSubProcesses
                };

            };
            api.getExpressionsData = function () {
                if ($scope.selectedDateOption.Value == 0) {
                    $scope.repricingDay = '';
                    return { "RepricingDay": "ScheduleTime" };
                }
                else
                    return undefined;

            };

            api.load = function (payload) {
                var data;
                if (payload != undefined && payload.data != undefined)
                    data = payload.data;

                if (data != undefined) {
                    $scope.repricingDay = data.RepricingDay;
                    $scope.divideProcessIntoSubProcesses = data.DivideProcessIntoSubProcesses;
                }


                $scope.selectedDateOption = UtilsService.getItemByVal($scope.dateOptions, payload.selectedDateOption, "Value");



            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
