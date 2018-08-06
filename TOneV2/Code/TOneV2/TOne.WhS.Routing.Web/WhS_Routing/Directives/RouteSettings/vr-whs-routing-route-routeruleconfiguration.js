'use strict';

app.directive('vrWhsRoutingRouteRouteruleconfiguration', ['UtilsService', 'WhS_Routing_FixedOptionLossTypeEnum',
    function (UtilsService, WhS_Routing_FixedOptionLossTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RouteRuleConfigurationDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/RouteRuleConfigurationTemplate.html"
        };

        function RouteRuleConfigurationDirectiveCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var fixedOptionLossTypeSelectorAPI;
            var fixedOptionLossTypeSelectorDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = UtilsService.getArrayEnum(WhS_Routing_FixedOptionLossTypeEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    fixedOptionLossTypeSelectorAPI = api;
                    fixedOptionLossTypeSelectorDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.selectedvalues = UtilsService.getItemByVal($scope.scopeModel.datasource, payload.FixedOptionLossType, "value");
                        $scope.scopeModel.fixedOptionLossDefaultValue = payload.FixedOptionLossDefaultValue;
                    }
                };

                api.getData = function () {

                    var selectedValue = $scope.scopeModel.selectedvalues;

                    var obj = {
                        $type: "TOne.WhS.Routing.Entities.RouteRuleConfiguration, TOne.WhS.Routing.Entities",
                        FixedOptionLossType: $scope.scopeModel.selectedvalues != undefined ? $scope.scopeModel.selectedvalues.value : null,
                        FixedOptionLossDefaultValue: $scope.scopeModel.fixedOptionLossDefaultValue
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);