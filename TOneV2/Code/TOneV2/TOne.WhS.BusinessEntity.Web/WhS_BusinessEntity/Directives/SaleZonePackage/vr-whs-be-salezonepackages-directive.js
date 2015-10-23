'use strict';
app.directive('vrWhsBeSellingnumberplan', ['WhS_BE_SellingNumberPlanAPIService', 'UtilsService',
    function (WhS_BE_SellingNumberPlanAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                $scope.selectedSellingNumberPlans;
                if ($attrs.ismultipleselection != undefined)
                $scope.selectedSellingNumberPlans = [];
                $scope.sellingNumberPlans = [];
                var beSellingNumberPlanObject = new beSellingNumberPlan(ctrl, $scope, $attrs);
                beSellingNumberPlanObject.initializeController();
                $scope.onselectionchanged = function () {

                    if (ctrl.onselectionchanged != undefined) {
                        var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                        if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                            onvaluechangedMethod();
                        }
                    }

                }
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getBeSellingNumberPlansTemplate(attrs);
            }

        };


        function getBeSellingNumberPlansTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection"
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SellingNumberPlanId" '
            + required + ' label="Selling Number Plan" datasource="sellingNumberPlans" selectedvalues="selectedSellingNumberPlans"  onselectionchanged="onselectionchanged" entityName="Selling Number Plan"></vr-select>'
               + '</div>'
        }

        function beSellingNumberPlan(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                        angular.forEach(response, function (itm) {
                            $scope.sellingNumberPlans.push(itm);
                        });
                    });
                }

                api.getData = function () {
                    return $scope.selectedSellingNumberPlans;
                }

                api.setData = function (selectedIds) {
                    if ($attrs.ismultipleselection) {
                        for (var i = 0; i < selectedIds.length; i++) {
                            var selectedSellingNumberPlan = UtilsService.getItemByVal($scope.sellingNumberPlans, selectedIds[i], "SellingNumberPlanId");
                            if (selectedSellingNumberPlan != null)
                                $scope.selectedSellingNumberPlans.push(selectedSellingNumberPlan);
                        }
                    } else {
                        var selectedSellingNumberPlan = UtilsService.getItemByVal($scope.sellingNumberPlans, selectedIds, "SellingNumberPlanId");
                        if (selectedSellingNumberPlan != null)
                            $scope.selectedSellingNumberPlans=selectedSellingNumberPlan;
                    }
                    
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);