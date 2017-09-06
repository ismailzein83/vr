'use strict';
app.directive('vrWhsBeSellingproductSelector', ['WhS_BE_SellingProductAPIService', 'UtilsService', '$compile', 'VRUIUtilsService', 'WhS_BE_SellingProductService',
function (WhS_BE_SellingProductAPIService, UtilsService, $compile, VRUIUtilsService, WhS_BE_SellingProductService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            isrequired: "=",
            selectedvalues: '=',
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

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewSellingProduct"';

        return '<vr-columns colnum="{{ctrl.normalColNum}}">'
            + '<vr-select  isrequired="ctrl.isrequired" ' + multipleselection + ' ' + hideremoveicon + ' ' + addCliked + ' datatextfield="Name" datavaluefield="SellingProductId" '
        + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" haspermission="ctrl.haspermission"></vr-select>'
            + '</vr-columns>';
    }

    function sellingProductCtor(ctrl, $scope, $attrs) {

        var selectorApi;
        var filter;
        $scope.addNewSellingProduct = function () {
            var sellingNumberPlanId = filter != undefined && filter.SellingNumberPlanId ? filter.SellingNumberPlanId : undefined;
            var onSellingProductAdded = function (sellingProductObj) {
                ctrl.datasource.push(sellingProductObj.Entity);
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues.push(sellingProductObj.Entity);
                else
                    ctrl.selectedvalues = sellingProductObj.Entity;
            };
            WhS_BE_SellingProductService.addSellingProduct(onSellingProductAdded, sellingNumberPlanId);
        };
        ctrl.haspermission = function () {
            return WhS_BE_SellingProductAPIService.HasAddSellingProductPermission();
        };

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

                var selectIfSingleItem;
                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                    selectIfSingleItem = payload.selectifsingleitem;
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
                    else if (selectedIds == undefined && selectIfSingleItem == true) {
                        selectorApi.selectIfSingleItem();
                    }
                });
            };

            api.clearDataSource = function () {
                selectorApi.clearDataSource();
            };

            api.resetFilter = function () {
                filter = undefined;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);