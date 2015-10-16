'use strict';
app.directive('vrWhsBeSalezonepackages', ['WhS_BE_SaleZonePackageAPIService', 'UtilsService',
    function (WhS_BE_SaleZonePackageAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                $scope.selectedSaleZonePackages;
                if ($attrs.ismultipleselection != undefined)
                $scope.selectedSaleZonePackages = [];
                $scope.saleZonePackages = [];
                var beSaleZonePackageObject = new beSaleZonePackage(ctrl, $scope, $attrs);
                beSaleZonePackageObject.initializeController();
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
                return getBeSaleZonePackagesTemplate(attrs);
            }

        };


        function getBeSaleZonePackagesTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection"
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SaleZonePackageId" '
            + required + ' label="Sale Zone Packages" datasource="saleZonePackages" selectedvalues="selectedSaleZonePackages"  onselectionchanged="onselectionchanged" entityName="Sale Zone Package"></vr-select>'
               + '</div>'
        }

        function beSaleZonePackage(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    return WhS_BE_SaleZonePackageAPIService.GetSaleZonePackages().then(function (response) {
                        angular.forEach(response, function (itm) {
                            $scope.saleZonePackages.push(itm);
                        });
                    });
                }

                api.getData = function () {
                    return $scope.selectedSaleZonePackages;
                }

                api.setData = function (selectedIds) {
                    if ($attrs.ismultipleselection) {
                        for (var i = 0; i < selectedIds.length; i++) {
                            var selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, selectedIds[i], "SaleZonePackageId");
                            if (selectedSaleZonePackage != null)
                                $scope.selectedSaleZonePackages.push(selectedSaleZonePackage);
                        }
                    } else {
                        var selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, selectedIds, "SaleZonePackageId");
                        if (selectedSaleZonePackage != null)
                            $scope.selectedSaleZonePackages=selectedSaleZonePackage;
                    }
                    
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);