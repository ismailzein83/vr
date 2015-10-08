'use strict';
app.directive('vrWhsBeRoutingproduct', ['WhS_BE_RoutingProductAPIService', 'UtilsService',
    function (WhS_BE_RoutingProductAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onloaded: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                salezonepackageid: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.selectedRoutingProducts;
                if ($attrs.ismultipleselection!=undefined)
                    $scope.selectedRoutingProducts = [];

                   
                $scope.routingProducts = [];
                var beRoutingProductObject = new beRoutingProduct(ctrl, $scope, $attrs);
                beRoutingProductObject.initializeController();
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
                        $scope.$watch('ctrl.salezonepackageid', function () {
                            if (ctrl.salezonepackageid != undefined) { 
                                $scope.isLoadingDirective = true;
                                WhS_BE_RoutingProductAPIService.GetRoutingProductsInfoBySaleZonePackage(ctrl.salezonepackageid).then(function (response) {
                                    $scope.routingProducts.length=0;
                                    angular.forEach(response, function (itm) {
                                        $scope.routingProducts.push(itm);
                                    });
                                }).catch(function (error) {
                                    //TODO handle the case of exceptions

                                }).finally(function () {
                                    $scope.isLoadingDirective = false;
                                });
                            }
                           
                        });
                    }
                }
            },
            template: function (element, attrs) {
                return getBeRoutingProductTemplate(attrs);
            }

        };


        function getBeRoutingProductTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection"
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div  vr-loader="isLoadingDirective">'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="RoutingProductId" '
            + required + ' label="Routing Products" datasource="routingProducts" selectedvalues="selectedRoutingProducts"  onselectionchanged="onselectionchanged" entityName="Routing Products"></vr-select>'
               + '</div>'
        }

        function beRoutingProduct(ctrl, $scope, $attrs) {

            function initializeController() {
                
                loadRoutingProducts();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return $scope.selectedRoutingProducts;
                }

                api.setData = function (selectedIds) {
                    if ($attrs.ismultipleselection) {
                        for (var i = 0; i < selectedIds.length; i++) {
                            var selectedRoutingProduct = UtilsService.getItemByVal($scope.routingProducts, selectedIds[i], "RoutingProductId");
                            if (selectedRoutingProduct != null)
                                $scope.selectedRoutingProducts.push(selectedRoutingProduct);
                        }
                    }
                    else {
                        var selectedRoutingProduct = UtilsService.getItemByVal($scope.routingProducts, selectedIds, "RoutingProductId");
                        console.log($scope.routingProducts);
                        console.log(selectedIds);
                        if (selectedRoutingProduct != null)
                            $scope.selectedRoutingProducts=selectedRoutingProduct;
                    }
                      
                }

                if (ctrl.onloaded != null)
                    ctrl.onloaded(api);
            }
            function loadRoutingProducts() {
                if ($attrs.salezonepackageid == undefined)
                {
                    $scope.isLoadingDirective = true;
                    return WhS_BE_RoutingProductAPIService.GetRoutingProducts().then(function (response) {
                        angular.forEach(response, function (itm) {
                            $scope.routingProducts.push(itm);
                        });
                    }).catch(function (error) {
                        //TODO handle the case of exceptions

                    }).finally(function () {
                        $scope.isLoadingDirective = false;
                        defineAPI();
                    });
                }
                else
                {
                    defineAPI();
                }
             
               
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);