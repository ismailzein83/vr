'use strict';
app.directive('vrWhsBeSellingproductSelector', ['WhS_BE_SellingProductAPIService', 'UtilsService','$compile','VRUIUtilsService',
function (WhS_BE_SellingProductAPIService, UtilsService, $compile, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                selectedvalues:'=',
                hideremoveicon: "@",
                normalColNum: '@'
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
                };
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
                multipleselection = "ismultipleselection";
            }
            else if (attrs.hideremoveicon != undefined) {
                hideremoveicon = "hideremoveicon";
            }

           
            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-select  isrequired="ctrl.isrequired" ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="Name" datavaluefield="SellingProductId" '
            + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged"></vr-select>'
                + '</vr-columns>';
        }

        function sellingProductCtor(ctrl, $scope, $attrs) {

            var selectorApi;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SellingProductId', $attrs, ctrl);
                };
                api.load = function (payload) {

                    selectorApi.clearDataSource();

                    var selectedIds;
                    var filter;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_BE_SellingProductAPIService.GetSellingProductsInfo(serializedFilter).then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SellingProductId', $attrs, ctrl);
                        }
                    });
                };

                api.clearDataSource = function () {
                    selectorApi.clearDataSource();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);