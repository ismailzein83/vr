(function (app) {

    'use strict';

    RoutingGroupFilterConditionDirective.$inject = ['Retail_Teles_RoutingGroupFilterOperatorEnum', 'UtilsService', 'VRUIUtilsService'];

    function RoutingGroupFilterConditionDirective(Retail_Teles_RoutingGroupFilterOperatorEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RoutingGroupFilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/RoutingGroupCondition/MainExtensions/Templates/RoutingGroupFilterTemplate.html"
        };

        function RoutingGroupFilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.routingGroupFilterOperators = UtilsService.getArrayEnum(Retail_Teles_RoutingGroupFilterOperatorEnum);
                $scope.scopeModel.selectedRoutingGroupFilterOperator = $scope.scopeModel.routingGroupFilterOperators[0];
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var routingGroupCondition;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        routingGroupCondition = payload.routingGroupCondition;
                        if(routingGroupCondition != undefined)
                        {
                            $scope.scopeModel.routingGroupName = routingGroupCondition.RoutingGroupName;

                            $scope.scopeModel.selectedRoutingGroupFilterOperator = UtilsService.getItemByVal($scope.scopeModel.routingGroupFilterOperators, routingGroupCondition.Operator, "value")
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Retail.Teles.Business.RoutingGroupFilter, Retail.Teles.Business",
                        RoutingGroupName: $scope.scopeModel.routingGroupName,
                        Operator: $scope.scopeModel.selectedRoutingGroupFilterOperator.value
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailTelesRoutinggroupconditionRoutinggroupfilter', RoutingGroupFilterConditionDirective);

})(app);