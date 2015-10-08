'use strict';
app.directive('vrWhsBePricingproducts', ['WhS_BE_PricingProductAPIService', 'UtilsService','$compile',
function (WhS_BE_PricingProductAPIService, UtilsService,$compile) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onloaded: '=',
                ismultipleselection: "@",
                isdisabled:"=",
                onselectionchanged: '=',
                isrequired: "@",
               
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.selectedPricingProducts = [];
                $scope.pricingProducts = [];
                var bePricingProductObject = new bePricingProduct(ctrl, $scope, $attrs);
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
            link: function preLink($scope, iElement, iAttrs) {
                var ctrl = $scope.ctrl;
                $scope.$watch('ctrl.isdisabled', function () {
                    var template = getBePricingProductsTemplate(iAttrs, ctrl);
                    iElement.html(template);
                    $compile(iElement.contents())($scope);
                });
            }

        };


        function getBePricingProductsTemplate(attrs, ctrl) {

                var multipleselection = "";
                if (attrs.ismultipleselection != undefined)
                    multipleselection = "ismultipleselection"
                var required = "";
                if (attrs.isrequired != undefined)
                    required = "isrequired";
                var disabled = "";
                if (ctrl.isdisabled)
                    disabled = "vr-disabled='true'"
                return '<div  vr-loader="isLoadingDirective">'
                    + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="PricingProductId" '
                + required + ' label="Pricing Product" datasource="pricingProducts" selectedvalues="selectedPricingProducts"  onselectionchanged="onselectionchanged" ' + disabled + '></vr-select>'
                   + '</div>'
        }

        function bePricingProduct(ctrl, $scope, $attrs) {

            function initializeController() {

                loadPricingProducts();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return $scope.selectedPricingProducts;
                }

                api.setData = function (selectedIds) {
                   
                    if ($attrs.ismultipleselection) {
                        for (var i = 0; i < selectedIds.length; i++) {
                            var selectedPricingProduct = UtilsService.getItemByVal($scope.pricingProducts, selectedIds[i], "PricingProductId");
                            if (selectedPricingProduct != null)
                                $scope.selectedPricingProducts.push(selectedPricingProduct);
                        }
                    }
                    else {
                        var selectedPricingProduct = UtilsService.getItemByVal($scope.pricingProducts, selectedIds, "PricingProductId");
                        if (selectedPricingProduct != null)
                            $scope.selectedPricingProducts=selectedPricingProduct;
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