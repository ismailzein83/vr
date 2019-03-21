(function (app) {

    'use strict';

    LogicalOperatorDirective.$inject = ['VR_GenericData_RecordQueryLogicalOperatorEnum', 'UtilsService'];

    function LogicalOperatorDirective(VR_GenericData_RecordQueryLogicalOperatorEnum, UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LogicalOperator($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordFilter/Templates/LogicalOperatorTemplate.html"

        };


        function LogicalOperator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    $scope.scopeModel.operators = UtilsService.getArrayEnum(VR_GenericData_RecordQueryLogicalOperatorEnum);

                    if (payload != undefined)
                        $scope.scopeModel.operator = payload.LogicalOperator;

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return $scope.scopeModel.operator;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

        }
    }

    app.directive('vrGenericdataRecordfilterLogicaloperator', LogicalOperatorDirective);

})(app);