'use strict';
app.directive('vrWhsRoutingRouterulesettingsPercentageFixed', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new percentageFixedCtor(ctrl, $scope);
                ctor.initializeController();

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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Percentage/Templates/OptionPercentageFixedDirective.html';
            }

        };

        function percentageFixedCtor(ctrl, $scope) {
            $scope.percentages = [];
            var index;

            function initializeController() {

                index = 0;
                $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
                $scope.addPercentageOption = function () {
                    $scope.percentages.push({
                        id: index++,
                        percentage: $scope.percentageValue
                    });
                };

                $scope.removePercentage = function ($event, option) {
                    $event.preventDefault();
                    $event.stopPropagation();
                    var index = UtilsService.getItemIndexByVal($scope.percentages, option.id, 'id');
                    $scope.percentages.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined)
                    {
                        for (var i = 0; i < payload.Percentages.length; i++) {
                            $scope.percentages.push({
                                id: i,
                                percentage: payload.Percentages[i]
                            });
                        }
                    }
                    
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.Percentages.FixedOptionPercentage, TOne.WhS.Routing.Business",
                        Percentages: UtilsService.getPropValuesFromArray($scope.percentages, "percentage"),
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);