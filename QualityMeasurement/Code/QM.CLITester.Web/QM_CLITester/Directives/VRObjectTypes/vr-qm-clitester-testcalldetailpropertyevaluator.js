(function (app) {

    'use strict';

    TestCallDetailPropertyEvaluator.$inject = ['Qm_CliTester_CallTestDetailPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function TestCallDetailPropertyEvaluator(Qm_CliTester_CallTestDetailPropertyEnum, UtilsService, VRUIUtilsService) {
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
                var vrTestCallDetailPropertyEvaluatorSecurity = new VRTestCallDetailPropertyEvaluator($scope, ctrl, $attrs);
                vrTestCallDetailPropertyEvaluatorSecurity.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/QM_CLITester/Directives/VRObjectTypes/Templates/TestCallDetailPropertyEvaluatorTemplate.html'
        };

        function VRTestCallDetailPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.testCallDetails = UtilsService.getArrayEnum(Qm_CliTester_CallTestDetailPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedTestCallDetail = UtilsService.getItemByVal($scope.scopeModel.testCallDetails, payload.objectPropertyEvaluator.TestCallDetailField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities",
                        TestCallDetailField: $scope.scopeModel.selectedTestCallDetail.value
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrQmClitesterTestcalldetailpropertyevaluator', TestCallDetailPropertyEvaluator);

})(app);
