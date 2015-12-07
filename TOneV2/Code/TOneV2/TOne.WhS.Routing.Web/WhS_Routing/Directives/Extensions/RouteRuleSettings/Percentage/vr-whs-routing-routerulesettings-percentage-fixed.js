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
            ctrl.percentages = [];
            var index;

            function initializeController() {
                ctrl.disableButton = true;
                index = 0;
                
                ctrl.addPercentageOption = function () {
                    ctrl.percentages.push({
                        id: ctrl.percentages.length + 1,
                        percentage: ctrl.percentageValue
                    });
                    ctrl.percentageValue = undefined;
                };
                
                ctrl.onvaluechanged = function (value) {
                    var totalPercentage = 0;
                    for (var j = 0; j < ctrl.percentages.length; j++) {
                        totalPercentage = totalPercentage + parseInt(ctrl.percentages[j].percentage);
                    }
                    
                    if (value > 100 - totalPercentage)
                        ctrl.disableButton = true;
                    else
                        ctrl.disableButton = (value == undefined);
                }

                ctrl.removeFilter = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.percentages, dataItem.id, 'id');
                    ctrl.percentages.splice(index, 1);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined)
                    {
                        for (var i = 0; i < payload.Percentages.length; i++) {
                            ctrl.percentages.push({
                                id: i,
                                percentage: payload.Percentages[i]
                            });
                        }
                    }
                    
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.Percentages.FixedOptionPercentage, TOne.WhS.Routing.Business",
                        Percentages: UtilsService.getPropValuesFromArray(ctrl.percentages, "percentage"),
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);