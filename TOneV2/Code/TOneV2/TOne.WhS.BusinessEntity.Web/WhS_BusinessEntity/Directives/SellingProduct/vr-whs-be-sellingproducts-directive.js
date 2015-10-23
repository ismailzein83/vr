'use strict';
app.directive('vrWhsBeSellingproducts', ['WhS_BE_SellingProductAPIService', 'UtilsService','$compile',
function (WhS_BE_SellingProductAPIService, UtilsService,$compile) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                isdisabled:"=",
                onselectionchanged: '=',
                isrequired: "@",
               
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.selectedSellingProducts;
                if ($attrs.ismultipleselection != undefined)
                    $scope.selectedSellingProducts = [];

                $scope.sellingProducts = [];
                var beSellingProductObject = new beSellingProduct(ctrl, $scope, $attrs);
                beSellingProductObject.initializeController();
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
                    var template = getBeSellingProductsTemplate(iAttrs, ctrl);
                    iElement.html(template);
                    $compile(iElement.contents())($scope);
                });
            }

        };


        function getBeSellingProductsTemplate(attrs, ctrl) {

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
                    + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SellingProductId" '
                + required + ' label="Pricing Product" datasource="sellingProducts" selectedvalues="selectedSellingProducts"  onselectionchanged="onselectionchanged" ' + disabled + '></vr-select>'
                   + '</div>'
        }

        function beSellingProduct(ctrl, $scope, $attrs) {

            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return $scope.selectedSellingProducts;
                }

                api.setData = function (selectedIds) {
                   
                    if ($attrs.ismultipleselection) {
                        for (var i = 0; i < selectedIds.length; i++) {
                            var selectedSellingProduct = UtilsService.getItemByVal($scope.sellingProducts, selectedIds[i], "SellingProductId");
                            if (selectedSellingProduct != null)
                                $scope.selectedSellingProducts.push(selectedSellingProduct);
                        }
                    }
                    else {
                        var selectedSellingProduct = UtilsService.getItemByVal($scope.sellingProducts, selectedIds, "SellingProductId");
                        if (selectedSellingProduct != null)
                            $scope.selectedSellingProducts=selectedSellingProduct;
                    }
                }
                api.load = function () {
                    return WhS_BE_SellingProductAPIService.GetAllSellingProduct().then(function (response) {
                        angular.forEach(response, function (itm) {
                            $scope.sellingProducts.push(itm);
                        });
                    }).catch(function (error) {

                    }).finally(function () {
                       
                    });
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);