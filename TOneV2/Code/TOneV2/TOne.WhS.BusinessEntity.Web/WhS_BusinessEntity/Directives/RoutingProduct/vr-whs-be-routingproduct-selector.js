﻿'use strict';
app.directive('vrWhsBeRoutingproductSelector', ['WhS_BE_RoutingProductAPIService', 'UtilsService',
    function (WhS_BE_RoutingProductAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                sellingnumberplanid: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.selectedRoutingProducts;
                if ($attrs.ismultipleselection!=undefined)
                    $scope.selectedRoutingProducts = [];
                $scope.filteredRoutingProducts = [];
                   
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
                        $scope.$watch('ctrl.sellingnumberplanid', function () {
                            if (ctrl.sellingnumberplanid != undefined) { 
                                $scope.isLoadingDirective = true;
                                $scope.filteredRoutingProducts.length = 0;
                                for (var i = 0; i < $scope.routingProducts.length; i++) {
                                    if ($scope.routingProducts[i].SellingNumberPlanId == ctrl.sellingnumberplanid)
                                        $scope.filteredRoutingProducts.push($scope.routingProducts[i]);
                                }
                                $scope.isLoadingDirective = false;
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
            var label = "Routing Product";
            if (attrs.ismultipleselection != undefined) {
                label = "Routing Products";
                multipleselection = "ismultipleselection";
            }
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="RoutingProductId" '
            + required + ' label="' + label + '" datasource="filteredRoutingProducts" selectedvalues="selectedRoutingProducts"  onselectionchanged="onselectionchanged" entityName="' + label + '"></vr-select>'
               + '</div>'
        }

        function beRoutingProduct(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (selectedIds) {
                    return WhS_BE_RoutingProductAPIService.GetRoutingProducts().then(function (response) {
                        //Load Data Region
                        angular.forEach(response, function (itm) {
                            $scope.routingProducts.push(itm);
                        });
                        if ($attrs.sellingnumberplanid == undefined)
                            $scope.filteredRoutingProducts = $scope.routingProducts;

                        setData(selectedIds);
                    });

                    function setData(selectedIds)
                    {
                        if (selectedIds == undefined)
                            return;

                        if ($attrs.ismultipleselection) {
                            for (var i = 0; i < selectedIds.length; i++) {
                                var selectedRoutingProduct = UtilsService.getItemByVal($scope.routingProducts, selectedIds[i], "RoutingProductId");
                                if (selectedRoutingProduct != null)
                                    $scope.selectedRoutingProducts.push(selectedRoutingProduct);
                            }
                        }
                        else {
                            var selectedRoutingProduct = UtilsService.getItemByVal($scope.routingProducts, selectedIds, "RoutingProductId");
                            if (selectedRoutingProduct != null)
                                $scope.selectedRoutingProducts = selectedRoutingProduct;
                        }
                    }
                }

                api.getData = function () {
                    return $scope.selectedRoutingProducts;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);