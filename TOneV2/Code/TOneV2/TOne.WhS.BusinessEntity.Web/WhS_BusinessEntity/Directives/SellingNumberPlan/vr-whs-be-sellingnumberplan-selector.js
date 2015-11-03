'use strict';
app.directive('vrWhsBeSellingnumberplanSelector', ['WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.selectedSellingNumberPlans;
                if ($attrs.ismultipleselection != undefined)
                    $scope.selectedSellingNumberPlans = [];

                ctrl.datasource = [];

                var ctor = new sellingNumberPlanCtor(ctrl, $scope, $attrs);
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
            template: function (element, attrs) {
                return getBeSellingNumberPlansTemplate(attrs);
            }

        };


        function getBeSellingNumberPlansTemplate(attrs) {

            var multipleselection = "";
            var label = "Selling Number Plan";
            if (attrs.ismultipleselection != undefined)
            {
                label = "Selling Number Plans";
                multipleselection = "ismultipleselection";
            }

            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SellingNumberPlanId" '
            + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="Selling Number Plan"></vr-select>'
               + '</div>'
        }

        function sellingNumberPlanCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SellingNumberPlanId', attrs, ctrl);
                        }
                    });
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SellingNumberPlanId', attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);