﻿'use strict';
app.directive('vrWhsBeSupplierzoneSelector', ['WhS_BE_SupplierZoneAPIService', 'UtilsService','VRUIUtilsService',
    function (WhS_BE_SupplierZoneAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                supplierid: "=",
                selectedvalues:'='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new supplierZoneCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
               
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
            + required + ' label="Supplier Zone" datasource="ctrl.searchSupplierZones" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="Supplier Zone"></vr-select>'
            + '</div>'
        }

        function supplierZoneCtor(ctrl, $scope, $attrs) {

            function initializeController() {

                ctrl.searchSupplierZones = function (searchValue) {
                    var filter = {
                        SupplierId: ctrl.supplierid,
                    }
                    return WhS_BE_SupplierZoneAPIService.GetSupplierZoneInfo(angular.toJson(filter), searchValue);
                }
                ctrl.supplierZones = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload;
                    }
                    if (selectedIds != undefined) {
                    return WhS_BE_SupplierZoneAPIService.GetSupplierZoneInfoByIds(selectedIds).then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });
                        VRUIUtilsService.setSelectedValues(selectedIds, 'SupplierZoneId', $attrs, ctrl);
                    });
                  }
                   
                }
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SupplierZoneId', $attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);