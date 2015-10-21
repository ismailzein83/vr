﻿'use strict';
app.directive('vrWhsBeSupplierzone', ['WhS_BE_SupplierZoneAPIService', 'UtilsService',
    function (WhS_BE_SupplierZoneAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                supplierid:"="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                $scope.selectedSupplierZones;
                if ($attrs.ismultipleselection != undefined)
                    $scope.selectedSupplierZones = [];
                $scope.supplierZones = [];
                var beSupplierZoneObject = new beSupplierZone(ctrl, $scope, $attrs);
                beSupplierZoneObject.initializeController();
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
                return getBeSupplierZoneTemplate(attrs);
            }

        };


        function getBeSupplierZoneTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection"
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div>'
               +  '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SupplierZoneId" '
            + required + ' label="Supplier Zone" datasource="supplierZones" selectedvalues="selectedSupplierZones"  onselectionchanged="onselectionchanged" entityName="Supplier Zone"></vr-select>'
            + '</div>'
        }

        function beSupplierZone(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    if (ctrl.supplierid != undefined)
                    {
                        return WhS_BE_SupplierZoneAPIService.GetSupplierZones(ctrl.supplierid).then(function (response) {
                            angular.forEach(response, function (itm) {
                                $scope.supplierZones.push(itm); 
                            });
                        });
                    }
                   
                }

                api.getData = function () {
                    return $scope.selectedSupplierZones;
                }

                api.setData = function (selectedIds) {
                    if ($attrs.ismultipleselection) {
                        for (var i = 0; i < selectedIds.length; i++) {
                            
                            var selectedSupplierZones = UtilsService.getItemByVal($scope.supplierZones, selectedIds[i], "SupplierZoneId");
                            if (selectedSupplierZones != null)
                                $scope.selectedSupplierZones.push(selectedSupplierZones);
                        }
                    } else {
                        var selectedSupplierZones = UtilsService.getItemByVal($scope.supplierZones, selectedIds, "SupplierZoneId");
                        if (selectedSupplierZones != null)
                            $scope.selectedSupplierZones = selectedSupplierZones;
                    }

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);