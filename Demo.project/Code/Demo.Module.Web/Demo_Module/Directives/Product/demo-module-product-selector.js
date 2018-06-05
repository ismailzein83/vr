'use strict';
app.directive('demoModuleProductSelector', ['VRNotificationService', 'Demo_Module_ProductAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_ProductAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            selectedvalues: '=',
            isrequired: "=",
            onselectitem: "=",
            ondeselectitem: "=",
            hideremoveicon: '@',
            normalColNum: '@',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            $scope.addNewProduct = function () {
                var onProductAdded = function (productObj) {
                    ctrl.datasource.push(productObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(productObj.Entity);
                    else
                        ctrl.selectedvalues = productObj.Entity;
                };
                Demo_Module_ProductService.addProduct(onProductAdded);
            };

            var productSelector = new ProductSelector(ctrl, $scope, $attrs);
            productSelector.initializeController();
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
            return getProductTemplate(attrs);
        }

    };

    function getProductTemplate(attrs) {

        var multipleselection = "";
        var label = "Product";
        if (attrs.ismultipleselection != undefined) {
            label = "Product";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewProduct"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns  colnum="{{ctrl.normalColNum}}"  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="ProductId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged=" ctrl.onselectionchanged" entityName="Product" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'
            + '</vr-select></vr-columns>';
    }

    function ProductSelector(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onselection = function (selectedProduct) {
                ctrl.onselectionchanged(selectedProduct);
            };

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.scopeModel.onCancelSearch = function (api) {
                $scope.scopeModel.searchProduct = undefined;
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;

                if (payload != undefined) {
                    selectedIds = [];
                    selectedIds.push(payload.selectedIds);
                    filter = payload.filter;
                }
                return getProductsInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('ProductId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getProductsInfo(attrs, ctrl, selectedIds, filter) {
        return Demo_Module_ProductAPIService.GetProductsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'ProductId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);