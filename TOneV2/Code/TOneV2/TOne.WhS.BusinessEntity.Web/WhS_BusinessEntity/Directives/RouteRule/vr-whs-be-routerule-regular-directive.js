'use strict';
app.directive('vrWhsBeRouteruleRegular', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var beRouteRuleRegularCtor = new beRouteRuleRegularCtor(ctrl, $scope);
                beRouteRuleRegularCtor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return getBeRouteRuleRegularTemplate(attrs);
            }

        };

        function getBeRouteRuleRegularTemplate(attrs) {
            return '/Client/Modules/WhS_BusinessEntity/Directives/RouteRule/Templates/RouteRuleRegularDirectiveTemplate.html';
        }

        function beRouteRuleRegularCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.RegularRouteRule, TOne.WhS.Routing.Business.RouteRules",
                        OptionsSettingsGroup: routeOptionSettingsGroupDirectiveAPI.getData(),
                        OptionOrderSettings: routeRuleOptionOrderSettingsDirectiveAPI.getData(),
                        OptionFilterSettings: routeRuleOptionFilterSettingsDirectiveAPI.getData(),
                        OptionPercentageSettings: routeRuleOptionPercentageSettingsDirectiveAPI.getData()
                    };
                }

                api.setData = function (routeRuleSettings) {
                    return UtilsService.waitMultipleAsyncOperations([setRouteOptionSettings, setRouteRuleOptionOrderSettings, setRouteRuleOptionFilterSettings, setRouteRuleOptionPercentageSettings]).then(function () {
                        defineAPI();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });

                    function setRouteOptionSettings() {
                        routeOptionSettingsGroupDirectiveAPI.setData(routeRuleSettings.OptionsSettingsGroup);
                    };
                    function setRouteRuleOptionOrderSettings() {
                        routeRuleOptionOrderSettingsDirectiveAPI.setData(routeRuleSettings.OptionOrderSettings);
                    };
                    function setRouteRuleOptionFilterSettings() {
                        routeRuleOptionFilterSettingsDirectiveAPI.setData(routeRuleSettings.OptionFilterSettings);
                    };
                    function setRouteRuleOptionPercentageSettings() {
                        routeRuleOptionPercentageSettingsDirectiveAPI.setData(routeRuleSettings.OptionPercentageSettings);
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);