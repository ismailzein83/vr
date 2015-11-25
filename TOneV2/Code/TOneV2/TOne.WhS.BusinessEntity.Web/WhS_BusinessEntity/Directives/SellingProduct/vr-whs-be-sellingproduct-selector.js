'use strict';
app.directive('vrWhsBeSellingproductSelector', ['WhS_BE_SellingProductAPIService', 'UtilsService','$compile','VRUIUtilsService',
function (WhS_BE_SellingProductAPIService, UtilsService, $compile, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                isdisabled:"=",
                onselectionchanged: '=',
                isrequired: "@",
                selectedvalues:'=',
                hideremoveicon: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new sellingProductCtor(ctrl, $scope, $attrs);
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
                return getTemplate(attrs);
            }

        };
        
        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "Selling Product";
            var hideremoveicon = "";

            if (attrs.ismultipleselection != undefined) {
                label = "Selling Products";
                multipleselection = "ismultipleselection"
            }
            else if (attrs.hideremoveicon != undefined) {
                hideremoveicon = "hideremoveicon";
            }

            var required = "";
            if (attrs.isrequired != undefined)
                required = 'isrequired="' + attrs.isrequired + '"';

            return '<div  vr-loader="isLoadingDirective">'
                + '<vr-select ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="Name" datavaluefield="SellingProductId" '
            + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged"  vr-disabled="ctrl.isdisabled"></vr-select>'
                + '</div>';
        }

        function sellingProductCtor(ctrl, $scope, $attrs) {

            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SellingProductId', $attrs, ctrl);
                }
                api.load = function (payload) {

                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload;
                    }
                    return WhS_BE_SellingProductAPIService.GetAllSellingProduct().then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SellingProductId', $attrs, ctrl);
                        }
                    });
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);