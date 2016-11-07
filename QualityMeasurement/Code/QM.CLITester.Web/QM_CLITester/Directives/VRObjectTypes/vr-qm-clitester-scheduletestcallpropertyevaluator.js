(function (app) {

    'use strict';

    ScheduleTestCallPropertyEvaluator.$inject = ['Qm_CliTester_ScheduleCallTestPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function ScheduleTestCallPropertyEvaluator(Qm_CliTester_ScheduleCallTestPropertyEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrScheduleTestCallPropertyEvaluatorSecurity = new VRScheduleTestCallPropertyEvaluator($scope, ctrl, $attrs);
                vrScheduleTestCallPropertyEvaluatorSecurity.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/QM_CLITester/Directives/VRObjectTypes/Templates/ScheduleTestCallPropertyEvaluatorTemplate.html'
        };

        function VRScheduleTestCallPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.scheduleTestCall = UtilsService.getArrayEnum(Qm_CliTester_ScheduleCallTestPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedScheduleTestCall = UtilsService.getItemByVal($scope.scopeModel.scheduleTestCall, payload.objectPropertyEvaluator.ScheduleTestCallField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "QM.CLITester.Entities.VRObjectTypes.ScheduleTestCallPropertyEvaluator, QM.CLITester.Entities",
                        TestCallDetailField: $scope.scopeModel.selectedScheduleTestCall.value
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrQmClitesterScheduletestcallpropertyevaluator', ScheduleTestCallPropertyEvaluator);

})(app);
