﻿'use strict';
app.directive('vrWhsBePricingproducts', ['WhS_BE_PricingProductAPIService', 'UtilsService',
    function (WhS_BE_PricingProductAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onloaded: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.selectedPricingProducts = [];
                $scope.pricingProducts = [];
                var bePricingProductObject = new bePricingProduct(ctrl, $scope);
                bePricingProductObject.initializeController();
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
                return getBePricingProductsTemplate(attrs);
            }

        };


        function getBePricingProductsTemplate(attrs) {
          
                var multipleselection = "";
                if (attrs.ismultipleselection != undefined)
                    multipleselection = "ismultipleselection"
                var required = "";
                if (attrs.isrequired != undefined)
                    required = "isrequired";
               
                return '<div  vr-loader="isLoadingDirective">'
                    + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="PricingProductId" '
                + required +' label="Pricing Product" datasource="pricingProducts" selectedvalues="selectedPricingProducts"  onselectionchanged="onselectionchanged"></vr-select>'
                   + '</div>'
        }

        function bePricingProduct(ctrl, $scope) {

            function initializeController() {

                loadPricingProducts();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return $scope.selectedPricingProducts;
                }

                api.setData = function (selectedIds) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedPricingProduct = UtilsService.getItemByVal($scope.pricingProducts, selectedIds[i], "PricingProductId");
                        if (selectedPricingProduct != null)
                            $scope.selectedPricingProducts.push(selectedPricingProduct);
                    }
                }

                if (ctrl.onloaded != null)
                    ctrl.onloaded(api);
            }
            function loadPricingProducts() {
                $scope.isLoadingDirective = true;
                return WhS_BE_PricingProductAPIService.GetAllPricingProduct().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.pricingProducts.push(itm);
                    });
                }).catch(function (error) {
                    //TODO handle the case of exceptions

                }).finally(function () {
                    $scope.isLoadingDirective = false;
                    defineAPI();
                });
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);